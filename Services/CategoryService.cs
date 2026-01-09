using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Services
{
    public class CategoryService
    {
        private readonly ZooDbContext _context;

        public CategoryService(ZooDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAll()
        {
            return _context.Categories
                .Include(c => c.Animals)
                .ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories
                .Include(c => c.Animals)
                .First(c => c.Id == id);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
    }
}
