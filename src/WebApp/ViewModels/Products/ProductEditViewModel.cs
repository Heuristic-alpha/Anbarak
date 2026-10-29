using System.ComponentModel.DataAnnotations;

namespace Anbarak.WebApp.ViewModels.Products
{
    public sealed class ProductEditViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
