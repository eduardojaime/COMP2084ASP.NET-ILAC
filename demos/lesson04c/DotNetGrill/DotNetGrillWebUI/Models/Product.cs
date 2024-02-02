using System.ComponentModel.DataAnnotations;

namespace DotNetGrillWebUI.Models
{
    // model class names are singular
    // this class represents a Product entity in the DB
    public class Product
    {
        // attributes represent columns in the corresponding table in the DB
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")] // number as currency e.g. '$ 5.00'
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string? Photo { get; set; } // how to store images in a DB? BEST practice is to store only the path to it
        // TODO map relationship to Category (1 to 1)
        // 1 to 1 is represented with two attributes
        // ForeignKey and Object
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
