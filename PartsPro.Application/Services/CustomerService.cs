using Microsoft.EntityFrameworkCore;
using PartsPro.Domain.Entities;
using PartsPro.DTOs;
using PartsPro.DTOs.CustomerDTOs;
using PartsPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartsPro.Services
{
    public interface ICustomerService
    {
        bool Register(RegisterDto registerDto);
        ProfileDto GetProfile(int customerId);
        bool UpdateProfile(int customerId, UpdateProfileDto updateDto);
        CustomerHistoryDto GetCustomerHistory(int customerId);
        bool AddVehicle(int customerId, VehicleDto vehicleDto);
        List<VehicleDto> GetCustomerVehicles(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // F12: Customer Self-Register
        public bool Register(RegisterDto registerDto)
        {
            try
            {
                // Check if email already exists
                if (_context.Customers.Any(c => c.Email == registerDto.Email))
                    throw new Exception("Email already registered");

                var customer = new Customer
                {
                    FullName = registerDto.FullName,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Address = registerDto.Address,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    RegistrationDate = DateTime.UtcNow,
                    IsActive = true,
                    TotalSpent = 0
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }

        // F12: Get Customer Profile
        public ProfileDto GetProfile(int customerId)
        {
            var customer = _context.Customers
                .Include(c => c.PurchaseHistories)
                .Include(c => c.ServiceHistories)
                .FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            return new ProfileDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                TotalSpent = customer.TotalSpent,
                TotalPurchases = customer.PurchaseHistories?.Count ?? 0,
                TotalServices = customer.ServiceHistories?.Count ?? 0
            };
        }

        // F12: Update Customer Profile
        public bool UpdateProfile(int customerId, UpdateProfileDto updateDto)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            customer.FullName = updateDto.FullName;
            customer.PhoneNumber = updateDto.PhoneNumber;
            customer.Address = updateDto.Address;

            _context.SaveChanges();
            return true;
        }

        // F14: Get Complete Customer History
        public CustomerHistoryDto GetCustomerHistory(int customerId)
        {
            var customer = _context.Customers
                .Include(c => c.Vehicles)
                .Include(c => c.PurchaseHistories)
                .Include(c => c.ServiceHistories)
                .FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            // Calculate loyalty points (10 points per 100 spent)
            int loyaltyPoints = (int)(customer.TotalSpent / 100) * 10;

            return new CustomerHistoryDto
            {
                Profile = GetProfile(customerId),
                Vehicles = customer.Vehicles?.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year,
                    Color = v.Color
                }).ToList() ?? new List<VehicleDto>(),

                PurchaseHistory = customer.PurchaseHistories?
                    .OrderByDescending(p => p.PurchaseDate)
                    .Select(p => new PurchaseHistoryDto
                    {
                        Id = p.Id,
                        PartName = p.PartName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        TotalPrice = p.TotalPrice,
                        PurchaseDate = p.PurchaseDate
                    }).ToList() ?? new List<PurchaseHistoryDto>(),

                ServiceHistory = customer.ServiceHistories?
                    .OrderByDescending(s => s.ServiceDate)
                    .Select(s => new ServiceHistoryDto
                    {
                        Id = s.Id,
                        VehicleNumber = s.Vehicle.VehicleNumber,
                        ServiceType = s.ServiceType,
                        ServiceDate = s.ServiceDate,
                        Cost = s.Cost,
                        Description = s.Description
                    }).ToList() ?? new List<ServiceHistoryDto>(),

                TotalSpent = customer.TotalSpent,
                LoyaltyPoints = loyaltyPoints
            };
        }

        public bool AddVehicle(int customerId, VehicleDto vehicleDto)
        {
            var vehicle = new Vehicle
            {
                VehicleNumber = vehicleDto.VehicleNumber,
                Make = vehicleDto.Make,
                Model = vehicleDto.Model,
                Year = vehicleDto.Year,
                Color = vehicleDto.Color,
                CustomerId = customerId
            };

            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
            return true;
        }

        public List<VehicleDto> GetCustomerVehicles(int customerId)
        {
            return _context.Vehicles
                .Where(v => v.CustomerId == customerId)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year,
                    Color = v.Color
                }).ToList();
        }
    }
}