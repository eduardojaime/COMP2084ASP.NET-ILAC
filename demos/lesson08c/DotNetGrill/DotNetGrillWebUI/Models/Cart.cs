namespace DotNetGrillWebUI.Models
{
    // Cart is a temporary list of products that a user intends to buy
    // Once user pays, and order is created, cart records are deleted for this user
    public class Cart
    {
        public int CartId { get; set; }
        // it will be associated to the Customer by GUID or email address
        public string CustomerId { get; set; }
        // Links 1 to 1 to Products > 1 product is 1 cart item
        public int ProductId { get; set; }
        // ? means this attribute
        // is allowed to hold null values
        public Product? Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
