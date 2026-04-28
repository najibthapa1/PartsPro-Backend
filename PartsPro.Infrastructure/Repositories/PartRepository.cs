using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class PartRepository(AppDbContext context)
    : RepositoryBase<Part>(context), IPartRepository
{
}