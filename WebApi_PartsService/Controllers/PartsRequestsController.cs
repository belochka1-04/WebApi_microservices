using Microsoft.AspNetCore.Mvc;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using WebApi_PartsService.Services;
using KameraData.Data.Dtos;

[ApiController]
[Route("api/[controller]")]
public class PartsRequestsController : ControllerBase
{
    private readonly IDatabaseService _requestService;

    [HttpPost]
    public async Task<ActionResult<PartsRequest>> Create([FromBody] CreateRequestDto dto)
    {
        var request = await _requestService.CreateAsync(dto.RecognizedPartNumber, dto.UserId);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    // GET /api/partsrequests/{id} - получить статус (для polling)
    [HttpGet("{id}")]
    public async Task<ActionResult<PartsRequest>> GetById(int id)
    {
        var request = await _requestService.GetByIdAsync(id);
        if (request == null) return NotFound();
        return Ok(request);
    }
}
