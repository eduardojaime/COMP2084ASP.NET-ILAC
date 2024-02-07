namespace DotNetGrill.Models
{
    // A cart represents a product that's added to the user's Shopping Cart
    // Temporary list of user selections
    // Carts are cleared once user either checks out or clears the cart
    public class Cart
    {
        public int CartId { get; set; }
        // it will be associated to the Customer by GUID or email address
        public string CustomerId { get; set; }
        // Links 1 to 1 to Products > 1 product is 1 cart item
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
