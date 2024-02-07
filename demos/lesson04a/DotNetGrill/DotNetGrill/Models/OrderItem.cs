namespace DotNetGrill.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        // Connects M to 1 with Order 
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        // Connects 1 to 1 to Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        // Calculated price > (product price * quantity)
        public decimal Price { get; set; }
    }
}
