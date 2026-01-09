using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Services
{
    public class ZooService
    {
        private readonly ZooDbContext _context;

        public ZooService(ZooDbContext context)
        {
            _context = context;
        }

        public List<Zoo> GetAll()
        {
            return _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .ToList();
        }

        public Zoo GetById(int id)
        {
            return _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .First(z => z.Id == id);
        }
    }
}
