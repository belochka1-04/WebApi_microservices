using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelService.Services;

namespace WebApi_ModelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelController : BaseControllerClass
    {

        public ModelController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        #region model
        [HttpGet("getmodel")]
        public async Task<ActionResult<Model>> GetAllModels()
        {
            var job = await _databaseService.GetAllModelsAsync();
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }

        [HttpGet("getmodelbyid/{modelId}")]
        public async Task<ActionResult<Model>> GetModelById(int modelId)
        {
            var job = await _databaseService.GetModelById(modelId);
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }

        [HttpGet("getmodelbyid2/{modelId}")]
        public async Task<ActionResult<Model>> GetModelById2(int modelId)
        {
            var job = await _databaseService.GetModelById2(modelId);
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }

        [HttpGet("models-like-number/{modelNumber}")]
        public async Task<IActionResult> GetModelsLikeNumber(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetModelsWithNumAsync(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены.");
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("models-like-number3/{modelNumber}")]
        public async Task<IActionResult> GetModelsLikeNumber3(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetModelsWithNum3Async(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены.");
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("jd-models-like-number/{modelNumber}")]
        public async Task<IActionResult> GetJDModelsLikeNumber(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetJDModelsWithNumAsync(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены.");
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("jd-models-like-number-save/{jobId}/{modelNumber}/{maxLen}/{minLen}")]
        public async Task<IActionResult> GetSetJDModelsLikeNumber(string modelNumber, int jobId, int maxLen, int minLen)
        {
            try
            {
            
                await _databaseService.GetJobDocModelsLikeNameAndInsertAsync( modelNumber, jobId, maxLen, minLen);
                return NoContent(); // 204 No Content - No data is returned
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("find_model_leva/{modelNumber}/{brand?}")]
        public async Task<IActionResult> GetModelsLevenshtein(string modelNumber, string? brand)
        {
            try
            {

               var models= await _databaseService.GetModelsLevaAsync(modelNumber, brand);
                if (models == null || models.Count == 0)
                {
                    return NoContent(); // 204 No Content - No data is returned

                }
                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("models-by-number/{modelNumber}")]
        public async Task<IActionResult> GetModelsByNumber(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetModelsWithNumForTrimAsync(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены."); // Возвращаем 404 Not Found, если модели не найдены
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }
        [HttpGet("models-by-number3/{modelNumber}")]
        public async Task<IActionResult> GetModelsByNumber3(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetModelsWithNumForTrim3Async(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены."); // Возвращаем 404 Not Found, если модели не найдены
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }
        #endregion

        #region newModel
        [HttpGet("getnewmodel")]
        public async Task<ActionResult<List<NModel>>> GetAllNModels()
        {
            var job = await _databaseService.GetAllNModelsAsync();
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }

        [HttpGet("getnewmodelbyid/{modelId}")]
        public async Task<ActionResult<NModel>> GetNModelById(int modelId)
        {
            var job = await _databaseService.GetNModelById(modelId);
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }

        [HttpGet("getnewmodelbyid2/{modelId}")]
        public async Task<ActionResult<NModel>> GetNModelById2(int modelId)
        {
            var job = await _databaseService.GetNModelById2(modelId);
            if (job == null)
            {
                return NotFound("No models found.");
            }

            return Ok(job);
        }
        [HttpGet("newmodels-like-number/{modelNumber}")]
        public async Task<IActionResult> GetNModelsLikeNumber(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetNModelsWithNumAsync(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены.");
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("newmodels-by-number/{modelNumber}")]
        public async Task<IActionResult> GetNModelsByNumber(string modelNumber)
        {
            try
            {
                var models = await _databaseService.GetNModelsWithNumForTrimAsync(modelNumber);

                if (models == null || models.Count == 0)
                {
                    return NotFound("Модели не найдены."); // Возвращаем 404 Not Found, если модели не найдены
                }

                return Ok(models); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении моделей."); // Возвращаем 500 Internal Server Error
            }
        }
        #endregion
    }


}

