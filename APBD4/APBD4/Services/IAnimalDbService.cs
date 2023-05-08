using APBDS24610.Models;

namespace APBDS24610.Services
{
    public interface IAnimalDbService
    {
        Task<IList<Animal>> GetAnimalsListAsync(string title);

        Task<bool> PostAnimals(Animal animal);

        Task<bool> PutAnimals(int idAnimal, Animal animal);

        Task<bool> DeleteAnimals(int idAnimal);
    }
}
