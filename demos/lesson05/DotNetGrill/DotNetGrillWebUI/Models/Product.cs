// Add data annotations for validation and display
using System.ComponentModel.DataAnnotations; // Require, Range, MaxLength, MinLength, RegularExpression

namespace DotNetGrillWebUI.Models
{
    // Model class for Product
    // Class will become a table in the database
    public class Product
    {
        // Attributes will become columns in the table
        // All attributes must be public and have a getter and setter
        // Primary key attribute
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name must be 255 characters or less")]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")] // show number as currency $ 99.99
        public decimal Price { get; set; }
        public int Rating { get; set; }
        // To make a field optional, use the question mark ? after the data type
        public string? Photo { get; set; } // ? allows NULL values in the db table
        // TODO: Add foreign key attribute to Category
        // M to 1 > Every product has only one Category
        public int CategoryId { get; set; } // Foreign Key, it will be added in the DB table as a column
        // Navigation Property (only works in EF and will not be added in the DB)
        public Category Category { get; set; }
    }
}
