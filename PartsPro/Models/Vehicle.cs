using System.ComponentModel.DataAnnotations;

namespace PartsPro.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VehicleNumber { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        public string Color { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}