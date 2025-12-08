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

namespace Task.Infrastructure.Repository
{
    public  class BoardRepository:IBoardRepository
    {
        private readonly TaskDbContext _taskDbContext;

     
        public BoardRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
          
        }
        public async Task<Board?>GetBoardByIdAsync(int bordId)
        {
            return await _taskDbContext.boards.FirstOrDefaultAsync(b => b.Id == bordId);
        }
        public async Task<bool>UpdateBoardNameAsync(int boardId,string name)
        {
            var board=await _taskDbContext.boards.FirstOrDefaultAsync(board => board.Id == boardId);
            if(board == null)
                return false;
            board.Name = name;
            await _taskDbContext.SaveChangesAsync();
            return true;
        }
    }
}
