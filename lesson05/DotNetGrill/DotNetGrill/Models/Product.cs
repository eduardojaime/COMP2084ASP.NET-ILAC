using System.ComponentModel.DataAnnotations;

namespace DotNetGrill.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]  // MS Currency format
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string Photo { get; set; }
        // Navigation M to 1 > every product has only one category
        public int CategoryId { get; set; }
        // Connects the related entity as an attribute
        public Category? Category { get; set; }
    }
}
