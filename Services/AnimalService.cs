using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Services
{
    public class AnimalService
    {
        private readonly ZooDbContext _context;

        public AnimalService(ZooDbContext context)
        {
            _context = context;
        }

        public List<Animal> GetAll()
        {
            return _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .ToList();
        }

        public Animal GetById(int id)
        {
            return _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .First(a => a.Id == id);
        }

        public void Add(Animal animal)
        {
            _context.Animals.Add(animal);
            _context.SaveChanges();
        }

        public void Update(Animal animal)
        {
            _context.Animals.Update(animal);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var animal = GetById(id);
            _context.Animals.Remove(animal);
            _context.SaveChanges();
        }
    }
}
