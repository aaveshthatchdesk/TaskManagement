using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public  interface IBoardRepository
    {
        Task<Board> GetBoardByIdAsync(int boardId);
        Task<bool> UpdateBoardNameAsync(int  boardId, string name);
    }
}
