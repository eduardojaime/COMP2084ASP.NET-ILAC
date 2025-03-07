namespace DotNetGrillWebUI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // Navigation property
        // 1 to M relationship between Product and Category
        // A category can have many products
        // ? means it can be null (when first created)
        public List<Product>? Products { get; set; }
    }
}
