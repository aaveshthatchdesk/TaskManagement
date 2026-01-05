using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IBoardRepository boardRepository;
        private readonly IActivityLogService activityLogService;

        public TaskItemService(ITaskItemRepository taskItemRepository, IBoardRepository boardRepository, IActivityLogService activityLogService)
        {
            _taskItemRepository = taskItemRepository;
            this.boardRepository = boardRepository;
            this.activityLogService = activityLogService;
        }

        public async Task<bool> UpdateTaskAsync(int taskId, TaskItemDto dto, int updatedByUserId)
        {
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return false;


            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.LastUpdatedOn = DateTime.UtcNow;

            task.Order = dto.Order;


            var success = await _taskItemRepository.SaveChangesAsync();
            if (success)
            {
                await activityLogService.LogAsync(
                    projectId: task.Board.ProjectId,
                    userId: updatedByUserId, // 👈 pass from controller
                    actionType: "TaskUpdated",
                    description: $"updated task '{task.Title}'",
                    taskId: task.Id,
                    boardId: task.BoardId
                );
            }

            return success;


        }
        public async Task<bool> UpdateTaskDescriptionAsync(int taskId, string description)
        {
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return false;



            task.Description = description;
            task.LastUpdatedOn = DateTime.UtcNow;


            return await _taskItemRepository.SaveChangesAsync();


        }
        public async Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(int projectId)
        {
            var tasks = await _taskItemRepository.GetByProjectIdAsync(projectId);
            return tasks.Select(MapToDto);

        }
        public async Task<TaskItemDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;
            return MapToDto(task);
        }

        public async Task<List<MemberBoardDto>> GetMemberBoardsAsync(int memberId)
        {
            var tasks = await _taskItemRepository.GetTasksForMemberAsync(memberId);
            var result = tasks
                .GroupBy(t => t.Board.Name.Trim().ToLower())
                .Select(g => new MemberBoardDto
                {
                    BoardName = g.First().Board.Name,
                    Tasks = g.OrderBy(t => t.Order)
                             .Select(t => new TaskItemDto
                             {
                                 Id = t.Id,
                                 Title = t.Title,

                                 Priority = t.Priority,
                                 DueDate = t.DueDate,
                                 BoardId = t.BoardId,
                                 ProjectId = t.Board.ProjectId,
                                 ProjectName = t.Board.Project.Name,

                                 CreatedOn = t.CreatedOn,
                                 LastUpdatedOn = t.LastUpdatedOn,
                                 IsCompleted = t.IsCompleted,
                                 Order = t.Order,
                                 TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                                 {
                                     TaskItemId = a.TaskItemId,
                                     AppUserId = a.AppUserId,
                                     AppUserName = a.AppUser.Name
                                 }).ToList()
                             })
                                .ToList()
                })
                                    .OrderBy(t => t.BoardName)
                                    .ToList();
            return result;
        }

        public async Task<TaskItemDto> CreateTaskAsync(int createdByUserId, TaskItemDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                BoardId = dto.BoardId,
                CreatedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow,
                Order = dto.Order,
                IsCompleted = false,
            };
            var created = await _taskItemRepository.CreateAsync(task, createdByUserId);

            var board = await boardRepository.GetBoardByIdAsync(created.BoardId);
            await activityLogService.LogAsync(
                       projectId: board.ProjectId,
                       userId: createdByUserId,
                       actionType: "TaskCreated",
                       description: $"created task '{created.Title}'",
                       taskId: created.Id,
                       boardId: created.BoardId
                );

            return new TaskItemDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Priority = created.Priority,
                DueDate = created.DueDate,
                BoardId = created.BoardId,
                CreatedOn = created.CreatedOn,
                LastUpdatedOn = created.LastUpdatedOn,
                IsCompleted = false,
                TaskAssignments = new List<TaskAssignmentDto>()
            };
        }
        public async Task<TaskItemDto> UpdateTasksAsync(int taskId, TaskItemDto dto)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new Exception("Task not found");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.Order = dto.Order;
            task.IsCompleted = dto.IsCompleted;
            task.LastUpdatedOn = DateTime.UtcNow;
            task.CompletedDate = dto.IsCompleted ? DateTime.UtcNow : null;


            var updated = await _taskItemRepository.UpdateAsync(task);
            return new TaskItemDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                Priority = updated.Priority,
                DueDate = updated.DueDate,
                BoardId = updated.BoardId,
                IsCompleted = updated.IsCompleted,
                LastUpdatedOn = updated.LastUpdatedOn,
                CompletedDate = updated.CompletedDate,
                TaskAssignments = new List<TaskAssignmentDto>()


            };
        }
        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                return false;
            var projectId = task.Board.ProjectId;
            var title = task.Title;
            var BoardId = task.BoardId;


            var success = await _taskItemRepository.DeleteAsync(task);
            if (success)
            {
                await activityLogService.LogAsync(
                     projectId: projectId,
    userId: userId,
    actionType: "TaskDeleted",
    description: $"deleted task '{title}'",
    taskId: taskId,
    boardId: BoardId
            );
            }
            return success;
        }
        private static TaskItemDto MapToDto(TaskItem t)
        {
            return new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DueDate = t.DueDate,
                BoardId = t.BoardId,
                SprintId = t.SprintId,
                LastUpdatedOn = DateTime.UtcNow,
                Order = t.Order,
                IsCompleted = t.IsCompleted,
                CompletedDate = t.CompletedDate,
                TaskCreators = t.TaskCreators.Select(c => new TaskCreatorDto
                {
                    TaskItemId = c.TaskItemId,
                    CreatedByUserId = c.AppUserId,
                    CreatedByUserName = c.AppUser.Name
                }).ToList(),

                TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                {
                    TaskItemId = a.TaskItemId,
                    AppUserId = a.AppUserId,
                    AppUserName = a.AppUser.Name
                }).ToList()
            };
        }
        public async Task<bool> ReorderTasksAsync(List<TaskReorderDto> tasks,int userId)
        {
            //var taskIds = tasks.Select(t => t.TaskId).ToList();
            //var taskItems = await _taskItemRepository.GetByIdsAsync(taskIds);
            //foreach (var task in taskItems)
            //{
            //    var dto = tasks.First(t => t.TaskId == task.Id);

            //    task.BoardId = dto.BoardId;
            //    task.Order = dto.Order;
            //    if (dto.IsDoneBoard)
            //    {

            //        task.IsCompleted = true;
            //        task.CompletedDate = DateTime.UtcNow;

            //    }
            //    else
            //    {
            //        task.IsCompleted = false;
            //        task.CompletedDate = null;

            //    }

            //    task.LastUpdatedOn = DateTime.UtcNow;
            //}
            //return await _taskItemRepository.SaveChangesAsync();

            var taskIds = tasks.Select(t => t.TaskId).ToList();

            // 🔑 Need Board + Project
            var taskItems = await _taskItemRepository
                .GetByIdsWithBoardAndProjectAsync(taskIds);

            foreach (var task in taskItems)
            {
                var dto = tasks.First(t => t.TaskId == task.Id);

                var oldBoardId = task.BoardId;
                var oldBoardName = task.Board.Name;

                bool boardChanged = oldBoardId != dto.BoardId;
                bool movedToDone = boardChanged && dto.IsDoneBoard;
                bool movedOutOfDone =
                    boardChanged &&
                    !dto.IsDoneBoard &&
                    oldBoardName.Equals("Done", StringComparison.OrdinalIgnoreCase);

                // 🔁 Apply changes
                task.BoardId = dto.BoardId;
                task.Order = dto.Order;

                if (movedToDone)
                {
                    task.IsCompleted = true;
                    task.CompletedDate ??= DateTime.UtcNow;

                    // ✅ LOG COMPLETION
                    await activityLogService.LogAsync(
                        projectId: task.Board.ProjectId,
                        userId: userId,
                        actionType: "TaskCompleted",
                        description: $"completed task '{task.Title}'",
                        taskId: task.Id,
                        boardId: dto.BoardId
                    );
                }
                else
                {
                    if (movedOutOfDone)
                    {
                        task.IsCompleted = false;
                        task.CompletedDate = null;
                    }

                    if (boardChanged)
                    {
                        await activityLogService.LogAsync(
                            projectId: task.Board.ProjectId,
                            userId: userId,
                            actionType: "TaskMoved",
                            description: $"moved  '{task.Title}' from {oldBoardName}",
                            taskId: task.Id,
                            boardId: dto.BoardId
                        );
                    }
                }

                task.LastUpdatedOn = DateTime.UtcNow;
            }

            return await _taskItemRepository.SaveChangesAsync();
        }

        public async Task<bool> ReorderTaskForMembersAsync(List<TaskReorderForMembersDto> tasks, int UserId)
        {
            var taskIds = tasks.Select(t => t.TaskId).ToList();
            var taskItems = await _taskItemRepository
       .GetByIdsWithBoardAndProjectAsync(taskIds);

            foreach (var task in taskItems)
            {
                var dto = tasks.First(t => t.TaskId == task.Id);

                var oldBoardName = task.Board.Name;

                // 🔑 Resolve correct board INSIDE API
                var targetBoard = await boardRepository.GetBoardByProjectAndNameAsync(
                    task.Board.ProjectId,
                    dto.TargetBoardName
                );

                if (targetBoard == null)
                    continue;

                task.BoardId = targetBoard.Id;
                task.Order = dto.Order;

                bool boardChanged = !oldBoardName.Equals(targetBoard.Name, StringComparison.OrdinalIgnoreCase);

                bool movedToDone =
           boardChanged &&
           targetBoard.Name.Equals("Done", StringComparison.OrdinalIgnoreCase);

                bool movedOutOfDone =
                    boardChanged &&
                    oldBoardName.Equals("Done", StringComparison.OrdinalIgnoreCase);

                // Completion logic
                if (dto.TargetBoardName.Equals("Done", StringComparison.OrdinalIgnoreCase))
                {
                    task.IsCompleted = true;
                    task.CompletedDate ??= DateTime.UtcNow;
                }
                else
                {
                    task.IsCompleted = false;
                    task.CompletedDate = null;
                }

                task.LastUpdatedOn = DateTime.UtcNow;

                if (movedToDone)
                {
                    task.IsCompleted = true;
                    task.CompletedDate ??= DateTime.UtcNow;

                    // ✅ LOG TASK COMPLETED
                    await activityLogService.LogAsync(
                        projectId: task.Board.ProjectId,
                        userId: UserId,
                        actionType: "TaskCompleted",
                        description: $"completed task '{task.Title}'",
                        taskId: task.Id,
                        boardId: targetBoard.Id
                    );
                }
                else
                {
                    if (movedOutOfDone)
                    {
                        task.IsCompleted = false;
                        task.CompletedDate = null;
                    }
                    if (boardChanged)
                    {
                        await activityLogService.LogAsync(
                              projectId: task.Board.ProjectId,
                              userId: UserId,
                               actionType: "TaskMoved",
                               description: $"moved '{task.Title}' from {oldBoardName} to {targetBoard.Name}",
                                taskId: task.Id,
                                boardId: targetBoard.Id
                            );
                    }
                }
            }

            return await _taskItemRepository.SaveChangesAsync();

        }
    }
}

