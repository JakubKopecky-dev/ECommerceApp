using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;



        [HttpGet]
        public async Task<IReadOnlyList<BrandDto>> GetAllBrands() => await _brandService.GetAllBrandsAsync();




        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetBrand(Guid brandId)
        {
            BrandDto? brand = await _brandService.GetBrandByIdAsync(brandId);

            return brand is not null ? Ok(brand) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateUpdateBrandDto createDto)
        {
            BrandDto brand = await _brandService.CreateBrandAsync(createDto);

            return CreatedAtAction(nameof(GetBrand), new { brandId = brand.Id }, brand);
        }



        [HttpPut("{brandId}")]
        public async Task<IActionResult> UpdateBrand(Guid brandId,[FromBody] CreateUpdateBrandDto updateDto)
        {
            BrandDto? brand = await _brandService.UpdateBrandAsync(brandId, updateDto);

            return brand is not null ? Ok(brand) : NotFound();
        }



        [HttpDelete("{brandId}")]
        public async Task<IActionResult> DeleteBrand(Guid brandId)
        {
            BrandDto? brand = await _brandService.DeleteBrandAsync(brandId);

            return brand is not null ? Ok(brand) : NotFound();
        }



    }
}
