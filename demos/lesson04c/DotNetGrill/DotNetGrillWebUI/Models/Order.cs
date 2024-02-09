using System.ComponentModel.DataAnnotations;

namespace DotNetGrillWebUI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime DateCreated {get;set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        // 1 to M relationship from Order to OrderItems
        // 1 part contains a list 
        public List<OrderItem>? OrderItems { get; set; }
    }
}
