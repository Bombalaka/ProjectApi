using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Interfaces;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("api/public/products")]
public class PublicProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IReviewRepository _reviewRepo;

    public PublicProductsController(IProductService service, IReviewRepository reviewRepo)
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
    public async Task<ActionResult<ProductSummaryDto>> GetById(string id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();

        return Ok(new ProductSummaryDto
        {
            Id = product.Id,
            Name = product.Name
        });
    }
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
