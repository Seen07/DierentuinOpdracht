using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using DierentuinOpdracht.Models.Enums;
using DierentuinOpdracht.Models.Results;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Services.Actions
{
    public class AnimalActionService
    {
        private const string FoodPlants = "Plants";
        private const string FoodMeat = "Meat";
        private const string FoodInsects = "Insects";
        private const string FoodFish = "Fish";
        private const string FoodMixed = "Mixed";

        private readonly ZooDbContext _context;

        public AnimalActionService(ZooDbContext context)
        {
            _context = context;
        }

        public async Task<AnimalSunResult> SunriseAsync(int animalId)
        {
            var animal = await GetAnimalAsync(animalId);
            return BuildSunResult(animal, isSunrise: true);
        }

        public async Task<AnimalSunResult> SunsetAsync(int animalId)
        {
            var animal = await GetAnimalAsync(animalId);
            return BuildSunResult(animal, isSunrise: false);
        }

        public async Task<AnimalFeedingResult> FeedingTimeAsync(int animalId)
        {
            var animal = await GetAnimalAsync(animalId);

            // Als het dier een prooi heeft en die zit in hetzelfde verblijf, dan gaat dat “boven” standaard voedsel.
            if (!string.IsNullOrWhiteSpace(animal.Prey) && animal.EnclosureId != null)
            {
                var preyInSameEnclosure = await _context.Animals
                    .Where(a => a.EnclosureId == animal.EnclosureId)
                    .Where(a => a.Id != animal.Id)
                    .FirstOrDefaultAsync(a =>
                        a.Name == animal.Prey ||
                        a.Species == animal.Prey);

                if (preyInSameEnclosure != null)
                {
                    return new AnimalFeedingResult
                    {
                        AnimalId = animal.Id,
                        AnimalName = animal.Name,
                        Food = $"{preyInSameEnclosure.Species} ({preyInSameEnclosure.Name})"
                    };
                }

                // Prooi opgegeven, maar niet aanwezig -> nog steeds tonen wat er staat
                if (animal.DietaryClass == DietaryClass.Carnivore)
                {
                    return new AnimalFeedingResult
                    {
                        AnimalId = animal.Id,
                        AnimalName = animal.Name,
                        Food = animal.Prey
                    };
                }
            }

            // Anders standaard op basis van DietaryClass
            return new AnimalFeedingResult
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name,
                Food = animal.DietaryClass switch
                {
                    DietaryClass.Herbivore => FoodPlants,
                    DietaryClass.Insectivore => FoodInsects,
                    DietaryClass.Piscivore => FoodFish,
                    DietaryClass.Omnivore => FoodMixed,
                    _ => FoodMeat
                }
            };
        }

        public async Task<AnimalConstraintsResult> CheckConstraintsAsync(int animalId)
        {
            var animal = await GetAnimalAsync(animalId);

            var result = new AnimalConstraintsResult
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name
            };

            // Constraint 1: Dier moet aan een verblijf gekoppeld zijn
            if (animal.EnclosureId == null)
            {
                result.NotSatisfied.Add("Animal is not assigned to an enclosure.");
                return result;
            }

            // Constraint 2: Security in enclosure moet voldoende zijn
            if (animal.Enclosure == null)
            {
                result.NotSatisfied.Add("Enclosure reference is missing.");
                return result;
            }

            if (animal.Enclosure.SecurityLevel >= animal.SecurityRequirement)
            {
                result.Satisfied.Add("Enclosure security level is sufficient.");
            }
            else
            {
                result.NotSatisfied.Add("Enclosure security level is NOT sufficient for this animal.");
            }

            // Constraint 3: Genoeg ruimte in verblijf (som van SpaceRequirement <= Enclosure.Size)
            var animalsInEnclosure = await _context.Animals
                .Where(a => a.EnclosureId == animal.EnclosureId)
                .ToListAsync();

            var totalRequiredSpace = animalsInEnclosure.Sum(a => a.SpaceRequirement);
            if (totalRequiredSpace <= animal.Enclosure.Size)
            {
                result.Satisfied.Add("Enclosure has enough total space for assigned animals.");
            }
            else
            {
                result.NotSatisfied.Add("Enclosure does NOT have enough total space for assigned animals.");
            }

            // Constraint 4: Prooi/predator conflict (als iemand jou als prooi heeft in hetzelfde verblijf)
            var predator = animalsInEnclosure.FirstOrDefault(a =>
                !string.IsNullOrWhiteSpace(a.Prey) &&
                (a.Prey == animal.Name || a.Prey == animal.Species));

            if (predator == null)
            {
                result.Satisfied.Add("No predator/prey conflict detected in this enclosure.");
            }
            else
            {
                result.NotSatisfied.Add($"Predator/prey conflict: {predator.Name} might eat this animal.");
            }

            return result;
        }

        private async Task<Animal> GetAnimalAsync(int animalId)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == animalId);

            if (animal == null)
            {
                throw new InvalidOperationException("Animal not found.");
            }

            return animal;
        }

        private static AnimalSunResult BuildSunResult(Animal animal, bool isSunrise)
        {
            // Diurnal: sunrise = wakker, sunset = slapen
            // Nocturnal: sunrise = slapen, sunset = wakker
            // Cathemeral: altijd actief
            var action = isSunrise ? "Sunrise" : "Sunset";

            if (animal.ActivityPattern == ActivityPattern.Cathemeral)
            {
                return new AnimalSunResult
                {
                    AnimalId = animal.Id,
                    AnimalName = animal.Name,
                    Action = action,
                    Result = "AlwaysActive"
                };
            }

            if (animal.ActivityPattern == ActivityPattern.Diurnal)
            {
                return new AnimalSunResult
                {
                    AnimalId = animal.Id,
                    AnimalName = animal.Name,
                    Action = action,
                    Result = isSunrise ? "WakesUp" : "GoesToSleep"
                };
            }

            // Nocturnal
            return new AnimalSunResult
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name,
                Action = action,
                Result = isSunrise ? "GoesToSleep" : "WakesUp"
            };
        }
    }
}
