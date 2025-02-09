using AIEngineApi.Data;
using AIEngineApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIEngineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController(IChatService chatService) : ControllerBase
    {
        // POST
        [HttpPost]
        public async Task<IActionResult> DefineTagForMessage([FromBody] string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return BadRequest("Ошибка! Получен пустой текст сообщения.");
                }
                
                ExecutionResult result = await chatService.GetTagByMessageAsync(message);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                string tag = result?.Data as string ?? string.Empty;

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }
    }
}
