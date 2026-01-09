using DierentuinOpdracht.Models.Enums;

namespace DierentuinOpdracht.Models
{
    public class Animal
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Species { get; set; }

        public AnimalSize Size { get; set; }
        public DietaryClass DietaryClass { get; set; }
        public ActivityPattern ActivityPattern { get; set; }
        public SecurityLevel SecurityRequirement { get; set; }

        public double SpaceRequirement { get; set; }

        public string Prey { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int? EnclosureId { get; set; }
        public Enclosure Enclosure { get; set; }
    }
}
