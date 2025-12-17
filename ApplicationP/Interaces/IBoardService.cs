using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface IBoardService
    {
        //Task<bool> RenameBoardAsync(int boardId,string Name);
        Task<IEnumerable<BoardDto>> GetBoardsByProjectAsync(int projectId);
        Task<BoardDto?> GetBoardByIdAsync(int projectId, int boardId);
        Task<BoardDto> CreateBoardAsync(BoardDto dto);
        Task<BoardDto> UpdateBoardAsync(int projectId, int boardId, BoardDto dto);
        Task<bool> DeleteBoardAsync(int projectId, int boardId);
    }
}
