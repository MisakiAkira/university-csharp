using APBDS24610.Services;
using Microsoft.AspNetCore.Mvc;
using APBDS24610.Models;

namespace APBDS24610.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalDbService _animalDbService;

        public AnimalsController(IAnimalDbService animalDbService)
        {
            _animalDbService = animalDbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnimals([FromQuery] string? orderBy)
        {
            IList<Animal> animals = await _animalDbService.GetAnimalsListAsync(orderBy);
            return Ok(animals);
        }

        [HttpPost]
        public async Task<IActionResult> PostAnimals(Animal animal)
        {
            return await _animalDbService.PostAnimals(animal) ? Ok() : BadRequest();
        }

        [HttpPut("{idAnimal}")]
        public async Task<IActionResult> PutAnimals(int idAnimal, Animal animal)
        {
            return await _animalDbService.PutAnimals(idAnimal, animal) ? Ok() : BadRequest();
        }

        [HttpDelete("{idAnimal}")]
        public async Task<IActionResult> DeleteAnimals(int idAnimal)
        {
            return await _animalDbService.DeleteAnimals(idAnimal) ? Ok() : BadRequest();
        }
    }
}
