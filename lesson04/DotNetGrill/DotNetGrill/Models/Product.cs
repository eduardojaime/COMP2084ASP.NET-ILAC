namespace DotNetGrill.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string Photo { get; set; }
        // Navigation M to 1 > every product has only one category
        public int CategoryId { get; set; }
        // Connects the related entity as an attribute
        public Category Category { get; set; }
    }
}
