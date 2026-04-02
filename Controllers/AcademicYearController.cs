using AdmissionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdmissionManagement.Controllers
{
    public class AcademicYearController : Controller
    {
 
        private readonly AdmitProDbContext _context;

        public AcademicYearController(AdmitProDbContext context)
        {
            _context = context;
        }

        // GET: AcademicYear
        public async Task<IActionResult> Index()
        {
            return View(await _context.AcademicYears.ToListAsync());
        }

        // GET: AcademicYear/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var academicYear = await _context.AcademicYears
                .FirstOrDefaultAsync(m => m.Id == id);

            if (academicYear == null) return NotFound();

            return View(academicYear);
        }

        // GET: AcademicYear/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicYear/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicYear academicYear)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        // GET: AcademicYear/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null) return NotFound();

            return View(academicYear);
        }

        // POST: AcademicYear/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AcademicYear academicYear)
        {
            if (id != academicYear.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicYear);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AcademicYears.Any(e => e.Id == academicYear.Id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        // GET: AcademicYear/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var academicYear = await _context.AcademicYears
                .FirstOrDefaultAsync(m => m.Id == id);

            if (academicYear == null) return NotFound();

            return View(academicYear);
        }

        // POST: AcademicYear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear != null)
            {
                _context.AcademicYears.Remove(academicYear);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

