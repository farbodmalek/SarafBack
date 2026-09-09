using GirlyShopBackend.Application.Products.Commands.CreateProduct;
using GirlyShopBackend.Application.Products.Commands.DeleteProduct;
using GirlyShopBackend.Application.Products.Commands.UpdateProduct;
using GirlyShopBackend.Application.Products.Queries.GetAllProducts;
using GirlyShopBackend.Application.Products.Queries.GetProductBySlug;
using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GirlyShopBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/products
    // GET /api/products?category=lebas-majlesi
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(category));
        return Ok(result);
    }

    // GET /api/products/{slug}
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetProductBySlugQuery(slug));
        if (result is null) return NotFound();
        return Ok(result);
    }

    public record CreateProductRequest(
        string Title, string Description, decimal BasePrice, decimal? DiscountPrice,
        int CategoryId, List<ProductVariantInput> Variants);

    // POST /api/products (بعداً [Authorize(Roles = "Admin")] اضافه می‌شود)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var id = await _mediator.Send(new CreateProductCommand(
            request.Title, request.Description, request.BasePrice,
            request.DiscountPrice, request.CategoryId, request.Variants));

        return CreatedAtAction(nameof(GetBySlug), new { slug = id.ToString() }, new { id });
    }

    public record UpdateProductRequest(
        string Title, string Description, decimal BasePrice, decimal? DiscountPrice, int CategoryId);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var success = await _mediator.Send(new UpdateProductCommand(
            id, request.Title, request.Description, request.BasePrice,
            request.DiscountPrice, request.CategoryId));

        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }
}
