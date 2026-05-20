using Microsoft.AspNetCore.Identity;
namespace PartsPro.Domain.Entities;

public class ApplicationUser: IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Staff? Staff { get; set; }
    public Customer? Customer { get; set; }
}