using System.ComponentModel.DataAnnotations;

namespace DotNetGrill.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } // email address
        public decimal Total { get; set; }
        public DateTime DateCreated { get; set; }
        [Display(Name = "First Name")] // label shown in UI
        public string FirstName { get; set; }
        [Display(Name = "Last Name")] // label shown in UI
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        [Display(Name = "Postal Code")] // label shown in UI
        public string PostalCode { get; set; }
        // EF needs this field to link Order to OrderItems
        // 1 to M
        public List<OrderItem>? OrderItems { get; set; }
    }
}
