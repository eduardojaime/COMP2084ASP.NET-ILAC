namespace DotNetGrillWebUI.Models
{
    // naming convention
    // class name is singular
    // dbset name is plural (in dbcontext class)
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // TODO represent a 1 to M relationship from Categories to Products
        // 1 to M is represented as a list
        // ? means the list can be null
        public List<Product>? Products { get; set; } // M part 
    }
}
