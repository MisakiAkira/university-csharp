using APBDS24610.Models;
using System.Data.SqlClient;

namespace APBDS24610.Services
{
    public class AnimalDbService : IAnimalDbService
    {
        private const string _connString = "";
        private const string _localConnString = "Data Source=localhost;Initial Catalog=jd;User ID={USERNAME};Password={PASSWORD}";


        public async Task<IList<Animal>> GetAnimalsListAsync(string orderBy)
        {
            List<Animal> animals = new();

            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            string sql;
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                sql = "SELECT * FROM Animal ORDER BY Name ASC";
            }
            else
            {
                sql = $"SELECT * FROM Animal ORDER BY {orderBy} ASC";
            }

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            await using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            while (await sqlDataReader.ReadAsync())
            {
                Animal animal = new()
                {
                    IdAnimal = int.Parse(sqlDataReader["IdAnimal"].ToString()),
                    Name = sqlDataReader["Name"].ToString(),
                    Description = sqlDataReader["Description"].ToString(),
                    Category = sqlDataReader["Category"].ToString(),
                    Area = sqlDataReader["Area"].ToString()
                };
                animals.Add(animal);
            }

            await sqlConnection.CloseAsync();

            return animals;
        }

        public async Task<bool> PostAnimals(Animal animal)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new(
                $"INSERT INTO Animal VALUES('{animal.Name}', '{animal.Description}', '{animal.Category}', '{animal.Area}')"
                ,sqlConnection);

            await sqlConnection.OpenAsync();

            int rows = await sqlCommand.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> PutAnimals(int idAnimal, Animal animal)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new(
                $"UPDATE Animal SET Name = '{animal.Name}', Description = '{animal.Description}', Category = '{animal.Category}', Area = '{animal.Area}' WHERE IdAnimal = {idAnimal}"
                , sqlConnection);

            await sqlConnection.OpenAsync();

            int rows = await sqlCommand.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteAnimals(int idAnimal)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new(
                $"DELETE FROM Animal WHERE IdAnimal = {idAnimal}"
                , sqlConnection);

            await sqlConnection.OpenAsync();

            int rows = await sqlCommand.ExecuteNonQueryAsync();

            return rows > 0;
        }
    }
}

