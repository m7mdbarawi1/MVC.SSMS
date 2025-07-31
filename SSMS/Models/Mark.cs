using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models;

public partial class Mark
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [Required, Range(0, 100, ErrorMessage = "Mark must be between 0 and 100.")]
    public decimal Marks { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    [ValidateNever]
    public virtual Material Material { get; set; } = null!;

    [ValidateNever]
    public virtual Student Student { get; set; } = null!;
}
