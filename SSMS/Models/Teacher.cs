using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int? UserId { get; set; }

    public int MaterialId { get; set; }

    public int Gender { get; set; }

    public string FullNameArabic { get; set; } = null!;

    public string? FullNameEnglish { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual User? User { get; set; }
}
