using System;
using System.ComponentModel.DataAnnotations;

namespace PartsPro.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalSpent { get; set; }

        // Navigation
        public ICollection<Vehicle> Vehicles { get; set; }
        public ICollection<PurchaseHistory> PurchaseHistories { get; set; }
        public ICollection<ServiceHistory> ServiceHistories { get; set; }
    }
}