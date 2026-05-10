using System;
using System.Collections.Generic;

namespace PartsPro.DTOs.CustomerDTOs
{
    public class PurchaseHistoryDto
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class ServiceHistoryDto
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string ServiceType { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public string ServiceStatus { get; set; }
        public string MechanicName { get; set; }
    }

    public class AppointmenHistoryDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class PartRequestHistoryDto
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public DateTime? FulfilledDate { get; set; }
    }

    public class CustomerHistoryDto
    {
        public ProfileDto Profile { get; set; }
        public List<VehicleDto> Vehicles { get; set; }
        public List<PurchaseHistoryDto> PurchaseHistory { get; set; }
        public List<ServiceHistoryDto> ServiceHistory { get; set; }
        public List<AppointmenHistoryDto> AppointmentHistory { get; set; }
        public List<PartRequestHistoryDto> PartRequestHistory { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalPurchaseCount { get; set; }
        public int TotalServiceCount { get; set; }
        public int LoyaltyPoints { get; set; }
        public decimal CreditBalance { get; set; }
        public DateTime LastActivityDate { get; set; }
    }

    public class CustomerSummaryDto
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public decimal AverageCustomerSpend { get; set; }
        public List<TopCustomerDto> TopCustomers { get; set; }
    }

    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalPurchases { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }
}