using System.ComponentModel.DataAnnotations;
namespace DotNetGrill.Models
{
    // this class will represent a product in the DB
    public class Product
    {
        // what information is relevant about this entity in my system?
        // these fields/attributes will become columns in my Products table
        // these must be public for EF to transform them into columns
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }
        public int Rating {  get; set; }
        // ? means photo can be null
        public string? Photo {  get; set; }
        // Category FK
        // 1 to 1 relationship from Product to Cat
        // this requires 2 fields: id and instance
        public int CategoryId { get; set; } 
        public Category? Category { get; set; }

    }
}
