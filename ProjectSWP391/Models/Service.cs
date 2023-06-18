using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ProjectSWP391.Models
{
    public partial class Service
    {
        public Service()
        {
            Bookings = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
        }

        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Service Name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Service Name must be between 1 and 30 characters")]
        [Trim(ErrorMessage = "Service Name cannot start or end with a space")]
        [NoConsecutiveSpaces(ErrorMessage = "Service Name cannot contain more than 2 consecutive spaces")]
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public int? ScategoryId { get; set; }

        public virtual ServiceCategory? Scategory { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        public class TrimAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is string str && (str.StartsWith(" ") || str.EndsWith(" ")))
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }

        public class NoConsecutiveSpacesAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is string str && Regex.IsMatch(str, @"\s{3,}"))
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }
    }
}
