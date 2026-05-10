using Microsoft.EntityFrameworkCore;
using PartsPro.Domain.Entities;
using PartsPro.Models;

namespace PartsPro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<ServiceHistory> ServiceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer - Vehicle relationship
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer - PurchaseHistory relationship
            modelBuilder.Entity<PurchaseHistory>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.PurchaseHistories)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer - ServiceHistory relationship
            modelBuilder.Entity<ServiceHistory>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.ServiceHistories)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}