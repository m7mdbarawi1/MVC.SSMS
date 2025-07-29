using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int UserId { get; set; }

    public int MaterialId { get; set; }

    [Range(0, 1, ErrorMessage = "Gender must be 0 (Female) or 1 (Male).")]
    public int Gender { get; set; }

    public string FullNameArabic { get; set; } = null!;
    //Hello world
    public string? FullNameEnglish { get; set; }

    [ValidateNever]
    public virtual Material Material { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; }
}
