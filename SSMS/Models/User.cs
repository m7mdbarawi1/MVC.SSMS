using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace SSMS.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserType { get; set; }

    [ValidateNever]
    public virtual Student? Student { get; set; }

    [ValidateNever]
    public virtual Teacher? Teacher { get; set; }
}
