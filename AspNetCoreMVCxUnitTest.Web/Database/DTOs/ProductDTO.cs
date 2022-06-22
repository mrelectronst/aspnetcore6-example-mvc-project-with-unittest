using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVCxUnitTest.Web.Database.DTOs
{
    public class ProductDTO
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }
    }
}
