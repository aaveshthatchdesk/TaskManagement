using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.Application.DTOs;
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
        //[HttpPut("{boardId}/Rename")]
        //public async Task<IActionResult> RenameBoard(int boardId, [FromBody] string newName)
        //{
        //    var result = await boardService.RenameBoardAsync(boardId, newName);

        //    if (!result)
        //        return NotFound("Board not found or invalid name");

        //    return Ok("Board name updated successfully");
        //}

        [HttpGet]
        public async Task<IActionResult> GetBoards([FromQuery] int projectId)
        {
            var boards = await boardService.GetBoardsByProjectAsync(projectId);
            return Ok(boards);
        }
        [HttpGet("{boardId}")]
        public async Task<IActionResult> GetBoard(int projectId, int boardId)
        {
            var board = await boardService.GetBoardByIdAsync(projectId, boardId);
            if (board == null)
                return NotFound();
            return Ok(board);
        }
        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateBoard(int projectId,[FromBody] Task.Application.DTOs.BoardDto dto)
        {
            dto.ProjectId = projectId;
            dto.TaskItems = new List<TaskItemDto>();
            var createdBoard = await boardService.CreateBoardAsync(dto);
            return Ok(createdBoard);
        }
        [HttpPut("{boardId}")]
        public async Task<IActionResult> UpdateBoard([FromQuery] int projectId, int boardId, [FromBody] Task.Application.DTOs.BoardDto dto)
        {
            var updatedBoard = await boardService.UpdateBoardAsync(projectId, boardId, dto);
            if (updatedBoard == null)
                return NotFound();
            return Ok(updatedBoard);
        }
        [HttpDelete("{boardId}")]
            
        public async Task<IActionResult> DeleteBoard([FromQuery] int projectId, int boardId)
        {
            var result = await boardService.DeleteBoardAsync(projectId, boardId);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
