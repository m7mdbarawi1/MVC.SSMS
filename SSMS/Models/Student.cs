using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    public int Gender { get; set; }

    public string FullNameArabic { get; set; } = null!;

    public string? FullNameEnglish { get; set; }

    public int? Age { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();
    [ValidateNever]
    public virtual User User { get; set; }
}
