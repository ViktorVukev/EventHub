using System.ComponentModel.DataAnnotations;

namespace EventHub.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MinLength(3, ErrorMessage = "Името трябва да съдържа минимум 3 символа.")]
        [MaxLength(30, ErrorMessage = "Името трябва да бъде до 30 символа.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "Описанието трябва да бъде до 200 символа.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string Description { get; set; }

    }
}
