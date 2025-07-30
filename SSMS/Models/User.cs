using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Models;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    [Range(1, 3, ErrorMessage = "UserType must be 1 (Student), 2 (Teacher), or 3 (Admin).")]
    public int UserType { get; set; }

    [ValidateNever]
    public virtual Student? Student { get; set; }

    [ValidateNever]
    public virtual Teacher? Teacher { get; set; }
}
