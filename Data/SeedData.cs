using DierentuinOpdracht.Models;
using DierentuinOpdracht.Models.Enums;

namespace DierentuinOpdracht.Data
{
    public static class SeedData
    {
        public static void Initialize(ZooDbContext context)
        {
            context.Database.EnsureCreated();

            // Als er al data is, seed niet opnieuw (voorkomt dubbele dropdown items)
            if (context.Categories.Any() || context.Enclosures.Any() || context.Animals.Any() || context.Zoos.Any())
            {
                return;
            }

            var mammals = new Category { Name = "Mammals" };
            var birds = new Category { Name = "Birds" };

            var zoo = new Zoo { Name = "City Zoo" };

            var savanna = new Enclosure
            {
                Name = "Savanna Enclosure",
                Size = 5000,
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Grassland,
                SecurityLevel = SecurityLevel.Medium,
                Zoo = zoo
            };

            var arctic = new Enclosure
            {
                Name = "Arctic Enclosure",
                Size = 3000,
                Climate = Climate.Arctic,
                HabitatType = HabitatType.Desert,
                SecurityLevel = SecurityLevel.High,
                Zoo = zoo
            };

            var animals = new List<Animal>
            {
                new Animal
                {
                    Name = "Leo",
                    Species = "Lion",
                    Size = AnimalSize.Large,
                    DietaryClass = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Diurnal,
                    SecurityRequirement = SecurityLevel.High,
                    SpaceRequirement = 500,
                    Prey = "Zebra",
                    Category = mammals,
                    Enclosure = savanna
                },
                new Animal
                {
                    Name = "Zara",
                    Species = "Zebra",
                    Size = AnimalSize.Medium,
                    DietaryClass = DietaryClass.Herbivore,
                    ActivityPattern = ActivityPattern.Diurnal,
                    SecurityRequirement = SecurityLevel.Low,
                    SpaceRequirement = 300,
                    Category = mammals,
                    Enclosure = savanna
                },
                new Animal
                {
                    Name = "Pingu",
                    Species = "Penguin",
                    Size = AnimalSize.Small,
                    DietaryClass = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Diurnal,
                    SecurityRequirement = SecurityLevel.Low,
                    SpaceRequirement = 50,
                    Prey = "Fish",
                    Category = birds,
                    Enclosure = arctic
                }
            };

            context.Categories.AddRange(mammals, birds);
            context.Zoos.Add(zoo);
            context.Enclosures.AddRange(savanna, arctic);
            context.Animals.AddRange(animals);

            context.SaveChanges();
        }
    }
}
