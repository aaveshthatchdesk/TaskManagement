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
        

        Task<IEnumerable<Board>> GetBoardsByProjectIdAsync(int projectId);
        Task<IEnumerable<Board>> GetBoardsByUserAsync(int userId);
        Task<Board?> GetBoardByIdAsync(int id);
        Task<Board> CreateBoardAsync(Board board);
        Task<Board> UpdateBoardAsync(Board board);
        Task<bool> DeleteBoardAsync(Board board);
        Task<Board?> GetBoardByProjectAndNameAsync(int projectId, string boardName);
    }
}
