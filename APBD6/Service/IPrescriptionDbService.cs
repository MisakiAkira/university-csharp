using APBD6.Models.DTO;

namespace APBD6.Service
{
    public interface IPrescriptionDbService
    {
        Task<IList<PrescriptionDTO>> GetPrescriptionList(string? surname);
        Task<IList<ResultDTO>> AddMedsToPrescription(int idPrescription, List<MedicamentDTO> medicaments);
    }
}
