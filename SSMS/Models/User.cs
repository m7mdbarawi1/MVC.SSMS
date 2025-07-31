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

    [Required, StringLength(50)]
    public string FullName { get; set; } = null!;

    [Required, StringLength(50)]
    public string UserName { get; set; } = null!;

    [Required, StringLength(50)]
    public string Password { get; set; } = null!;

    [Required, Range(1, 3, ErrorMessage = "UserType must be 1 (Student), 2 (Teacher), or 3 (Admin).")]
    public int UserType { get; set; }

    [ValidateNever]
    public virtual Student? Student { get; set; }

    [ValidateNever]
    public virtual Teacher? Teacher { get; set; }






    // Not mapped property for UI display
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string UserTypeDisplay
    {
        get
        {
            return UserType switch
            {
                1 => "Student",
                2 => "Teacher",
                3 => "Admin",
                _ => "Unknown"
            };
        }
    }

}
