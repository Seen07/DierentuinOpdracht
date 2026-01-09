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
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("Animal niet gevonden");
            }

            var results = new List<string>();

        
            if (string.IsNullOrWhiteSpace(animal.Name))
            {
                results.Add("Name ontbreekt.");
            }
            else
            {
                results.Add("Name is ingevuld.");
            }

            if (string.IsNullOrWhiteSpace(animal.Species))
            {
                results.Add(" Species ontbreekt");
            }
            else
            {
                results.Add("Species is ingevuld.");
            }

            if (animal.SpaceRequirement <= 0)
            {
                results.Add(" SpaceRequirement voldoet niet");
            }
            else
            {
                results.Add(" SpaceRequirement is goed");
            }

            
            if (animal.CategoryId == null)
            {
                results.Add(" Category is leeg");
            }
            else
            {
                results.Add("Category is gekoppeld.");
            }

            if (animal.Enclosure == null)
            {
                results.Add(" Enclosure is niet ingevuld");
            }
            else
            {
                
                if (animal.SpaceRequirement > animal.Enclosure.Size)
                {
                    results.Add(" Te weinig ruimte");
                }
                else
                {
                    results.Add(" Genoeg ruimte in het verblijf.");
                }

            
                if (animal.SecurityRequirement > animal.Enclosure.SecurityLevel)
                {
                    results.Add("Security mismatch");
                }
                else
                {
                    results.Add(" Security level voldoende");
                }
            }

            ViewBag.AnimalName = animal.Name;
            ViewBag.Results = results;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("Animal niet gevonden");
            }

            var message = await GetFeedingMessageAsync(animal);

            ViewBag.AnimalName = animal.Name;
            ViewBag.Message = message;

            return View();
        }

        private async Task<string> GetFeedingMessageAsync(Models.Animal animal)
        {
            
            if (animal.EnclosureId.HasValue && animal.DietaryClass == DietaryClass.Carnivore)
            {
                var otherAnimals = await _context.Animals
                    .Where(a => a.EnclosureId == animal.EnclosureId && a.Id != animal.Id)
                    .ToListAsync();

              
                var target = otherAnimals.FirstOrDefault();
                if (target != null)
                {
                    return $"{animal.Name} eet {target.Name} (uit hetzelfde verblijf).";
                }
            }

          
            if (!string.IsNullOrWhiteSpace(animal.Prey))
            {
                return $"{animal.Name} eet {animal.Prey}.";
            }

           
            return animal.DietaryClass switch
            {
                DietaryClass.Herbivore => $"{animal.Name} eet planten.",
                DietaryClass.Omnivore => $"{animal.Name} eet planten en vlees.",
                DietaryClass.Carnivore => $"{animal.Name} eet vlees.",
                DietaryClass.Insectivore => $"{animal.Name} eet insecten.",
                DietaryClass.Piscivore => $"{animal.Name} eet vis.",
                _ => $"{animal.Name} eet onbekend voedsel."
            };
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
        public async Task<IActionResult> Index(
            string? search,
            AnimalSize? size,
            DietaryClass? dietaryClass,
            ActivityPattern? activityPattern,
            SecurityLevel? securityRequirement,
            int? categoryId,
            int? enclosureId)
        {
            var query = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Name.Contains(search) ||
                    a.Species.Contains(search));
            }

            if (size.HasValue)
            {
                query = query.Where(a => a.Size == size.Value);
            }

            if (dietaryClass.HasValue)
            {
                query = query.Where(a => a.DietaryClass == dietaryClass.Value);
            }

            if (activityPattern.HasValue)
            {
                query = query.Where(a => a.ActivityPattern == activityPattern.Value);
            }

            if (securityRequirement.HasValue)
            {
                query = query.Where(a => a.SecurityRequirement == securityRequirement.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            if (enclosureId.HasValue)
            {
                query = query.Where(a => a.EnclosureId == enclosureId.Value);
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);
            ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name", enclosureId);

            ViewBag.Search = search;
            ViewBag.Size = size;
            ViewBag.DietaryClass = dietaryClass;
            ViewBag.ActivityPattern = activityPattern;
            ViewBag.SecurityRequirement = securityRequirement;

            var animals = await query.ToListAsync();
            return View(animals);
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
