using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Vendors;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VendorController(IVendorService vendorService) : ControllerBase
{
    private readonly IVendorService _vendorService = vendorService;

    /// <summary>
    /// Retrieve paginated list of vendors using query parameters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendorResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var vendors = await _vendorService.GetAllVendorsAsync(pageNumber, pageSize);
        return Ok(vendors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VendorResponse>> GetById(int id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        return Ok(vendor);
    }

    /// <summary>
    /// Get a specific vendor by their Name (Username)
    /// </summary>
    [HttpGet("search/{name}")]
    public async Task<ActionResult<VendorResponse>> GetByName(string name)
    {
        var vendor = await _vendorService.GetVendorByNameAsync(name);
        return Ok(vendor);
    }

    /// <summary>
    /// Register a new vendor in the system
    /// </summary>
    [HttpPost("create")]
    public async Task<ActionResult<VendorResponse>> Create(VendorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _vendorService.CreateVendorAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Update existing vendor information
    /// </summary>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, VendorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _vendorService.UpdateVendorAsync(id, request);
        return Ok();
    }

    /// <summary>
    /// Permanently remove a vendor from the system
    /// </summary>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _vendorService.DeleteVendorAsync(id);
        return Ok();
    }
}
