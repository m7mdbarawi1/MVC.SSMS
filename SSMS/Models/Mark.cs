using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class Mark
{
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int MaterialId { get; set; }

    public decimal Mark1 { get; set; }
    [ValidateNever]
    public virtual Class Class { get; set; } = null!;
    [ValidateNever]
    public virtual Material Material { get; set; } = null!;
    [ValidateNever]
    public virtual Student Student { get; set; } = null!;
}
