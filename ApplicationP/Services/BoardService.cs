using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;

namespace Task.Application.Services
{
    public  class BoardService:IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }
        public async Task<bool> RenameBoardAsync(int  boardId,string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _boardRepository.UpdateBoardNameAsync(boardId, name);
        }
    }
    
}
