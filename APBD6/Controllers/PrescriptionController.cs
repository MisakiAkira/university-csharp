using APBD6.Models.DTO;
using APBD6.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APBD6.Controllers
{
    [Route("api/prescription")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionDbService _prescriptionDbService;

        public PrescriptionController(IPrescriptionDbService service)
        {
            _prescriptionDbService = service;
        }

        [HttpGet]
        public async Task<IList<PrescriptionDTO>> GetPrescriptions([FromQuery] string? surname)
        {
            return await _prescriptionDbService.GetPrescriptionList(surname);
        }

        [HttpPost]
        [Route("idPrescription")]
        public async Task<IList<ResultDTO>> AddMedsToPrescription([Required]int idPrescription, List<MedicamentDTO> medicaments)
        {
            return await _prescriptionDbService.AddMedsToPrescription(idPrescription, medicaments);
        }
    }
}
