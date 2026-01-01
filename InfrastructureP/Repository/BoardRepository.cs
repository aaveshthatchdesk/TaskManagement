using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Task.Infrastructure.Repository
{
    public  class BoardRepository: IBoardRepository
    {
        private readonly TaskDbContext _taskDbContext;

     
        public BoardRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
          
        }
    

        public async Task<IEnumerable<Board>> GetBoardsByProjectIdAsync(int projectId)
        {
            return await _taskDbContext.boards
               
                .Where(b => b.ProjectId == projectId)
                  .Include(b => b.TaskItems)
                   .ThenInclude(t => t.TaskAssignments)
                .ThenInclude(a => a.AppUser)
                .ToListAsync();
        }
        public async Task<IEnumerable<Board>> GetBoardsByUserAsync(int userId)
        {
           
            var projectIds = await _taskDbContext.tasks
       .Where(t => t.TaskAssignments.Any(a => a.AppUserId == userId))
       .Select(t => t.Board.ProjectId)
       .Distinct()
       .ToListAsync();

            
            return await _taskDbContext.boards
                .Where(b => projectIds.Contains(b.ProjectId))
                .Include(b => b.Project)
                .Include(b => b.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                        .ThenInclude(a => a.AppUser)
                .ToListAsync();
        }
        public async Task<Board?> GetBoardByIdAsync(int boardId)
        {
            return await _taskDbContext.boards
                .Include(b => b.TaskItems)
                .FirstOrDefaultAsync(b => b.Id == boardId);
        }
        public async Task<Board> CreateBoardAsync(Board board)
        {
            _taskDbContext.boards.Add(board);
            await _taskDbContext.SaveChangesAsync();
            return await _taskDbContext.boards
        .Include(b => b.TaskItems)
            .ThenInclude(t => t.TaskAssignments)
                .ThenInclude(a => a.AppUser)
        .FirstAsync(b => b.Id == board.Id);
        }
        public async Task<Board> UpdateBoardAsync(Board board)
        {
            _taskDbContext.boards.Update(board);
            await _taskDbContext.SaveChangesAsync();
            return board;
        }
        public async Task<bool> DeleteBoardAsync(Board board)
        {
            var existing = await _taskDbContext.boards
     .FirstOrDefaultAsync(b => b.Id == board.Id);

            if (existing == null)
                return false;
            _taskDbContext.boards.Remove(existing);
            var rows = await _taskDbContext.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<Board?> GetBoardByProjectAndNameAsync(
    int projectId,
    string boardName)
        {
            return await _taskDbContext.boards.FirstOrDefaultAsync(b =>
                b.ProjectId == projectId &&
                b.Name.ToLower() == boardName.ToLower()
            );
        }



    }
}
