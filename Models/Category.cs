using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DierentuinOpdracht.Models
{
    public class Category
    {
        public int Id { get; set; }

     
        public string Name { get; set; }


        public List<Animal> Animals { get; set; } = new();
    }
}
