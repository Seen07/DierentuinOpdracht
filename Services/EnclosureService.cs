using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Services
{
    public class EnclosureService
    {
        private readonly ZooDbContext _context;

        public EnclosureService(ZooDbContext context)
        {
            _context = context;
        }

        public List<Enclosure> GetAll()
        {
            return _context.Enclosures
                .Include(e => e.Animals)
                .Include(e => e.Zoo)
                .ToList();
        }

        public Enclosure GetById(int id)
        {
            return _context.Enclosures
                .Include(e => e.Animals)
                .Include(e => e.Zoo)
                .First(e => e.Id == id);
        }

        public void Add(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);
            _context.SaveChanges();
        }
    }
}
