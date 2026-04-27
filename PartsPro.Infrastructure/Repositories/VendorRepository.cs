using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class VendorRepository(AppDbContext context) 
    : RepositoryBase<Vendor>(context), IVendorRepository
{
    // Inherits all basic CRUD from RepositoryBase
}
