using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Models;

public partial class Teacher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeacherId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [Required, Range(0, 1, ErrorMessage = "Gender must be 0 (Female) or 1 (Male).")]
    public int Gender { get; set; }

    [Required, StringLength(50)]
    public string FullNameArabic { get; set; } = null!;

    [StringLength(50)]
    public string? FullNameEnglish { get; set; }

    [ValidateNever]
    public virtual Material Material { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; }
}
