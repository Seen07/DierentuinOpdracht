using System;
using Humanizer;
using static System.Net.Mime.MediaTypeNames;

namespace DierentuinOpdracht.Models.Enums
{

    // Geeft aan hoe groot een dier is.
    // Wordt gebruikt in Animal
    public enum AnimalSize
    {
        Microscopic,
        VerySmall,
        Small,
        Medium,
        Large,
        VeryLarge
    }
    //Geeft aan wat een dier eet.
    // Deze enum wordt gebruikt bij de FeedingTime actie.

    public enum DietaryClass
    {
        Carnivore,
        Herbivore,
        Omnivore,
        Insectivore,
        Piscivore
    }

    // Bepaalt wanneer een dier actief is.
    // Wordt gebruikt bij Sunrise en Sunset acties.
    public enum ActivityPattern
    {
        Diurnal,
        Nocturnal,
        Cathemeral
    }
    // Geeft het vereiste beveiligingsniveau aan.
    public enum SecurityLevel
    {
        Low,
        Medium,
        High
    }
    // Wordt gebruikt in: Enclosure
    //Geeft het klimaat van een verblijf aan.
    public enum Climate
    {
        Tropical,
        Temperate,
        Arctic
    }

    [Flags]
    //Geeft het type leefomgeving van een verblijf aan.
    
    public enum HabitatType
    {
        Forest,
        Aquatic,
        Desert,
        Grassland
    }
}