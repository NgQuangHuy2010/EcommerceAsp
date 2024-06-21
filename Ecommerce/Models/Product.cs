namespace Ecommerce.Models;

public partial class Product
{
    public int Id { get; set; }

    public int? IdCategory { get; set; }

    public string? NameProduct { get; set; }

    public string? ImageProduct { get; set; }

    public decimal? PriceProduct { get; set; }

    public decimal? Discount { get; set; }

    public string? Description { get; set; }

    public string? StockQuantity { get; set; }

    public int? Status { get; set; }

    public string? Model { get; set; }

    public string? Producer { get; set; }

    public string? Origin { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }
}
