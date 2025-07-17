using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassNameArabic { get; set; } = null!;

    public string? ClassNameEnglish { get; set; }

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
