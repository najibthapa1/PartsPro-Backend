using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class VehicleRepository(AppDbContext context) 
    : RepositoryBase<Vehicle>(context), IVehicleRepository
{
    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
    {
        return await Context.Set<Vehicle>()
            .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
    }

    public async Task<List<Vehicle>> GetByCustomerIdAsync(int customerId)
    {
        return await Context.Set<Vehicle>()
            .Where(v => v.CustomerId == customerId)
            .ToListAsync();
    }
}

