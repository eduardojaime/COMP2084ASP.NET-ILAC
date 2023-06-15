using System.ComponentModel.DataAnnotations;

namespace DotNetGrill.Models
{
    // Naming convention: Use singulars for naming model classes
    // since they represent a single Product
    // Step 1) Add all attributes
    // Step 2) Add data annotations (for validation and checks)
    public class Product
    {
        // important to add { get; set; }, otherwise it gets ignored by EF
        public int ProductId { get; set; }
        [Required] // makes field required
        [MaxLength(255)] // limits number of characters in this field
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")] // format number as currency for the UI
        public decimal Price { get; set; }
        public int Rating { get; set; }
        // ? makes the field nullable, needed to allow it to be empty in the DB
        public string? Photo { get; set; }
        // category > connects to a categoryId from the other class
        // TODO
        // public string Category { get; set; } // just as a place holder until I create Category.cs
        // EF needs the following two fields to associate a Product with a Category
        public int CategoryId { get; set; } // the id
        public Category? Category { get; set; } // the actual category
    }
}
