using System;
using System.Linq;
using System.Threading.Tasks;
using DierentuinOpdracht.Data;
using DierentuinOpdracht.Models;
using DierentuinOpdracht.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Controllers
{
    public class EnclosuresController : Controller
    {
        private readonly ZooDbContext _context;

        public EnclosuresController(ZooDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return NotFound("Enclosure niet gevonden");
            }

            var results = new System.Collections.Generic.List<string>();

            
            if (string.IsNullOrWhiteSpace(enclosure.Name))
                results.Add(" Name ontbreekt");
            else
                results.Add(" Name ingevuld");

            if (enclosure.Size <= 0)
                results.Add(" Size moet groter");
            else
                results.Add("Size is goed");

            
            var totalRequiredSpace = enclosure.Animals.Sum(a => a.SpaceRequirement);

            if (totalRequiredSpace > enclosure.Size)
                results.Add("Te weinig ruimte");
            else
                results.Add("Ruimte is voldoende");

            var highestRequiredSecurity = enclosure.Animals.Count == 0
                ? SecurityLevel.Low
                : enclosure.Animals.Max(a => a.SecurityRequirement);

            if (enclosure.SecurityLevel < highestRequiredSecurity)
                results.Add(" Security level te laag");
            else
                results.Add("Security level is voldoende.");

          
            results.Add($"er zijn {enclosure.Animals.Count} dieren");

            ViewBag.EnclosureName = enclosure.Name;
            ViewBag.Results = results;

            return View();
        }


        public async Task<IActionResult> FeedingTime(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return NotFound("Enclosure niet gevonden");
            }

            var results = enclosure.Animals
                .Select(a => new EnclosureFeedingRowViewModel
                {
                    AnimalName = a.Name,
                    Species = a.Species,
                    Food = GetFoodText(enclosure.Animals, a)
                })
                .ToList();

            var vm = new EnclosureFeedingTimeViewModel
            {
                EnclosureName = enclosure.Name,
                Rows = results
            };

            return View(vm);
        }

        private static string GetFoodText(System.Collections.Generic.List<Models.Animal> animals, Models.Animal animal)
        {
         
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

        public class EnclosureFeedingTimeViewModel
        {
            public string EnclosureName { get; set; } = string.Empty;
            public System.Collections.Generic.List<EnclosureFeedingRowViewModel> Rows { get; set; }
                = new System.Collections.Generic.List<EnclosureFeedingRowViewModel>();
        }

        public class EnclosureFeedingRowViewModel
        {
            public string AnimalName { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Food { get; set; } = string.Empty;
        }


        public async Task<IActionResult> Sunrise(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return NotFound("Enclosure niet gevonden");
            }

            var wakingUp = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Diurnal)
                .ToList();

            var goingToSleep = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Nocturnal)
                .ToList();

            var alwaysActive = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Cathemeral)
                .ToList();

            ViewBag.EnclosureName = enclosure.Name;
            ViewBag.ActionTitle = "Sunrise";

            ViewBag.WakingUp = wakingUp;
            ViewBag.GoingToSleep = goingToSleep;
            ViewBag.AlwaysActive = alwaysActive;

            return View("DayCycle");
        }

     
        public async Task<IActionResult> Sunset(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return NotFound("Enclosure niet gevonden");
            }

           
            var wakingUp = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Nocturnal)
                .ToList();

            var goingToSleep = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Diurnal)
                .ToList();

            var alwaysActive = enclosure.Animals
                .Where(a => a.ActivityPattern == ActivityPattern.Cathemeral)
                .ToList();

            ViewBag.EnclosureName = enclosure.Name;
            ViewBag.ActionTitle = "Sunset";

            ViewBag.WakingUp = wakingUp;
            ViewBag.GoingToSleep = goingToSleep;
            ViewBag.AlwaysActive = alwaysActive;

            return View("DayCycle");
        }


        // GET: Enclosures
public async Task<IActionResult> Index(
    string? search,
    Climate? climate,
    string? habitatType,
    SecurityLevel? securityLevel,
    int? zooId)
{
    var query = _context.Enclosures
        .Include(e => e.Zoo)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(e => e.Name.Contains(search));
    }

    if (climate.HasValue)
    {
        query = query.Where(e => e.Climate == climate.Value);
    }

            if (!string.IsNullOrWhiteSpace(habitatType))
            {
                if (Enum.TryParse<HabitatType>(habitatType, out var parsed))
                {
                    query = query.Where(e => e.HabitatType == parsed);
                }
            }
            if (securityLevel.HasValue)
    {
        query = query.Where(e => e.SecurityLevel == securityLevel.Value);
    }

    if (zooId.HasValue)
    {
        query = query.Where(e => e.ZooId == zooId.Value);
    }

    ViewBag.Zoos = new SelectList(_context.Zoos, "Id", "Name", zooId);

    ViewBag.Search = search;
    ViewBag.Climate = climate;
    ViewBag.HabitatType = habitatType;
    ViewBag.SecurityLevel = securityLevel;
            ViewBag.HabitatOptions = new SelectList(new[]
{
    new { Value = "", Text = "-- All --" },
    new { Value = "Forest", Text = "Forest" },
    new { Value = "Aquatic", Text = "Aquatic" },
    new { Value = "Desert", Text = "Desert" },
    new { Value = "Grassland", Text = "Grassland" }
}, "Value", "Text");


            return View(await query.ToListAsync());
}


        // GET: Enclosures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures
                .Include(e => e.Zoo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        // GET: Enclosures/Create
        public IActionResult Create()
        {
            FillDropDowns();
            return View();
        }

        // POST: Enclosures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Size,Climate,HabitatType,SecurityLevel,ZooId")] Enclosure enclosure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enclosure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            FillDropDowns(enclosure);
            return View(enclosure);
        }

        // GET: Enclosures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            FillDropDowns(enclosure);
            return View(enclosure);
        }

        // POST: Enclosures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Size,Climate,HabitatType,SecurityLevel,ZooId")] Enclosure enclosure)
        {
            if (id != enclosure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enclosure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnclosureExists(enclosure.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            FillDropDowns(enclosure);
            return View(enclosure);
        }

        // GET: Enclosures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures
                .Include(e => e.Zoo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        // POST: Enclosures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EnclosureExists(int id)
        {
            return _context.Enclosures.Any(e => e.Id == id);
        }

        private void FillDropDowns(Enclosure? enclosure = null)
        {
            // Zoo dropdown (toon Name)
            ViewData["ZooId"] = new SelectList(_context.Zoos, "Id", "Name", enclosure?.ZooId);

            // Enum dropdowns
            ViewData["Climate"] = new SelectList(
                Enum.GetValues(typeof(Climate)).Cast<Climate>().Select(x => new { Id = x, Name = x.ToString() }),
                "Id",
                "Name",
                enclosure?.Climate
            );

            ViewData["HabitatType"] = new SelectList(
                Enum.GetValues(typeof(HabitatType)).Cast<HabitatType>().Select(x => new { Id = x, Name = x.ToString() }),
                "Id",
                "Name",
                enclosure?.HabitatType
            );

            ViewData["SecurityLevel"] = new SelectList(
                Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>().Select(x => new { Id = x, Name = x.ToString() }),
                "Id",
                "Name",
                enclosure?.SecurityLevel
            );
        }
    }
} 