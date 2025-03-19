using System.ComponentModel.DataAnnotations;

namespace DotNetGrillWebUI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        // it will be associated to the Customer by GUID or email address
        public string CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime DateCreated { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
