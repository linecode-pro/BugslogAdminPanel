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
    public class TagsController(ITagRepository _repository) : ControllerBase
    {
        // GET
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                ExecutionResult result = await _repository.GetAllTagsAsync();

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                List<SimpleTag> tags = result?.Data as List<SimpleTag> ?? new();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }                        
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateNewTag([FromBody] SimpleTag tag)
        {
            try
            {
                if (tag is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан тег.");
                }

                if (string.IsNullOrWhiteSpace(tag.Name))
                {
                    return BadRequest("Ошибка! Не указано Наименование тега.");
                }

                ExecutionResult result = await _repository.CreateNewTagAsync(tag);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                tag = result.Data as SimpleTag ?? throw new Exception("Ошибка! В ответе от базы данных не передан созданный тег.");

                return new ObjectResult(tag) { StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // POST
        [HttpPut]
        public async Task<IActionResult> UpdateTag([FromBody] SimpleTag tag)
        {
            try
            {
                if (tag is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан тег.");
                }

                if (string.IsNullOrWhiteSpace(tag.Name))
                {
                    return BadRequest("Ошибка! Не указано Наименование тега.");
                }

                ExecutionResult result = await _repository.UpdateTagAsync(tag);

                if (!result.CompletedSuccessfully)
                {
                    return BadRequest(result.ErrorText);
                }

                tag = result.Data as SimpleTag ?? throw new Exception("Ошибка! В ответе от базы данных не передан тег.");

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex!.Message);
            }
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> DeleteTag([FromBody] SimpleTag tag)
        {
            try
            {
                if (tag is null)
                {
                    return BadRequest("Ошибка! В теле запроса не передан тег.");
                }

                if (string.IsNullOrWhiteSpace(tag.Id))
                {
                    return BadRequest("Ошибка! Не указан Id тега.");
                }

                ExecutionResult result = await _repository.DeleteTagAsync(tag);

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
        [Route("/setdefaulttags")]
        public async Task<IActionResult> SetDefaultTags()
        {
            try
            {
                ExecutionResult result = await _repository.SetDefaultTagsAsync();

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
    }
}
