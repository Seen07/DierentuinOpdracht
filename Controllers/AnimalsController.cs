using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using DierentuinOpdracht.Models.Enums;


namespace DierentuinOpdracht.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ZooDbContext _context;

        public AnimalsController(ZooDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Sunrise(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            string message;

            if (animal.ActivityPattern == ActivityPattern.Diurnal)
            {
                message = $"{animal.Name} wordt wakker bij zonsopgang.";
            }
            else if (animal.ActivityPattern == ActivityPattern.Nocturnal)
            {
                message = $"{animal.Name} gaat slapen bij zonsopgang.";
            }
            else
            {
                message = $"{animal.Name} is altijd actief.";
            }

            ViewBag.AnimalName = animal.Name;
            ViewBag.Message = message;

            return View();
        }

           
           

        
        [HttpGet]
        public async Task<IActionResult> Sunset(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal niet gevonden");
            }

            string message;

            switch (animal.ActivityPattern)
            {
                case ActivityPattern.Diurnal:
                    message = $"{animal.Name} gaat slapen bij zonsondergang.";
                    break;

                case ActivityPattern.Nocturnal:
                    message = $"{animal.Name} wordt wakker bij zonsondergang.";
                    break;

                default:
                    message = $"{animal.Name} is altijd actief.";
                    break;
            }

            ViewBag.Title = "Sunset";
            ViewBag.AnimalName = animal.Name;
            ViewBag.Message = message;

            return View("ActionResult");
        }


        // GET: Animals
        public async Task<IActionResult> Index()
        {
            var animals = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure);

            return View(await animals.ToListAsync());
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name");
            return View();
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Species,Size,DietaryClass,ActivityPattern,SecurityRequirement,SpaceRequirement,Prey,CategoryId,EnclosureId")]
            Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);
            return View(animal);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);
            return View(animal);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Species,Size,DietaryClass,ActivityPattern,SecurityRequirement,SpaceRequirement,Prey,CategoryId,EnclosureId")]
            Animal animal)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);
            return View(animal);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }


        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

    }

}
