using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMS.Models;

public partial class Mark
{
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int MaterialId { get; set; }

    [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100.")]
    public decimal Marks { get; set; }

    [ValidateNever]
    public virtual Class Class { get; set; } = null!;

    [ValidateNever]
    public virtual Material Material { get; set; } = null!;

    [ValidateNever]
    public virtual Student Student { get; set; } = null!;
}
