using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.ViewModels;
using AdmissionManagement.Models;


namespace AdmissionManagement.Controllers
{
    public class SeatMatrixController : Controller
    {
        private readonly AdmitProDbContext _db;
        public SeatMatrixController(AdmitProDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var data = await _db.SeatMatrices
                .Include(s => s.Program).ThenInclude(p => p.Department)
                .Include(s => s.AcademicYear)
                .OrderBy(s => s.Program.Code)
                .ToListAsync();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new SeatMatrixFormVm
            {
                Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Name).ToListAsync(),
                AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeatMatrixFormVm vm)
        {
            if (vm.KcetSeats + vm.ComedkSeats + vm.ManagementSeats != vm.TotalIntake)
                ModelState.AddModelError("", "KCET + COMEDK + Management seats must equal Total Intake.");

            if (!ModelState.IsValid)
            {
                vm.Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Name).ToListAsync();
                vm.AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync();
                return View(vm);
            }

            // Check if allocated seats already exceed new configuration
            var existing = await _db.SeatMatrices
                .FirstOrDefaultAsync(s => s.ProgramId == vm.ProgramId && s.AcademicYearId == vm.AcademicYearId);

            if (existing != null)
            {
                // Validate we're not shrinking below what's already allocated
                var avail = await _db.SeatAvailability
                    .FirstOrDefaultAsync(s => s.ProgramId == vm.ProgramId && s.AcademicYearId == vm.AcademicYearId);
                if (avail != null)
                {
                    if (vm.KcetSeats < avail.KcetAllocated)
                    { ModelState.AddModelError("", $"Cannot reduce KCET below {avail.KcetAllocated} (already allocated)."); }
                    if (vm.ComedkSeats < avail.ComedkAllocated)
                    { ModelState.AddModelError("", $"Cannot reduce COMEDK below {avail.ComedkAllocated} (already allocated)."); }
                    if (vm.ManagementSeats < avail.MgmtAllocated)
                    { ModelState.AddModelError("", $"Cannot reduce Management below {avail.MgmtAllocated} (already allocated)."); }
                }
                if (!ModelState.IsValid)
                {
                    vm.Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Name).ToListAsync();
                    vm.AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync();
                    return View(vm);
                }
                existing.TotalIntake = vm.TotalIntake; existing.KcetSeats = vm.KcetSeats;
                existing.ComedkSeats = vm.ComedkSeats; existing.ManagementSeats = vm.ManagementSeats;
                existing.SupernumerarySeats = vm.SupernumerarySeats;
                _db.SeatMatrices.Update(existing);
            }
            else
            {
                _db.SeatMatrices.Add(new Models.SeatMatrix
                {
                    ProgramId = vm.ProgramId,
                    AcademicYearId = vm.AcademicYearId,
                    TotalIntake = vm.TotalIntake,
                    KcetSeats = vm.KcetSeats,
                    ComedkSeats = vm.ComedkSeats,
                    ManagementSeats = vm.ManagementSeats,
                    SupernumerarySeats = vm.SupernumerarySeats
                });
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Seat matrix saved.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var seat = await _db.SeatMatrices.FindAsync(id);
            if (seat == null) return NotFound();

            var vm = new SeatMatrixFormVm
            {
                Id = id,
                ProgramId = seat.ProgramId,
                AcademicYearId = seat.AcademicYearId,
                TotalIntake = seat.TotalIntake,
                KcetSeats = seat.KcetSeats,
                ComedkSeats = seat.ComedkSeats,
                ManagementSeats = seat.ManagementSeats,
                SupernumerarySeats = seat.SupernumerarySeats,
                Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Name).ToListAsync(),
                AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeatMatrixFormVm vm)
        {
            if (vm.KcetSeats + vm.ComedkSeats + vm.ManagementSeats != vm.TotalIntake)
                ModelState.AddModelError("", "KCET + COMEDK + Management seats must equal Total Intake.");

            if (!ModelState.IsValid)
            {
                vm.Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Name).ToListAsync();
                vm.AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync();
                return View(vm);
            }

            var seat = await _db.SeatMatrices.FindAsync(id);
            if (seat == null) return NotFound();

            seat.ProgramId = vm.ProgramId;
            seat.AcademicYearId = vm.AcademicYearId;
            seat.TotalIntake = vm.TotalIntake;
            seat.KcetSeats = vm.KcetSeats;
            seat.ComedkSeats = vm.ComedkSeats;
            seat.ManagementSeats = vm.ManagementSeats;
            seat.SupernumerarySeats = vm.SupernumerarySeats;

            _db.SeatMatrices.Update(seat);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Seat matrix updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var seat = await _db.SeatMatrices.FindAsync(id);
            if (seat == null) return NotFound();
            return View(seat);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _db.SeatMatrices.FindAsync(id);
            if (seat == null) return NotFound();

            _db.SeatMatrices.Remove(seat);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Seat matrix deleted.";
            return RedirectToAction(nameof(Index));
        }

        // AJAX: return seat availability for a program/year/quota
        [HttpGet]
        public async Task<IActionResult> Availability(int programId, int academicYearId, string quota)
        {
            var row = await _db.SeatAvailability
                .FirstOrDefaultAsync(s => s.ProgramId == programId && s.AcademicYearId == academicYearId);

            if (row == null) return Json(new { remaining = 0, total = 0, used = 0 });

            int remaining = quota switch
            {
                "KCET" => row.KcetRemaining,
                "COMEDK" => row.ComedkRemaining,
                "Management" => row.MgmtRemaining,
                _ => 0
            };
            int total = quota switch
            {
                "KCET" => row.KcetSeats,
                "COMEDK" => row.ComedkSeats,
                "Management" => row.ManagementSeats,
                _ => 0
            };
            return Json(new { remaining, total, used = total - remaining });
        }
    }
}

