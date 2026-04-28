using PartsPro.Application.DTOs.Sales;

namespace PartsPro.Application.Interfaces.Services;

public interface ISaleService
{
    Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request);

    Task<SaleResponse> GetSaleByIdAsync(int id);

    Task<List<SaleResponse>> GetAllSalesAsync();

    Task<List<SaleResponse>> GetSalesByCustomerIdAsync(int customerId);
}