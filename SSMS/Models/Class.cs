using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models;

public partial class Class
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClassId { get; set; }

    [Required, StringLength(50)]
    public string ClassNameArabic { get; set; } = null!;

    [StringLength(50)]
    public string? ClassNameEnglish { get; set; }

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
