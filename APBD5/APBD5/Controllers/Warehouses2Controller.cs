using CZW5.DTO;
using CZW5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD5.Controllers
{
    [Route("api/warehouses2")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IProductDBService _productDbService;

        public Warehouses2Controller(IProductDBService productDbService)
        {
            _productDbService = productDbService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductDTO product)
        {
            await _productDbService.PostProductProcedur(product);
            return Ok();
        }
    }
}
