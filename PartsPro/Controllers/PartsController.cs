using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs;
using PartsPro.Application.Interfaces;
 
namespace PartsPro.Controllers;
 
[ApiController]
[Route("api/parts")]
[Authorize]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;
 
    public PartsController(IPartService partService)
    {
        _partService = partService;
    }
 
    // GET /api/parts — Staff and Admin can view parts
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll()
    {
        var parts = await _partService.GetAllPartsAsync();
        return Ok(parts);
    }
 
    // GET /api/parts/5
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var part = await _partService.GetPartByIdAsync(id);
            return Ok(part);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
 
    // GET /api/parts/low-stock — F15 hook
    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLowStock()
    {
        var parts = await _partService.GetLowStockPartsAsync();
        return Ok(parts);
    }
 
    // POST /api/parts — Admin only
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] PartRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var part = await _partService.CreatePartAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = part.Id }, part);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
 
    // PUT /api/parts/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PartRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var part = await _partService.UpdatePartAsync(id, request);
            return Ok(part);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
 
    // DELETE /api/parts/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _partService.DeletePartAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}