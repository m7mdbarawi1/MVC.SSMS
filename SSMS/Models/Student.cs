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

    public int ClassId { get; set; }

    public int UserId { get; set; }

    [Range(0, 1, ErrorMessage = "Gender must be 0 (Female) or 1 (Male).")]
    public int Gender { get; set; }

    public string FullNameArabic { get; set; } = null!;

    public string? FullNameEnglish { get; set; }

    [Range(6, 18, ErrorMessage = "Age must be between 6 and 18.")]
    public int? Age { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();
    [ValidateNever]
    public virtual User User { get; set; }
}
