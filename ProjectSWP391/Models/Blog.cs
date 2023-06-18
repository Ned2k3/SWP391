using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectSWP391.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(70, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 50 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(5000, MinimumLength = 100, ErrorMessage = "Content must be between 100 and 5000 characters.")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Blog date is required!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? BlogDate { get; set; }


        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }
    }
}
