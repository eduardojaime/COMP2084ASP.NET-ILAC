namespace DotNetGrill.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // EF navigation field
        // Add this so that Category can reference back to a list of Products associated to it
        // It will not be added to the table in the DB but only used by EF in the app
        // Make Nullable because we might have 1 category that doesn't have products yet
        public List<Product>? Products { get; set; }
    }
}
