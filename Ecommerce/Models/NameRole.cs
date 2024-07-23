using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class NameRole
{
    public string Id { get; set; } = null!;

    public string? NameRole1 { get; set; }

    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
