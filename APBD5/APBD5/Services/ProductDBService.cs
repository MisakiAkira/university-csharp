using CZW5.DTO;
using System.Data.SqlClient;
using System.Net;

namespace CZW5.Services
{
    public class ProductDBService : IProductDBService
    {
        private const string _connString = "";

        private async Task<bool> productExist(int productId)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"SELECT idproduct FROM product WHERE idproduct = {productId}", sqlConnection);

            await sqlConnection.OpenAsync();

            int lines = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();

            return lines > 0;
        }

        private async Task<bool> warehouseExist(int warehouseId)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"SELECT idwarehouse FROM warehouse WHERE idwarehouse = {warehouseId}", sqlConnection);

            await sqlConnection.OpenAsync();

            int lines = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();

            return lines > 0;
        }

        private async Task<bool> OrderExist(ProductDTO product)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"SELECT * FROM order WHERE idproduct = {product.IdProduct} AND amount = {product.Amount}", sqlConnection);

            await sqlConnection.OpenAsync();

            int lines = await sqlCommand.ExecuteNonQueryAsync();

            if (lines <= 0)
            {
                await sqlConnection.CloseAsync();
                return false;
            }

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (DateTime.Parse(reader["createdat"].ToString()) > product.CreatedAt)
                {
                    await sqlConnection.CloseAsync();
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> HaveBeenDone(int productId)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"SELECT * FROM product_warehouse WHERE idproduct = {productId}", sqlConnection);

            await sqlConnection.OpenAsync();

            int lines = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();

            return lines > 0;
        }

        private async Task UpdateOrder(ProductDTO product)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"UPDATE order SET fullfilledat = {DateTime.Now} WHERE idproduct = {product.IdProduct} AND amount = {product.Amount}", sqlConnection);

            await sqlConnection.OpenAsync();

            await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();
        }

        private async Task InsertIntoProductWarehouse(ProductDTO product)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new("INSERT INTO product_warehouse VALUSE ((SELECT MAX(idproductwarehosue) FROM product_warehouse) + 1" +
                $", {product.IdWarehouse}, {product.IdProduct}, (SELECT idorder FROM order WHERE idproduct = {product.IdProduct} AND amount = {product.Amount})," +
                $"(SELECT price FROM product WHERE idproduct = {product.IdProduct}) * {product.Amount}, {DateTime.Now})", sqlConnection);

            await sqlConnection.OpenAsync();

            await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();
        }

        public async Task<ResultDTO> PostProduct(ProductDTO product)
        {
            if (!await productExist(product.IdProduct))
            {
                return new ResultDTO
                {
                    Code = 404, 
                    Message = $"Product with this id {product.IdProduct} does not exist"
                };
            }
            if (!await warehouseExist(product.IdWarehouse))
            {
                return new ResultDTO
                {
                    Code = 404,
                    Message = $"Warehouse with id {product.IdWarehouse} does not exist"
                };
            }
            if (product.Amount <= 0)
            {
                return new ResultDTO
                {
                    Code = 400,
                    Message = $"Amount must be > than 0. Amount given: {product.Amount}"
                };
            }
            if(!await OrderExist(product))
            {
                return new ResultDTO
                {
                    Code = 404,
                    Message = "Order with this product and amount does not exist"
                };
            }
            if(await HaveBeenDone(product.IdProduct))
            {
                return new ResultDTO
                {
                    Code = 400,
                    Message = "Order already've been done"
                };
            }

            await UpdateOrder(product);
            await InsertIntoProductWarehouse(product);

            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new("SELECT MAX(idproductwarehosue) FROM product_warehosue", sqlConnection);

            await sqlConnection.OpenAsync();

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            ResultDTO result = new ResultDTO
            {
                Code = 200
            };

            while(await reader.ReadAsync())
            {
                result.Message = reader["idproductwarehosue"].ToString();
            }

            await sqlConnection.CloseAsync();

            return result;
        }

        public async Task PostProductProcedur(ProductDTO product)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new($"EXEC AddProductToWarehouse {product.IdProduct}, {product.IdWarehouse}, {product.Amount}, {DateTime.Now.ToString("s")}", sqlConnection);

            await sqlConnection.OpenAsync();

            await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();
        }
    }
}
