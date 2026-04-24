using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PartsPro.Domain.Entities;

namespace PartsPro.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Inventory 
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    // Users 
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    // Sales 
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<CreditRecord> CreditRecords => Set<CreditRecord>();

    // Purchases 
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

    // Customer portal 
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<PartRequest> PartRequests => Set<PartRequest>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // ApplicationUser 
        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Staff 
        builder.Entity<Staff>(e =>
        {
            e.HasKey(s => s.Id);

            e.HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(s => s.Department)
                .HasMaxLength(100);
        });

        // Customer 
        builder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);

            e.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(c => c.Address)
                .HasMaxLength(250);
        });

        // Vehicle 
        builder.Entity<Vehicle>(e =>
        {
            e.HasKey(v => v.Id);

            e.HasIndex(v => v.PlateNumber)
                .IsUnique();

            e.HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            e.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Vendor 
        builder.Entity<Vendor>(e =>
        {
            e.HasKey(v => v.Id);

            e.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(150);

            e.Property(v => v.Email)
                .HasMaxLength(150);

            e.Property(v => v.Phone)
                .HasMaxLength(20);
        });

        // Part 
        builder.Entity<Part>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasIndex(p => p.PartNumber)
                .IsUnique();

            e.HasOne(p => p.Vendor)
                .WithMany(v => v.Parts)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict); 

            e.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            e.Property(p => p.PartNumber)
                .IsRequired()
                .HasMaxLength(50);

            e.Property(p => p.Price)
                .HasColumnType("decimal(10,2)");

            e.Property(p => p.Stock)
                .HasDefaultValue(0);
        });

        // Sale 
        builder.Entity<Sale>(e =>
        {
            e.HasKey(s => s.Id);

            e.HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(s => s.TotalAmount)
                .HasColumnType("decimal(10,2)");

            e.Property(s => s.DiscountAmount)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            e.Property(s => s.FinalAmount)
                .HasColumnType("decimal(10,2)");
        });

        // SaleItem 
        builder.Entity<SaleItem>(e =>
        {
            e.HasKey(si => si.Id);

            e.HasOne(si => si.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(si => si.Part)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(si => si.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(si => si.UnitPrice)
                .HasColumnType("decimal(10,2)");

            
            e.Ignore(si => si.LineTotal);
        });

        // PurchaseInvoice 
        builder.Entity<PurchaseInvoice>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasOne(p => p.Vendor)
                .WithMany(v => v.PurchaseInvoices)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(p => p.TotalCost)
                .HasColumnType("decimal(10,2)");

            e.Property(p => p.Status)
                .HasConversion<string>(); // store enum as string in DB
        });

        // PurchaseItem 
        builder.Entity<PurchaseItem>(e =>
        {
            e.HasKey(pi => pi.Id);

            e.HasOne(pi => pi.PurchaseInvoice)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pi => pi.Part)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(pi => pi.UnitCost)
                .HasColumnType("decimal(10,2)");

            e.Ignore(pi => pi.LineTotal);
        });

        // CreditRecord 
        builder.Entity<CreditRecord>(e =>
        {
            e.HasKey(c => c.Id);

            e.HasOne(c => c.Customer)
                .WithMany(cu => cu.CreditRecords)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(c => c.Amount)
                .HasColumnType("decimal(10,2)");

            e.Property(c => c.Status)
                .HasConversion<string>();
            
            e.Ignore(c => c.IsOverdue);
        });

        // Appointment 
        builder.Entity<Appointment>(e =>
        {
            e.HasKey(a => a.Id);

            e.HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.Vehicle)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(a => a.Status)
                .HasConversion<string>();

            e.Property(a => a.ServiceType)
                .IsRequired()
                .HasMaxLength(150);
        });

        // PartRequest 
        builder.Entity<PartRequest>(e =>
        {
            e.HasKey(pr => pr.Id);

            e.HasOne(pr => pr.Customer)
                .WithMany(c => c.PartRequests)
                .HasForeignKey(pr => pr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(pr => pr.Urgency)
                .HasConversion<string>();

            e.Property(pr => pr.PartName)
                .IsRequired()
                .HasMaxLength(150);
        });

        // Review 
        builder.Entity<Review>(e =>
        {
            e.HasKey(r => r.Id);

            e.HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.ToTable(t => t.HasCheckConstraint(
                "CK_Review_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5"));
        });
    }
}