using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class Event : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [MinLength(3, ErrorMessage = "Името трябва да съдържа минимум 3 символа.")]
        [MaxLength(50, ErrorMessage = "Името трябва да бъде до 50 символа.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string Name { get; set; }

        [MaxLength(60, ErrorMessage = "Името на мястото трябва да бъде до 60 символа.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string Place { get; set; }

        [MaxLength(300, ErrorMessage = "Описанието трябва да бъде до 300 символа.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Това поле е задължително.")]
        [Range(1, 1000, ErrorMessage = "Цената трябва да бъде в диапазона 1 до 1000 лв.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Това поле е задължително.")]
        public DateTime DateAndTime { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateAndTime < DateTime.Now)
            {
                yield return new ValidationResult("Датата на събитието не може да бъде минала.", new[] { "DateAndTime" });
            }
        }
    }
}
