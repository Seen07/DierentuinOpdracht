using System;
using System.Collections.Generic;
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
    public class ZoosController : Controller
    {
        private readonly ZooDbContext _context;

        public ZoosController(ZooDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CheckConstraints(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                    .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound("Zoo niet gevonden");
            }

            var results = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(zoo.Name))
                results.Add("Zoo name ontbreekt");
            else
                results.Add(" name ingevuld");

            var enclosureCount = zoo.Enclosures.Count;
            results.Add($"Aantal verblijven: {enclosureCount}");

            var totalAnimals = zoo.Enclosures.Sum(e => e.Animals.Count);
            results.Add($"Aantal dieren: {totalAnimals}");


            foreach (var enclosure in zoo.Enclosures)
            {
                var totalRequiredSpace = enclosure.Animals.Sum(a => a.SpaceRequirement);

                if (totalRequiredSpace > enclosure.Size)
                {
                    results.Add(" Verblijf  is niet voldoende");
                }
                else
                {
                    results.Add("Verblijf  is voldoende");
                }

                var highestRequiredSecurity = enclosure.Animals.Count == 0
                    ? SecurityLevel.Low
                    : enclosure.Animals.Max(a => a.SecurityRequirement);

                if (enclosure.SecurityLevel < highestRequiredSecurity)
                {
                    results.Add("Verblijf is niet voldoenede");
                }
                else
                {
                    results.Add(" Verblijf security is voldoende");
                }
            }

            ViewBag.ZooName = zoo.Name;
            ViewBag.Results = results;

            return View();
        }

            public async Task<IActionResult> Sunrise(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                    .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound("Zoo niet gevonden");
            }

            var wakingUp = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Diurnal)
                .ToList();

            var goingToSleep = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Nocturnal)
                .ToList();

            var alwaysActive = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Cathemeral)
                .ToList();

            ViewBag.ZooName = zoo.Name;
            ViewBag.ActionTitle = "Sunrise";
            ViewBag.WakingUp = wakingUp;
            ViewBag.GoingToSleep = goingToSleep;
            ViewBag.AlwaysActive = alwaysActive;

            return View("DayCycle");
        }

    
        public async Task<IActionResult> Sunset(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                    .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound("Zoo niet gevonden");
            }

           
            var wakingUp = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Nocturnal)
                .ToList();

            var goingToSleep = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Diurnal)
                .ToList();

            var alwaysActive = zoo.Enclosures
                .SelectMany(e => e.Animals)
                .Where(a => a.ActivityPattern == ActivityPattern.Cathemeral)
                .ToList();

            ViewBag.ZooName = zoo.Name;
            ViewBag.ActionTitle = "Sunset";
            ViewBag.WakingUp = wakingUp;
            ViewBag.GoingToSleep = goingToSleep;
            ViewBag.AlwaysActive = alwaysActive;

            return View("DayCycle");
        }


        // GET: Zoos/FeedingTime/5
        public async Task<IActionResult> FeedingTime(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                    .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound("Zoo niet gevonden");
            }

            var rows = zoo.Enclosures
                .SelectMany(e => e.Animals.Select(a => new ZooFeedingRowViewModel
                {
                    EnclosureName = e.Name,
                    AnimalName = a.Name,
                    Species = a.Species,
                    Food = GetFoodText(e.Animals, a)
                }))
                .ToList();

            var vm = new ZooFeedingTimeViewModel
            {
                ZooName = zoo.Name,
                Rows = rows
            };

            return View(vm);
        }

        private static string GetFoodText(System.Collections.Generic.List<DierentuinOpdracht.Models.Animal> animals, DierentuinOpdracht.Models.Animal animal)
        {
            // Eten van andere dieren gaat boven gegeven eten
            if (animal.DietaryClass == DietaryClass.Carnivore)
            {
                var prey = animals.FirstOrDefault(a => a.Id != animal.Id);
                if (prey != null)
                {
                    return prey.Name;
                }

                return "meat";
            }

            if (!string.IsNullOrWhiteSpace(animal.Prey))
            {
                return animal.Prey;
            }

            return animal.DietaryClass switch
            {
                DietaryClass.Herbivore => "plants",
                DietaryClass.Omnivore => "plants and meat",
                DietaryClass.Insectivore => "insects",
                DietaryClass.Piscivore => "fish",
                _ => "unknown"
            };
        }

        public class ZooFeedingTimeViewModel
        {
            public string ZooName { get; set; } = string.Empty;
            public System.Collections.Generic.List<ZooFeedingRowViewModel> Rows { get; set; }
                = new System.Collections.Generic.List<ZooFeedingRowViewModel>();
        }

        public class ZooFeedingRowViewModel
        {
            public string EnclosureName { get; set; } = string.Empty;
            public string AnimalName { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Food { get; set; } = string.Empty;
        }




        // GET: Zoos
        public async Task<IActionResult> Index(string? search, int? minEnclosures)
        {
            var query = _context.Zoos
                .Include(z => z.Enclosures)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(z => z.Name.Contains(search));
            }

            if (minEnclosures.HasValue)
            {
                query = query.Where(z => z.Enclosures.Count >= minEnclosures.Value);
            }

            ViewBag.Search = search;
            ViewBag.MinEnclosures = minEnclosures;

            var zoos = await query.ToListAsync();
            return View(zoos);
        }


        // GET: Zoos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // GET: Zoos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zoos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Zoo zoo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zoo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zoo);
        }

        // GET: Zoos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo == null)
            {
                return NotFound();
            }
            return View(zoo);
        }

        // POST: Zoos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Zoo zoo)
        {
            if (id != zoo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zoo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZooExists(zoo.Id))
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
            return View(zoo);
        }

        // GET: Zoos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // POST: Zoos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo != null)
            {
                _context.Zoos.Remove(zoo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZooExists(int id)
        {
            return _context.Zoos.Any(e => e.Id == id);
        }
    }
}
