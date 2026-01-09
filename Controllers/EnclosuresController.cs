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

        // GET: Enclosures
        public async Task<IActionResult> Index()
        {
            var enclosures = _context.Enclosures.Include(e => e.Zoo);
            return View(await enclosures.ToListAsync());
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