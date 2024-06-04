namespace DotNetGrillWebUI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // TODO: Relationship to Product
        // 1 to M > 1 Category can be associated to Many Products
        // e.g. Breakfast can be associated to Scrambled Eggs, Pancakes, etc...
        // Navigation Property (only works within EF, not added to the DB table)
        public List<Product> Products { get; set; }
    }
}
