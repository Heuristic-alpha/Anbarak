using Anbarak.Application.Features.Products;
using Anbarak.Contracts.DTOs.Products;
using Anbarak.WebApp.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Anbarak.WebApp.Controllers;

[Authorize(Roles = $"{IdentityRoles.Administrator},{IdentityRoles.WarehouseManager}")]
public sealed class ProductsController(ProductService productService) : Controller
{
    private static readonly string[] UnitOfMeasures =
    [
        "Each",
        "Kilogram",
        "Gram",
        "Liter",
        "Milliliter",
        "Meter",
        "Centimeter",
        "Kilometer",
        "Box",
        "Pallet"
    ];

    public async Task<IActionResult> Index(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllAsync(includeInactive, cancellationToken);

        ViewBag.IncludeInactive = includeInactive;

        return View(products);
    }

    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken = default)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);

        return product is null ? NotFound() : View(product);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateUnitOfMeasures();

        return View(new ProductCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            PopulateUnitOfMeasures(model.UnitOfMeasure);
            return View(model);
        }

        try
        {
            var input = new CreateProductInput(
                model.Sku,
                model.Name,
                model.UnitOfMeasure,
                model.Description);

            var created = await productService.CreateAsync(input, cancellationToken);

            TempData["SuccessMessage"] = $"Product \'{created.Name}\' created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ProductSkuAlreadyExistsException)
        {
            ModelState.AddModelError(nameof(model.Sku), $"A product with this SKU \'{model.Sku}\' already exists.");
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        PopulateUnitOfMeasures(model.UnitOfMeasure);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken = default)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        ViewBag.ProductSku = product.Sku;
        ViewBag.UnitOfMeasure = product.UnitOfMeasure;

        var model = new ProductEditViewModel
        {
            Name = product.Name,
            Description = product.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, ProductEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        // Fetch persisted display details, not client-supplied values.
        var existing = await productService.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return NotFound();
        }

        ViewBag.ProductSku = existing.Sku;
        ViewBag.UnitOfMeasure = existing.UnitOfMeasure;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var input = new UpdateProductInput(model.Name, model.Description);

            var updated = await productService.UpdateAsync(id, input, cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = $"Product \'{updated.Name}\' updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken = default)
    {
        var succeeded = await productService.DeactivateAsync(id, cancellationToken);

        if (!succeeded)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Product deactivated successfully.";

        return RedirectToAction(nameof(Index), new { includeInactive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken = default)
    {
        var succeeded = await productService.ActivateAsync(id, cancellationToken);

        if (!succeeded)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Product activated successfully.";

        return RedirectToAction(nameof(Index), new { includeInactive = true });
    }

    private void PopulateUnitOfMeasures(string? selected = null)
    {
        ViewBag.UnitOfMeasures = new SelectList(UnitOfMeasures, selected);
    }
}
