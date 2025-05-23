using System;
using System.Collections.Generic;

namespace Shared.Models;

public class PasswordmanagerUserDTO
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public DateTime? Datelastlogin { get; set; }

    public DateTime? Datelastlogout { get; set; }

    public DateTime? Datecreated { get; set; }

    public DateTime? Dateretired { get; set; }

    public string UmsUserid { get; set; } = null!;

    public List<string> Roles = [];
}
