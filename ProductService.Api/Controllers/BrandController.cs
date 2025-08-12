using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Enum;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;



        [HttpGet]
        public async Task<IReadOnlyList<BrandDto>> GetAllBrands(CancellationToken ct) => await _brandService.GetAllBrandsAsync(ct);




        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetBrand(Guid brandId, CancellationToken ct)
        {
            BrandDto? brand = await _brandService.GetBrandByIdAsync(brandId, ct);

            return brand is not null ? Ok(brand) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateUpdateBrandDto createDto, CancellationToken ct)
        {
            BrandDto brand = await _brandService.CreateBrandAsync(createDto, ct);

            return CreatedAtAction(nameof(GetBrand), new { brandId = brand.Id }, brand);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{brandId}")]
        public async Task<IActionResult> UpdateBrand(Guid brandId, [FromBody] CreateUpdateBrandDto updateDto, CancellationToken ct)
        {
            BrandDto? brand = await _brandService.UpdateBrandAsync(brandId, updateDto, ct);

            return brand is not null ? Ok(brand) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{brandId}")]
        public async Task<IActionResult> DeleteBrand(Guid brandId, CancellationToken ct)
        {
            BrandDto? brand = await _brandService.DeleteBrandAsync(brandId, ct);

            return brand is not null ? Ok(brand) : NotFound();
        }



    }
}
