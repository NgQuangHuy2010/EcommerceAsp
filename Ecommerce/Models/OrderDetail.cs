using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public string? OrderId { get; set; }

    public decimal? Amount { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? DetailedAddress { get; set; }

    public string? Products { get; set; }

    public string? PartnerCode { get; set; }

    public string? Address { get; set; }
}
