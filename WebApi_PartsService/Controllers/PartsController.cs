using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController : BaseControllerClass
    {

        private readonly ILogger<PartsController> _logger;

        public PartsController(IDatabaseService databaseService, ILogger<PartsController> logger) : base(databaseService)
        {
            _logger = logger;
        }

        [HttpGet("getPartsByModelId/{modelId}")]
        public async Task<IActionResult> GetPartsByModelId(int modelId)
        {
            if (modelId <= 0)
            {
                return BadRequest("�������� ������������� ������."); // ���������� 400 Bad Request
            }

            try
            {
                var parts = await _databaseService.GetPartsByModelIdAsync(modelId);

                if (parts == null || parts.Count == 0)
                {
                    return NotFound("����� �� ������� ��� ���������� �������������� ������."); // ���������� 404 Not Found
                }

                return Ok(parts); // ���������� 200 OK � ���������� �������
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to read parts for model {ModelId}", modelId);
                // �������� ������
                return StatusCode(500, "������ ������� ��� ��������� ������."); // ���������� 500 Internal Server Error
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read parts for model {ModelId}", modelId);
                // �������� ������
                return StatusCode(500, "������ ������� ��� ��������� ������."); // ���������� 500 Internal Server Error
            }
        }


    }
}
