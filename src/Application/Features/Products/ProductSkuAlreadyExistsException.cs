namespace Anbarak.Application.Features.Products;

public sealed class ProductSkuAlreadyExistsException : Exception
{
    public ProductSkuAlreadyExistsException(string sku) : base($"A product with SKU '{sku}' already exists.")
    {
    }
}
