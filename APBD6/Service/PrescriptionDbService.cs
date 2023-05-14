using APBD6.Models.DTO;
using Microsoft.Data.SqlClient;

namespace APBD6.Service
{
    public class PrescriptionDbService : IPrescriptionDbService
    {
        private const string _connString = "";

        public async Task<IList<ResultDTO>> AddMedsToPrescription(int idPrescription, List<MedicamentDTO> medicaments)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            List<ResultDTO> results = new();

            foreach (var item in medicaments)
            {
                sqlCommand.Parameters.Clear();
                sqlCommand.CommandText = $"if exists(select * from medicament where idmedicament = {item.IdMedicament})\r\n\t" +
                    $"if exists(select * from prescription where idprescription = {idPrescription})\r\n\t\t" +
                    $"if not exists(select * from Prescription_Medicament where idmedicament = {item.IdMedicament} and idprescription = {idPrescription})\r\n\t\t\t" +
                    $"insert into prescription_medicament values ({item.IdMedicament}, {idPrescription}, {item.Dose}, @details)";
                sqlCommand.Parameters.AddWithValue("@details", item.Details);

                int lines = await sqlCommand.ExecuteNonQueryAsync();

                ResultDTO result = new();

                if (lines > 0)
                {
                    result.Code = 200;
                    result.Message = "Complete succesfully";
                }
                else
                {
                    result.Code = 400;
                    result.Message = "Something went wrong";
                }

                results.Add(result);
            }

            await sqlConnection.CloseAsync();

            return results;
        }

        public async Task<IList<PrescriptionDTO>> GetPrescriptionList(string? surname)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            string sql;
            if (string.IsNullOrWhiteSpace(surname))
            {
                sql = "SELECT * FROM Prescription";
            }
            else
            {
                sql = "SELECT * FROM Prescription JOIN patient ON patient.idpatient = prescription.idpatient WHERE lastname LIKE @surname";
                sqlCommand.Parameters.AddWithValue("@surname", surname);
            }
            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            List<PrescriptionDTO> result = new();

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                PrescriptionDTO dto = new()
                {
                    IdPrescription = int.Parse(reader["IdPrescription"].ToString()),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    DueDate = DateTime.Parse(reader["DueDate"].ToString())
                };
                result.Add(dto);
            }

            await sqlConnection.CloseAsync();

            return result;
        }
    }
}
