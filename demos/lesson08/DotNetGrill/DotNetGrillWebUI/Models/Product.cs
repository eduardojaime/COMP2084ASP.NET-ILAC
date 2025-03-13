using System.ComponentModel.DataAnnotations;

namespace DotNetGrillWebUI.Models
{
    // Class names are in singular form
    // This class represents a product entity in the DB (record in a table)
    public class Product
    {
        // Public fields represent columns in the table
        // Primary key, it will automatically be an identity column because it contains ID
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string? Photo { get; set; } // ? means it can be null
        // Navigation property
        // 1 to M relationship between Product and Category
        // A product can belong to only one category
        // Add a categoryId and a category property
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
