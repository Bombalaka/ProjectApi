using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Application.DTOs;
using ProductApi.Domain.Interfaces;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IReviewRepository _reviewRepo;

    public ProductsController(IProductService service, IReviewRepository reviewRepo)
    {
        _reviewRepo = reviewRepo;
        _service = service;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductSummaryDto>>> GetAll()
    {
        var products = await _service.GetAllAsync();

        var result = products.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(string id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        var created = await _service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Product product)
    {
        if (id != product.Id) return BadRequest();
        await _service.UpdateAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("review/{id}")]
    [ProducesResponseType(typeof(ProductForReviewDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductForReview(string id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();

        var dto = new ProductForReviewDto
        {
            Id = product.Id,
            Name = product.Name
        };

        return Ok(dto);
    }
    // This endpoint is for submitting a review for a product
    [HttpPost("{id}/reviews")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SubmitReview(string id, [FromBody] ReviewInputDto dto)
    {
        if (dto.Stars < 1 || dto.Stars > 5)
            return BadRequest("Stars must be between 1 and 5");

        var review = new Review
        {
            ProductId = id,
            Stars = dto.Stars,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepo.AddAsync(review);
        return Ok("✅ Review saved.");
    }

    // This endpoint is for getting all reviews for a product
    [HttpGet("{id}/reviews")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetReviews(string id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null)
            return NotFound("Product not found.");

        var reviews = await _reviewRepo.GetByProductIdAsync(id);

        var result = reviews.Select(r => new ReviewOutputDto
        {
            ProductId = r.ProductId ?? string.Empty,
            ProductName = product.Name, // injected from lookup
            Stars = r.Stars,
            Description = r.Description,
            CreatedAt = r.CreatedAt
        });

        return Ok(result);
    }
}
