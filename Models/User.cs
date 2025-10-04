using System;
using System.Collections.Generic;

namespace VisionManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

}
