using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string MaterialNameArabic { get; set; } = null!;

    public string? MaterialNameEnglish { get; set; }

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
