using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService boardService;

        public BoardController(IBoardService boardService)
        {
            this.boardService = boardService;
        }
        [HttpPut("{boardId}/Rename")]
        public async Task<IActionResult> RenameBoard(int boardId, [FromBody] string newName)
        {
            var result = await boardService.RenameBoardAsync(boardId, newName);

            if (!result)
                return NotFound("Board not found or invalid name");

            return Ok("Board name updated successfully");
        }
    }
}
