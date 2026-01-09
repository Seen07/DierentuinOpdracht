using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DierentuinOpdracht.Models.Enums;

namespace DierentuinOpdracht.Models
{
    public class Enclosure
    {
        public int Id { get; set; }

  
        public string Name { get; set; }


        public double Size { get; set; }

        public Climate Climate { get; set; }
        public HabitatType HabitatType { get; set; }
        public SecurityLevel SecurityLevel { get; set; }        

        public List<Animal> Animals { get; set; } = new();

        public int? ZooId { get; set; }
        public Zoo Zoo { get; set; }
    }
}
