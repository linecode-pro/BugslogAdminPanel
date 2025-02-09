using NoSqlBaseApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using NoSqlBaseApi.Repositories;

namespace NoSqlBaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromptsController(IPromptRepository _repository) : ControllerBase
    {
        // GET
        public async Task<IActionResult> GetAllPrompts()
        {
            try
            {
                ExecutionResult result = await _repository.GetAllPromptsAsync();

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                List<SimplePrompt> prompts = result?.Data as List<SimplePrompt> ?? new();

                return Ok(prompts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }                        
        }


        // POST
        [HttpPost]
        public async Task<IActionResult> CreateNewTag([FromBody] SimplePrompt promt)
        {
            try
            {
                if (promt is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан промт.");
                }

                if (string.IsNullOrWhiteSpace(promt.Message))
                {
                    return BadRequest("Ошибка! Не указано Наименование промта.");
                }

                ExecutionResult result = await _repository.CreateNewPromptAsync(promt);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                promt = result.Data as SimplePrompt ?? throw new Exception("Ошибка! В ответе от базы данных не передан созданный промт.");

                return new ObjectResult(promt) { StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // POST
        [HttpPut]
        public async Task<IActionResult> UpdateTag([FromBody] SimplePrompt promt)
        {
            try
            {
                if (promt is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан промт.");
                }

                if (string.IsNullOrWhiteSpace(promt.Message))
                {
                    return BadRequest("Ошибка! Не указано Наименование промта.");
                }

                ExecutionResult result = await _repository.UpdatePromptAsync(promt);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                promt = result.Data as SimplePrompt ?? throw new Exception("Ошибка! В ответе от базы данных не передан промт.");

                return Ok(promt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> DeleteTag([FromBody] SimplePrompt promt)
        {
            try
            {
                if (promt is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан промт.");
                }

                if (string.IsNullOrWhiteSpace(promt.Id))
                {
                    return BadRequest("Ошибка! Не указан Id промта.");
                }

                ExecutionResult result = await _repository.DeletePromptAsync(promt);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // POST
        [HttpPost]
        [Route("/setdefaultprompts")]
        public async Task<IActionResult> SetDefaultTags()
        {
            try
            {
                ExecutionResult result = await _repository.SetDefaultPromptsAsync();

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // GET
        [Route("/getdefaultprompt")]
        public async Task<IActionResult> GetDefaultPrompt()
        {
            try
            {
                ExecutionResult result = await _repository.GetDefaultPromptAsync();

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                SimplePrompt prompt = result?.Data as SimplePrompt ?? new();

                return Ok(prompt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }
    }
}
