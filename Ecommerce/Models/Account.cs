using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }
}
