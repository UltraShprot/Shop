using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class CartProduct
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
}
