using Microsoft.AspNetCore.Mvc;
using CZW5.DTO;
using CZW5.Services;

namespace CZW5.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private readonly IProductDBService _productDbService;

        public WarehousesController(IProductDBService productDbService)
        {
            _productDbService = productDbService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductDTO product)
        {
            return Ok(await _productDbService.PostProduct(product));
        }
    }
}
