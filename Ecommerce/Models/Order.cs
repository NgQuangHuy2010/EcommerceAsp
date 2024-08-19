using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? OrderId { get; set; }

    public decimal? Amount { get; set; }

    public string? OrderInfo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? PartnerCode { get; set; }

    public string? Status { get; set; }
}
