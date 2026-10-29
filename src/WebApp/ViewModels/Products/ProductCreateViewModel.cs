using System.ComponentModel.DataAnnotations;

namespace Anbarak.WebApp.ViewModels.Products;

public sealed class ProductCreateViewModel
{
    [Required]
    [StringLength(64)]
    [RegularExpression(@"^\S+$", ErrorMessage = "SKU cannot contain whitespace.")]
    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    [Display(Name = "Unit of measure")]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}
