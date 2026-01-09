using System.Collections.Generic;

namespace DierentuinOpdracht.Models.Results
{
    public class AnimalConstraintsResult
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;

        public List<string> Satisfied { get; set; } = new();
        public List<string> NotSatisfied { get; set; } = new();
    }
}
