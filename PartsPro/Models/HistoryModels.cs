using System;

namespace PartsPro.Models
{
    public class PurchaseHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }

        public Customer Customer { get; set; }
    }

    public class ServiceHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public string ServiceType { get; set; }
        public DateTime ServiceDate { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }

        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}