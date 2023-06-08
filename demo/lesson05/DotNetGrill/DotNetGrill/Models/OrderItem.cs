namespace DotNetGrill.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        // EF needs these two fileds to link item to order
        // M to 1
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        // EF needs these two fields to link item to product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        // db fields
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
