using System.Runtime.CompilerServices;

namespace APBDS24610.Models
{
    public class Animal
    {
        [assembly: InternalsVisibleTo("AnimalDbService")]
        public int IdAnimal { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Area { get; set; }
    }
}
