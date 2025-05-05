using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("api/public/products")]
public class PublicProductsController : ControllerBase
{
    private readonly IProductService _service;

    public PublicProductsController(IProductService service)
    {
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
}
