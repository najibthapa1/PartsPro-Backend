using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Staff;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    /// <summary>
    /// Grabs a list of all staff members.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var staffList = await _staffService.GetAllStaffAsync(pageNumber, pageSize);
        return Ok(staffList);
    }

    /// <summary>
    /// Fetches the details of a single staff member using their unique ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StaffResponse>> GetById(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        return Ok(staff);
    }

    /// <summary>
    /// Updates a staff member's information (like changing their department or name).
    /// </summary>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, UpdateStaffRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _staffService.UpdateStaffAsync(id, request);
        return Ok();
    }

    /// <summary>
    /// Deactivates a staff member's account. We use a soft delete here so we don't lose their past data.
    /// </summary>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _staffService.DeleteStaffAsync(id);
        return Ok();
    }
}
