using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) 
    : RepositoryBase<Customer>(context), ICustomerRepository
{
    public async Task<Customer?> GetByUserIdAsync(string userId)
    {
        return await Context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string query)
    {
        var queryLower = query.ToLower();

        return await Context.Set<Customer>()
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.CreditRecords)
            .Where(c => 
                c.Id.ToString() == query || 
                (c.User.FullName != null && c.User.FullName.ToLower().Contains(queryLower)) ||
                (c.User.PhoneNumber != null && c.User.PhoneNumber.Contains(queryLower)) ||
                c.Vehicles.Any(v => v.PlateNumber.ToLower().Contains(queryLower)))
            .ToListAsync();
    }
}
