using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Models;

public partial class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StudentId { get; set; }

    [Required]
    public int ClassId { get; set; }
    [Required]
    public int UserId { get; set; }

    [Required, Range(0, 1, ErrorMessage = "Gender must be 0 (Female) or 1 (Male).")]
    public int Gender { get; set; }

    [Required, StringLength(50)]
    public string FullNameArabic { get; set; } = null!;

    [StringLength(50)]
    public string? FullNameEnglish { get; set; }

    [Required, Range(6, 18, ErrorMessage = "Age must be between 6 and 18.")]
    public int? Age { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();
    [ValidateNever]
    public virtual User User { get; set; }
}
