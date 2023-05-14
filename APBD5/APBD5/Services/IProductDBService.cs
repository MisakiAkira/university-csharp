using CZW5.DTO;

namespace CZW5.Services
{
    public interface IProductDBService
    {
        Task<ResultDTO> PostProduct(ProductDTO product);
        Task PostProductProcedur(ProductDTO product);
    }
}
