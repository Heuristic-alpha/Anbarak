using Anbarak.Domain.Common;
using Anbarak.Domain.Enums;

namespace Anbarak.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public const int SkuMaxLength = 64;
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 1000;

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Product()
    {
    }

    public Product(string sku, string name, UnitOfMeasure unitOfMeasure, string? description = null)
    {
        Sku = NormalizeSku(sku);
        Name = ValidateRequiredText(name, nameof(name), NameMaxLength);

        if (!Enum.IsDefined(unitOfMeasure))
        {
            throw new ArgumentOutOfRangeException(nameof(unitOfMeasure), unitOfMeasure, "Invalid unit of measure.");
        }

        UnitOfMeasure = unitOfMeasure;
        Description = NormalizeOptionalText(description);
        IsActive = true;
    }

    public void UpdateDetails(string name, string? description)
    {
        Name = ValidateRequiredText(name, nameof(name), NameMaxLength);

        Description = NormalizeOptionalText(description);
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private static string NormalizeSku(string sku)
    {
        var normalized = ValidateRequiredText(sku, nameof(sku), SkuMaxLength);

        if (normalized.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("SKU cannot contain whitespace.",nameof(sku));
        }

        return normalized.ToUpperInvariant();
    }

    private static string ValidateRequiredText(string? value,string parameterName,int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.",parameterName);
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"The value cannot exceed {maxLength} characters.",parameterName);
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))return null;

        var normalized = value.Trim();

        if (normalized.Length > DescriptionMaxLength)
        {
            throw new ArgumentException($"Description cannot exceed {DescriptionMaxLength} characters.",nameof(value));
        }
        return normalized;
    }
}
