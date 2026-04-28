using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.PurchaseInvoices;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PurchaseInvoiceController : ControllerBase
{
    private readonly IPurchaseInvoiceService _purchaseInvoiceService;

    public PurchaseInvoiceController(IPurchaseInvoiceService purchaseInvoiceService)
    {
        _purchaseInvoiceService = purchaseInvoiceService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreatePurchaseInvoiceRequest request)
    {
        var purchaseInvoice = await _purchaseInvoiceService.CreatePurchaseInvoiceAsync(request);
        return Ok(purchaseInvoice);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchaseInvoices = await _purchaseInvoiceService.GetAllPurchaseInvoicesAsync();
        return Ok(purchaseInvoices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchaseInvoice = await _purchaseInvoiceService.GetPurchaseInvoiceByIdAsync(id);
        return Ok(purchaseInvoice);
    }

    [HttpGet("vendor/{vendorId:int}")]
    public async Task<IActionResult> GetByVendorId(int vendorId)
    {
        var purchaseInvoices = await _purchaseInvoiceService.GetPurchaseInvoicesByVendorIdAsync(vendorId);
        return Ok(purchaseInvoices);
    }
}