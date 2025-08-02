using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Models;

public partial class Student : NormalUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StudentId { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required, Range(6, 18, ErrorMessage = "Age must be between 6 and 18.")]
    public int? Age { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

}
