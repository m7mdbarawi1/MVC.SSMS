using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Models;

public partial class Teacher : NormalUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeacherId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [ValidateNever]
    public virtual Material Material { get; set; } = null!;
}
