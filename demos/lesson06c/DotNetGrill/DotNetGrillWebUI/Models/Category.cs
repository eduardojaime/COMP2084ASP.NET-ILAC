namespace DotNetGrillWebUI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // TODO map relationship to Product (1 to M)
        // the M part is represented as a list
        // ? means this attribute can be null
        public List<Product>? Products { get; set; }
    }
}
