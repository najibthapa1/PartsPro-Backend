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
}
