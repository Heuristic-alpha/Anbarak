namespace Anbarak.Contracts.DTOs.Products;

public sealed record CreateProductInput
    (
string Sku,
string Name,
string UnitOfMeasure,
string? Description
    );
