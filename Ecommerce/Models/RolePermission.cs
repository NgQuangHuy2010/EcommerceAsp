using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class RolePermission
{
    public int Id { get; set; }

    public string? PermissionsId { get; set; }

    public string? RoleId { get; set; }

    public virtual AspNetRole? Permissions { get; set; }

    public virtual NameRole? Role { get; set; }
}
