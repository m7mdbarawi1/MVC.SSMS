using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models;

public partial class Material
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaterialId { get; set; }

    public string MaterialNameArabic { get; set; } = null!;

    public string? MaterialNameEnglish { get; set; }

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual Teacher? Teacher { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
