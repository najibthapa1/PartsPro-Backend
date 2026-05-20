using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Customers;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;
using PartsPro.Domain.Enums;

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

        return customers.Select(MapCustomerProfile);
    }

    public async Task<CustomerProfileResponse> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository
            .FindByCondition(c => c.Id == customerId, trackChanges: false)
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.Sales)
            .FirstOrDefaultAsync();

        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");
        return MapCustomerProfile(customer);
    }

    public async Task UpdateProfileAsync(int customerId, UpdateCustomerRequest request)
    {
        var customer = await _customerRepository
            .FindByCondition(c => c.Id == customerId, trackChanges: true)
            .Include(c => c.User)
            .FirstOrDefaultAsync();

        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");

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
                _logger.LogWarning("Failed to update customer user details: {Errors}", errors);
                throw new BadRequestException(errors);
            }
        }

        _customerRepository.Update(customer);
        await _customerRepository.SaveChangesAsync();
    }

    public async Task AddVehicleAsync(int customerId, AddVehicleRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");

        var duplicate = await _vehicleRepository.GetByPlateNumberAsync(request.PlateNumber);
        if (duplicate != null) throw new ConflictException($"Vehicle with plate number {request.PlateNumber} already exists.");

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
    }

    public async Task<IEnumerable<CustomerVehicleResponse>> GetVehiclesAsync(int customerId)
    {
        var vehicles = await _vehicleRepository.GetByCustomerIdAsync(customerId);
        return vehicles.Select(MapVehicle);
    }

    public async Task<CustomerHistoryResponse> GetCustomerHistoryAsync(int customerId)
    {
        var profile = await GetProfileAsync(customerId);
        var vehicles = (await GetVehiclesAsync(customerId)).ToList();
        var sales = await GetSalesForHistoryAsync(customerId);
        var appointments = (await GetAppointmentsAsync(customerId)).ToList();
        var partRequests = (await GetPartRequestsAsync(customerId)).ToList();
        var reviews = (await GetReviewsAsync(customerId)).ToList();

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

        var activityDates = new List<DateTime> { profile.CreatedAt };
        activityDates.AddRange(vehicles.Select(v => v.CreatedAt));
        activityDates.AddRange(sales.Select(s => s.CreatedAt));
        activityDates.AddRange(appointments.Select(a => a.CreatedAt));
        activityDates.AddRange(partRequests.Select(p => p.CreatedAt));
        activityDates.AddRange(reviews.Select(r => r.CreatedAt));
        activityDates.AddRange(creditRecords.Select(c => c.CreatedAt));

        return new CustomerHistoryResponse
        {
            Profile = profile,
            Vehicles = vehicles,
            Sales = sales,
            Appointments = appointments,
            PartRequests = partRequests,
            Reviews = reviews,
            CreditRecords = creditRecords,
            TotalSpent = sales.Sum(s => s.FinalAmount),
            LoyaltyPoints = profile.LoyaltyPoints,
            CreditBalance = creditRecords
                .Where(c => c.Status.Equals("Unpaid", StringComparison.OrdinalIgnoreCase) || c.Status.Equals("Overdue", StringComparison.OrdinalIgnoreCase))
                .Sum(c => c.Amount),
            LastActivityDate = activityDates.Count > 0 ? activityDates.Max() : null
        };
    }

    public async Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<CustomerSearchResponse>();

        var customers = await _customerRepository.SearchCustomersAsync(query);
        return customers.Select(c => new CustomerSearchResponse
        {
            Id = c.Id,
            FullName = c.User?.FullName ?? c.FullName,
            Email = c.User?.Email ?? string.Empty,
            Phone = c.User?.PhoneNumber ?? string.Empty,
            Address = c.Address,
            CreatedAt = c.CreatedAt,
            IsActive = c.User?.IsActive ?? false,
            TotalCreditOwed = c.CreditRecords
                .Where(cr => cr.Status == InvoiceStatus.Unpaid || cr.Status == InvoiceStatus.Overdue)
                .Sum(cr => cr.Amount),
            Vehicles = c.Vehicles.Select(v => new CustomerVehicleDto
            {
                PlateNumber = v.PlateNumber,
                Model = v.Model,
                Year = v.Year
            }).ToList()
        });
    }

    public async Task<IEnumerable<CustomerAppointmentResponse>> GetAppointmentsAsync(int customerId)
    {
        var appointments = await _appointmentRepository
            .FindByCondition(a => a.CustomerId == customerId, trackChanges: false)
            .Include(a => a.Vehicle)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return appointments.Select(MapAppointment);
    }

    public async Task<CustomerAppointmentResponse> CreateAppointmentAsync(int customerId, CreateAppointmentRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");

        var vehicle = await _vehicleRepository
            .FindByCondition(v => v.Id == request.VehicleId && v.CustomerId == customerId, trackChanges: false)
            .FirstOrDefaultAsync();

        if (vehicle == null) throw new BadRequestException("Selected vehicle does not belong to this customer.");
        if (request.AppointmentDate <= DateTime.UtcNow) throw new BadRequestException("Appointment date must be in the future.");

        var appointment = new Appointment
        {
            CustomerId = customerId,
            VehicleId = request.VehicleId,
            ServiceType = request.ServiceType.Trim(),
            AppointmentDate = DateTime.SpecifyKind(request.AppointmentDate, DateTimeKind.Utc),
            Notes = request.Notes,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _appointmentRepository.Create(appointment);
        await _appointmentRepository.SaveChangesAsync();
        return (await GetAppointmentsAsync(customerId)).First(a => a.Id == appointment.Id);
    }

    public async Task<CustomerAppointmentResponse> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequest request)
    {
        var appointment = await _appointmentRepository
            .FindByCondition(a => a.Id == appointmentId, trackChanges: true)
            .FirstOrDefaultAsync();

        if (appointment == null) throw new NotFoundException($"Appointment with ID {appointmentId} not found.");
        if (!Enum.TryParse<AppointmentStatus>(request.Status, true, out var status))
            throw new BadRequestException("Invalid status. Use Pending, Confirmed, Completed, or Cancelled.");

        appointment.Status = status;
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();
        return (await GetAppointmentsAsync(appointment.CustomerId)).First(a => a.Id == appointment.Id);
    }

    public async Task DeleteAppointmentAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null) throw new NotFoundException($"Appointment with ID {appointmentId} not found.");
        _appointmentRepository.Delete(appointment);
        await _appointmentRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CustomerPartRequestResponse>> GetPartRequestsAsync(int customerId)
    {
        var requests = await _partRequestRepository
            .FindByCondition(pr => pr.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();

        return requests.Select(MapPartRequest);
    }

    public async Task<CustomerPartRequestResponse> CreatePartRequestAsync(int customerId, CreatePartRequestCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");

        if (!Enum.TryParse<PartRequestUrgency>(request.Urgency, true, out var urgency))
            throw new BadRequestException("Invalid urgency. Use Low, Medium, or High.");

        var partRequest = new PartRequest
        {
            CustomerId = customerId,
            PartName = request.PartName.Trim(),
            Description = request.Description.Trim(),
            Urgency = urgency,
            IsResolved = false,
            CreatedAt = DateTime.UtcNow
        };

        _partRequestRepository.Create(partRequest);
        await _partRequestRepository.SaveChangesAsync();
        return (await GetPartRequestsAsync(customerId)).First(pr => pr.Id == partRequest.Id);
    }

    public async Task<CustomerPartRequestResponse> UpdatePartRequestStatusAsync(int requestId, UpdatePartRequestStatusRequest request)
    {
        var partRequest = await _partRequestRepository
            .FindByCondition(pr => pr.Id == requestId, trackChanges: true)
            .FirstOrDefaultAsync();

        if (partRequest == null) throw new NotFoundException($"Part request with ID {requestId} not found.");

        partRequest.IsResolved = request.IsResolved;
        _partRequestRepository.Update(partRequest);
        await _partRequestRepository.SaveChangesAsync();
        return (await GetPartRequestsAsync(partRequest.CustomerId)).First(pr => pr.Id == partRequest.Id);
    }

    public async Task DeletePartRequestAsync(int requestId)
    {
        var partRequest = await _partRequestRepository.GetByIdAsync(requestId);
        if (partRequest == null) throw new NotFoundException($"Part request with ID {requestId} not found.");
        _partRequestRepository.Delete(partRequest);
        await _partRequestRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CustomerReviewResponse>> GetReviewsAsync(int customerId)
    {
        var reviews = await _reviewRepository
            .FindByCondition(r => r.CustomerId == customerId, trackChanges: false)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapReview);
    }

    public async Task<CustomerReviewResponse> CreateReviewAsync(int customerId, CreateReviewRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) throw new NotFoundException($"Customer with ID {customerId} not found.");

        var review = new Review
        {
            CustomerId = customerId,
            Rating = request.Rating,
            Comment = request.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _reviewRepository.Create(review);
        await _reviewRepository.SaveChangesAsync();
        return (await GetReviewsAsync(customerId)).First(r => r.Id == review.Id);
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null) throw new NotFoundException($"Review with ID {reviewId} not found.");
        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangesAsync();
    }

    public async Task<CustomerInsightSummaryResponse> GetCustomerInsightsAsync()
    {
        var customers = await _customerRepository
            .FindAll(trackChanges: false)
            .Include(c => c.User)
            .Include(c => c.Sales)
            .Include(c => c.CreditRecords)
            .ToListAsync();

        var customerRows = customers.Select(c => new CustomerInsightCustomerResponse
        {
            CustomerId = c.Id,
            FullName = c.User?.FullName ?? c.FullName,
            Email = c.User?.Email ?? string.Empty,
            PhoneNumber = c.User?.PhoneNumber ?? string.Empty,
            SaleCount = c.Sales.Count,
            TotalSpent = c.Sales.Sum(s => s.FinalAmount),
            LastPurchaseDate = c.Sales.OrderByDescending(s => s.CreatedAt).Select(s => (DateTime?)s.CreatedAt).FirstOrDefault()
        }).ToList();

        var regulars = customerRows.Where(c => c.SaleCount >= 3).OrderByDescending(c => c.SaleCount).ToList();
        var highSpenders = customerRows.Where(c => c.TotalSpent >= 5000).OrderByDescending(c => c.TotalSpent).ToList();

        var pendingCredits = customers
            .Select(c => new CustomerInsightCreditResponse
            {
                CustomerId = c.Id,
                FullName = c.User?.FullName ?? c.FullName,
                Email = c.User?.Email ?? string.Empty,
                PhoneNumber = c.User?.PhoneNumber ?? string.Empty,
                TotalCreditOwed = c.CreditRecords
                    .Where(cr => cr.Status == InvoiceStatus.Unpaid || cr.Status == InvoiceStatus.Overdue)
                    .Sum(cr => cr.Amount),
                OldestCreditDate = c.CreditRecords
                    .Where(cr => cr.Status == InvoiceStatus.Unpaid || cr.Status == InvoiceStatus.Overdue)
                    .OrderBy(cr => cr.CreatedAt)
                    .Select(cr => (DateTime?)cr.CreatedAt)
                    .FirstOrDefault()
            })
            .Where(c => c.TotalCreditOwed > 0)
            .OrderByDescending(c => c.TotalCreditOwed)
            .ToList();

        return new CustomerInsightSummaryResponse
        {
            RegularCustomers = regulars.Count,
            HighSpenders = highSpenders.Count,
            CustomersWithPendingCredits = pendingCredits.Count,
            TotalPendingCredit = pendingCredits.Sum(c => c.TotalCreditOwed),
            RegularCustomerList = regulars,
            HighSpenderList = highSpenders,
            PendingCreditList = pendingCredits
        };
    }

    private async Task<List<CustomerSaleResponse>> GetSalesForHistoryAsync(int customerId)
    {
        var sales = await _saleRepository
            .FindByCondition(s => s.CustomerId == customerId, trackChanges: false)
            .Include(s => s.Items)
            .ThenInclude(i => i.Part)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return sales.Select(s => new CustomerSaleResponse
        {
            Id = s.Id,
            TotalAmount = s.TotalAmount,
            DiscountAmount = s.DiscountAmount,
            FinalAmount = s.FinalAmount,
            LoyaltyDiscountApplied = s.DiscountAmount > 0,
            CreatedAt = s.CreatedAt,
            Items = s.Items.Select(i => new CustomerSaleItemResponse
            {
                PartId = i.PartId,
                PartName = i.Part?.Name ?? "Part",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()
        }).ToList();
    }

    private static CustomerProfileResponse MapCustomerProfile(Customer customer)
    {
        var totalSpent = customer.Sales.Sum(s => s.FinalAmount);
        return new CustomerProfileResponse
        {
            Id = customer.Id,
            UserId = customer.UserId,
            FullName = customer.User?.FullName ?? customer.FullName,
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

    private static CustomerVehicleResponse MapVehicle(Vehicle v) => new()
    {
        Id = v.Id,
        PlateNumber = v.PlateNumber,
        Model = v.Model,
        Year = v.Year,
        Notes = v.Notes,
        CreatedAt = v.CreatedAt
    };

    private static CustomerAppointmentResponse MapAppointment(Appointment a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        VehicleId = a.VehicleId,
        VehiclePlateNumber = a.Vehicle?.PlateNumber ?? string.Empty,
        VehicleModel = a.Vehicle?.Model ?? string.Empty,
        ServiceType = a.ServiceType,
        AppointmentDate = a.AppointmentDate,
        Status = a.Status.ToString(),
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };

    private static CustomerPartRequestResponse MapPartRequest(PartRequest pr) => new()
    {
        Id = pr.Id,
        CustomerId = pr.CustomerId,
        PartName = pr.PartName,
        Description = pr.Description,
        Urgency = pr.Urgency.ToString(),
        IsResolved = pr.IsResolved,
        CreatedAt = pr.CreatedAt
    };

    private static CustomerReviewResponse MapReview(Review r) => new()
    {
        Id = r.Id,
        CustomerId = r.CustomerId,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt
    };

    private static int CalculateLoyaltyPoints(decimal totalSpent) => (int)(totalSpent / 100m) * 10;
}
