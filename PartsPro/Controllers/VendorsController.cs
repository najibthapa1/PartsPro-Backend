using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs;
using PartsPro.Application.Interfaces;
 
namespace PartsPro.Controllers;
 
[ApiController]
[Route("api/vendors")]
[Authorize(Roles = "Admin")]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;
 
    public VendorsController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }
 
    // GET /api/vendors
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendors = await _vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }
 
    // GET /api/vendors/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            return Ok(vendor);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
 
    // POST /api/vendors
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VendorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var vendor = await _vendorService.CreateVendorAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = vendor.Id }, vendor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
 
    // PUT /api/vendors/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] VendorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var vendor = await _vendorService.UpdateVendorAsync(id, request);
            return Ok(vendor);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
 
    // DELETE /api/vendors/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _vendorService.DeleteVendorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}