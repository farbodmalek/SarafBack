using GirlyShopBackend.Application.Categories.Commands.CreateCategory;
using GirlyShopBackend.Application.Categories.Commands.DeleteCategory;
using GirlyShopBackend.Application.Categories.Queries.GetAllCategories;
using GirlyShopBackend.Core.Domain.ViewModel.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GirlyShopBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
    {
        var id = await _mediator.Send(new CreateCategoryCommand(request.Name, request.ParentCategoryId, request.ImageUrl));
        return CreatedAtAction(nameof(GetAll), new { id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }
}
