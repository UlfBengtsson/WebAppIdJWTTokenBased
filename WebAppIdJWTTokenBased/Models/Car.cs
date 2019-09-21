using System.ComponentModel.DataAnnotations;

namespace WebAppIdJWTTokenBased.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Brand { get; set; }
        [Display(Name = "Model")]
        public string ModelName { get; set; }
    }
}