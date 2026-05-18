using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Customers;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;

namespace PartsPro.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IRepositoryBase<Appointment> _appointmentRepository;
    private readonly IRepositoryBase<PartRequest> _partRequestRepository;
    private readonly IRepositoryBase<Review> _reviewRepository;
    private readonly IRepositoryBase<CreditRecord> _creditRecordRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        ISaleRepository saleRepository,
        IRepositoryBase<Appointment> appointmentRepository,
        IRepositoryBase<PartRequest> partRequestRepository,
        IRepositoryBase<Review> reviewRepository,
        IRepositoryBase<CreditRecord> creditRecordRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _saleRepository = saleRepository;
        _appointmentRepository = appointmentRepository;
        _partRequestRepository = partRequestRepository;
        _reviewRepository = reviewRepository;
        _creditRecordRepository = creditRecordRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerProfileResponse>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var customers = await _customerRepository
            .FindAll(trackChanges: false)
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.Sales)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return customers.Select(customer =>
        {
            var totalSpent = customer.Sales.Sum(s => s.FinalAmount);

            return new CustomerProfileResponse
            {
                Id = customer.Id,
                UserId = customer.UserId,
                FullName = customer.FullName,
                Email = customer.User?.Email ?? string.Empty,
                PhoneNumber = customer.User?.PhoneNumber ?? string.Empty,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.User?.IsActive ?? false,
                VehicleCount = customer.Vehicles.Count,
                SaleCount = customer.Sales.Count,
                TotalSpent = totalSpent,
                LoyaltyPoints = CalculateLoyaltyPoints(totalSpent)
            };
        });
    }

    public async Task<CustomerProfileResponse> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository
            .FindByCondition(c => c.Id == customerId, trackChanges: false)
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.Sales)
            .FirstOrDefaultAsync();

        if (customer == null)
            throw new NotFoundException($"Customer with ID {customerId} not found.");

        var totalSpent = customer.Sales.Sum(s => s.FinalAmount);

        return new CustomerProfileResponse
        {
            Id = customer.Id,
            UserId = customer.UserId,
            FullName = customer.FullName,
            Email = customer.User?.Email ?? string.Empty,
            PhoneNumber = customer.User?.PhoneNumber ?? string.Empty,
            Address = customer.Address,
            CreatedAt = customer.CreatedAt,
            IsActive = customer.User?.IsActive ?? false,
            VehicleCount = customer.Vehicles.Count,
            SaleCount = customer.Sales.Count,
            TotalSpent = totalSpent,
            LoyaltyPoints = CalculateLoyaltyPoints(totalSpent)
        };
    }

    public async Task UpdateProfileAsync(int customerId, UpdateCustomerRequest request)
    {
        var customer = await _customerRepository
            .FindByCondition(c => c.Id == customerId, trackChanges: true)
            .Include(c => c.User)
            .FirstOrDefaultAsync();

        if (customer == null)
            throw new NotFoundException($"Customer with ID {customerId} not found.");

        customer.FullName = request.FullName;
        customer.Address = request.Address;

        if (customer.User != null)
        {
            customer.User.FullName = request.FullName;
            customer.User.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(customer.User);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Failed to update customer user details: {errors}");
                throw new BadRequestException(errors);
            }
        }

        _customerRepository.Update(customer);
        await _customerRepository.SaveChangesAsync();

        _logger.LogInformation($"Customer {customerId} updated successfully.");
    }

    public async Task AddVehicleAsync(int customerId, AddVehicleRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new NotFoundException($"Customer with ID {customerId} not found.");

        var duplicate = await _vehicleRepository.GetByPlateNumberAsync(request.PlateNumber);
        if (duplicate != null)
            throw new ConflictException($"Vehicle with plate number {request.PlateNumber} already exists.");

        var vehicle = new Vehicle
        {
            CustomerId = customerId,
            PlateNumber = request.PlateNumber,
            Model = request.Model,
            Year = request.Year,
            Notes = request.Notes
        };

        _vehicleRepository.Create(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        _logger.LogInformation($"Vehicle {request.PlateNumber} added to customer {customerId}.");
    }

    public async Task<IEnumerable<CustomerVehicleResponse>> GetVehiclesAsync(int customerId)
    {
        var vehicles = await _vehicleRepository.GetByCustomerIdAsync(customerId);

        return vehicles.Select(v => new CustomerVehicleResponse
        {
            Id = v.Id,
            PlateNumber = v.PlateNumber,
            Model = v.Model,
            Year = v.Year,
            Notes = v.Notes,
            CreatedAt = v.CreatedAt
        });
    }

    public async Task<CustomerHistoryResponse> GetCustomerHistoryAsync(int customerId)
    {
        var profile = await GetProfileAsync(customerId);

        var vehicles = (await _vehicleRepository.GetByCustomerIdAsync(customerId))
            .Select(v => new CustomerVehicleResponse
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Model = v.Model,
                Year = v.Year,
                Notes = v.Notes,
                CreatedAt = v.CreatedAt
            })
            .ToList();

        var sales = (await _saleRepository.GetByCustomerIdAsync(customerId))
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new CustomerSaleResponse
            {
                Id = s.Id,
                TotalAmount = s.TotalAmount,
                DiscountAmount = s.DiscountAmount,
                FinalAmount = s.FinalAmount,
                CreatedAt = s.CreatedAt
            })
            .ToList();

        var appointments = await _appointmentRepository
            .FindByCondition(a => a.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new CustomerAppointmentResponse
            {
                Id = a.Id,
                ServiceType = a.ServiceType,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status.ToString(),
                Notes = a.Notes,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        var partRequests = await _partRequestRepository
            .FindByCondition(pr => pr.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(pr => pr.CreatedAt)
            .Select(pr => new CustomerPartRequestResponse
            {
                Id = pr.Id,
                PartName = pr.PartName,
                Urgency = pr.Urgency.ToString(),
                IsResolved = pr.IsResolved,
                CreatedAt = pr.CreatedAt
            })
            .ToListAsync();

        var reviews = await _reviewRepository
            .FindByCondition(r => r.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CustomerReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        var creditRecords = await _creditRecordRepository
            .FindByCondition(c => c.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CustomerCreditRecordResponse
            {
                Id = c.Id,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                PaidAt = c.PaidAt
            })
            .ToListAsync();

        var totalSpent = sales.Sum(s => s.FinalAmount);
        var loyaltyPoints = CalculateLoyaltyPoints(totalSpent);
        var creditBalance = creditRecords
            .Where(c => c.Status.Equals("Unpaid", StringComparison.OrdinalIgnoreCase))
            .Sum(c => c.Amount);

        var activityDates = new List<DateTime> { profile.CreatedAt };
        activityDates.AddRange(vehicles.Select(v => v.CreatedAt));
        activityDates.AddRange(sales.Select(s => s.CreatedAt));
        activityDates.AddRange(appointments.Select(a => a.CreatedAt));
        activityDates.AddRange(partRequests.Select(p => p.CreatedAt));
        activityDates.AddRange(reviews.Select(r => r.CreatedAt));
        activityDates.AddRange(creditRecords.Select(c => c.CreatedAt));

        var lastActivityDate = activityDates.Count > 0 ? activityDates.Max() : (DateTime?)null;

        return new CustomerHistoryResponse
        {
            Profile = profile,
            Vehicles = vehicles,
            Sales = sales,
            Appointments = appointments,
            PartRequests = partRequests,
            Reviews = reviews,
            CreditRecords = creditRecords,
            TotalSpent = totalSpent,
            LoyaltyPoints = loyaltyPoints,
            CreditBalance = creditBalance,
            LastActivityDate = lastActivityDate
        };
    }

    public async Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query)
    {
        // If they just hit search without typing anything, don't return the whole database
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<CustomerSearchResponse>();
        }

        _logger.LogInformation($"Searching customers with query: {query}");
        var customers = await _customerRepository.SearchCustomersAsync(query);

        // Package up the messy database models into clean, front-end friendly DTOs
        return customers.Select(c => new CustomerSearchResponse
        {
            Id = c.Id,
            FullName = c.User?.FullName ?? c.FullName,
            Email = c.User?.Email ?? string.Empty,
            Phone = c.User?.PhoneNumber ?? string.Empty,
            Address = c.Address,
            CreatedAt = c.CreatedAt,
            IsActive = c.User?.IsActive ?? false,
            
            // Automatically calculate how much money they owe us by summing up unpaid/overdue bills
            TotalCreditOwed = c.CreditRecords.Where(cr => cr.Status == PartsPro.Domain.Enums.InvoiceStatus.Unpaid || cr.Status == PartsPro.Domain.Enums.InvoiceStatus.Overdue).Sum(cr => cr.Amount),
            
            // Just send back the basic vehicle details so the UI isn't overloaded
            Vehicles = c.Vehicles.Select(v => new CustomerVehicleDto
            {
                PlateNumber = v.PlateNumber,
                Model = v.Model,
                Year = v.Year
            }).ToList()
        });
    }

    private static int CalculateLoyaltyPoints(decimal totalSpent)
        => (int)(totalSpent / 100m) * 10;
}