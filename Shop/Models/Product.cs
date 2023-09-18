using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
	public class Product 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		[Range(0.01, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
		public decimal Price { get; set; }
        public string ImageName {  get; set; }
        public bool isAvailable { get; set; }
        [NotMapped]
        public IFormFile file { get; set; }
        [NotMapped]
        public int Count { get; set; }

	}
}
