using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Parts;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class PartController(IPartService partService) : ControllerBase
{
    private readonly IPartService _partService = partService;

    /// <summary>
    /// Retrieve paginated list of parts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var parts = await _partService.GetAllPartsAsync(pageNumber, pageSize);
        return Ok(parts);
    }

    /// <summary>
    /// Get a specific part by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PartResponse>> GetById(int id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        return Ok(part);
    }

    /// <summary>
    /// Create a new part in the inventory
    /// </summary>
    [HttpPost("create")]
    public async Task<ActionResult<PartResponse>> Create(CreatePartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _partService.CreatePartAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Update existing part details
    /// </summary>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, CreatePartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _partService.UpdatePartAsync(id, request);
        return Ok();
    }

    /// <summary>
    /// Delete a part from the system
    /// </summary>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _partService.DeletePartAsync(id);
        return Ok();
    }
}
