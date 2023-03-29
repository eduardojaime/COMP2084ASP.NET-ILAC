using System.ComponentModel.DataAnnotations;

namespace DotNetGrill.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime DateCreated {get;set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
