namespace DotNetGrill.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // 1 to M relationship from Category to Products
        // this requires a list
        // ? allows this field to be null
        public List<Product>? Products { get; set; }
    }
}
