using System;
using System.Collections.Generic;

namespace VisionManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string? OtpCode { get; set; }  // nullable OTP field
    public DateTime? OtpExpiration { get; set; }  // expiration time
    public bool IsOtpVerified { get; set; } = false;  // to check if user verified OTP


    public int RoleId { get; set; }


    public virtual Role Role { get; set; } = null!;

}
