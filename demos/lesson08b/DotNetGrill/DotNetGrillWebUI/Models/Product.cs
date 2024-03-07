using System.ComponentModel.DataAnnotations;
namespace DotNetGrillWebUI.Models
{
    // this class represents a Product entity in the DB
    public class Product
    {
        // each public attribute will be converted to a column in the corresponding db table
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")] // represents number as currency with $ symbol
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string? Photo {  get; set; }
        // TODO represent a 1 to 1 relationship from Products to Categories
        // 1 to 1 is represented with two attributes
        // ID and Instance
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
