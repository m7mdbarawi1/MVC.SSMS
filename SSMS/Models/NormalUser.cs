using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models
{
    public abstract class NormalUser
    {
        [Required, Range(0, 1, ErrorMessage = "Gender must be 0 (Female) or 1 (Male).")]
        public int Gender { get; set; }

        [Required, StringLength(50)]
        public string FullNameArabic { get; set; } = null!;

        [StringLength(50)]
        public string? FullNameEnglish { get; set; }

        [Required]
        public int UserId { get; set; }

        [ValidateNever]
        public virtual User User { get; set; } = null!;
    }
}
