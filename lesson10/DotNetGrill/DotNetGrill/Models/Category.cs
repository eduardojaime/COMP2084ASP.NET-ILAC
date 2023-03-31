using MessagePack;
using Microsoft.Build.Framework;

namespace DotNetGrill.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // Navigation 1 to M > 1 Category is linked to Many products
        public List<Product>? Products { get; set; }
    }
}
