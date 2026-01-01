using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }
        //public async Task<bool> RenameBoardAsync(int  boardId,string name)
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //        return false;

        //    return await _boardRepository.UpdateBoardNameAsync(boardId, name);
        //}


        public async Task<IEnumerable<BoardDto>> GetBoardsByProjectAsync(int projectId)
        {
            var boards = await _boardRepository.GetBoardsByProjectIdAsync(projectId);
            return boards.Select(b => new BoardDto
            {
                Id = b.Id,
                Name = b.Name,
                ProjectId = b.ProjectId,
                TaskItems = b.TaskItems.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    BoardId = t.BoardId,
                    SprintId = t.SprintId,
                    DueDate= t.DueDate,
                    Priority = t.Priority,
                    Order = t.Order,
                    IsCompleted = t.IsCompleted,
                    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                    {
                        TaskItemId = a.TaskItemId,
                        AppUserId = a.AppUserId,
                        AppUserName = a.AppUser.Name
                    }).ToList()
                }).ToList()
            });
        }

        public async Task<BoardDto?> GetBoardByIdAsync(int projectId, int boardId)
        {
            var board = await _boardRepository.GetBoardByIdAsync(boardId);
            if (board == null || board.ProjectId != projectId)
                return null;
            return new BoardDto
            {
                Id = board.Id,
                Name = board.Name,
                ProjectId = board.ProjectId,
                TaskItems = board.TaskItems.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    BoardId = t.BoardId,
                    SprintId = t.SprintId,
                    Priority = t.Priority,
                    Order = t.Order,
                    IsCompleted = t.IsCompleted,

                }).ToList()
            };
        }

        public async Task<IEnumerable<BoardDto>> GetBoardsByUserAsync(int userId)
        {
            var boards = await _boardRepository.GetBoardsByUserAsync(userId);
            var result= boards
                        .GroupBy(b=>b.Name.Trim().ToLower())
                         .Select(b =>
                         {
                             var allTasks = b
                                 .SelectMany(b => b.TaskItems ?? Enumerable.Empty<TaskItem>())
                                 .Where(t => t.TaskAssignments.Any(a => a.AppUserId == userId))
                                 .OrderBy(t => t.Order)
                                 .ToList();
                             return new BoardDto
                             {
                                 
                                 Name = b.First().Name,
                                 TaskItems = allTasks

                                     .Select(t => new TaskItemDto
                                     {
                                         Id = t.Id,
                                         Title = t.Title,
                                         Description = t.Description,
                                         BoardId = t.BoardId,
                                         ProjectId = t.Board.ProjectId,
                                         ProjectName = t.Board.Project.Name,
                                         SprintId = t.SprintId,
                                         DueDate = t.DueDate,
                                         Priority = t.Priority,
                                         CompletedDate = t.CompletedDate,
                                         Order = t.Order,
                                         IsCompleted = t.IsCompleted,
                                         TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                                         {
                                             TaskItemId = a.TaskItemId,
                                             AppUserId = a.AppUserId,
                                             AppUserName = a.AppUser.Name
                                         }).ToList()
                                     }).ToList()
                             };
                    }).ToList();

            return result;

      
        }


        public async Task<BoardDto> CreateBoardAsync(BoardDto dto)
        {
            //var projectExists = await _boardRepository.projects.AnyAsync(p => p.Id == dto.ProjectId);
            //if (!projectExists)
            //    throw new ArgumentException("Project does not exist");
            var board = new Board
            {
                Name = dto.Name,
                ProjectId = dto.ProjectId
            };
            var createdBoard = await _boardRepository.CreateBoardAsync(board);
            return new BoardDto
            {
                Id = createdBoard.Id,
                Name = createdBoard.Name,
                ProjectId = createdBoard.ProjectId,
                 
               TaskItems = createdBoard.TaskItems?
            .OrderBy(t => t.Order)
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DueDate = t.DueDate,
                BoardId = t.BoardId,
                Order = t.Order,
                SprintId = t.SprintId,
                IsCompleted = t.IsCompleted,
                  TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                  {
                      TaskItemId = a.TaskItemId,
                      AppUserId = a.AppUserId,
                      AppUserName = a.AppUser.Name
                  }).ToList() ?? new List<TaskAssignmentDto>()
            })
            .ToList() ?? new List<TaskItemDto>()
                 };
        
        }
        public async Task<BoardDto> UpdateBoardAsync(int projectId, int boardId, BoardDto dto)
        {
            var board = await _boardRepository.GetBoardByIdAsync(boardId);
            if (board == null || board.ProjectId != projectId)
                throw new ArgumentException("Board does not exist in the specified project");
            board.Name = dto.Name;
            var updatedBoard = await _boardRepository.UpdateBoardAsync(board);
            return new BoardDto
            {
                Id = updatedBoard.Id,
                Name = updatedBoard.Name,
                ProjectId = updatedBoard.ProjectId,
                TaskItems = updatedBoard.TaskItems.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    BoardId = t.BoardId,
                    SprintId = t.SprintId,
                    Priority = t.Priority,
                    Order = t.Order,
                    IsCompleted = t.IsCompleted,
                }).ToList()
            };
        }
        public async Task<bool> DeleteBoardAsync(int projectId, int boardId)
        {
            var board = await _boardRepository.GetBoardByIdAsync(boardId);
            if (board == null || board.ProjectId != projectId)
                return false;
            return await _boardRepository.DeleteBoardAsync(board);

          
        }
    }
    
}
