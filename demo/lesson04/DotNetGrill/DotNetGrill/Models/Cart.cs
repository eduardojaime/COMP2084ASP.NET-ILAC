namespace DotNetGrill.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string CustomerId {  get; set; } // customer email address
        // EF needs the following fields to link this to a Product
        public int ProductId { get; set; }
        public Product Product { get; set; }
        // The following are fields that will be added to the table
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreate { get; set; }
        
    }
}
