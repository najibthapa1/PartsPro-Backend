using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Sales;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateSaleRequest request)
    {
        var sale = await _saleService.CreateSaleAsync(request);
        return Ok(sale);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetSaleByIdAsync(id);
        return Ok(sale);
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var sales = await _saleService.GetSalesByCustomerIdAsync(customerId);
        return Ok(sales);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllSalesAsync();
        return Ok(sales);
    }
    
    [HttpPost("{id:int}/email")]
    public async Task<IActionResult> SendInvoiceEmail(int id)
    {
        var isSent = await _saleService.SendInvoiceEmailAsync(id);

        if (!isSent)
        {
            return BadRequest(new
            {
                message = "Invoice email could not be sent. Please check customer email or SMTP settings."
            });
        }

        return Ok(new
        {
            message = "Invoice email sent successfully."
        });
    }
}