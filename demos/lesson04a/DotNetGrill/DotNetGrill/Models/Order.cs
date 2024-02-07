using System.ComponentModel.DataAnnotations;

namespace DotNetGrill.Models
{
    // This is created after the user pays for the products
    public class Order
    {
        public int OrderId { get; set; }
        // A user will have multiple orders in our database
        public string CustomerId { get; set; }
        // Calculated by adding the result of multiplying (Qty * Price)
        // for all products in the cart
        public decimal Total { get; set; }
        public DateTime DateCreated {get;set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        // Display is for Label elements to show this text on the UI
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        // 1 to M relationship with OrderItems
        // 1 Order can contain One or More OrderItems
        // ? allows this field to be null
        public List<OrderItem>? OrderItems { get; set; }
    }
}
