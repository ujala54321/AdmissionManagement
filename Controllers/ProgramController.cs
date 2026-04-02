using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.ViewModels;

namespace AdmissionManagement.Controllers
{
    public class ProgramController : Controller
    {
        private readonly AdmitProDbContext _db;
        public ProgramController(AdmitProDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var programs = await _db.Programs
                .Include(p => p.Department).ThenInclude(d => d.Campus).ThenInclude(c => c.Institution)
                .OrderBy(p => p.Department.Name).ThenBy(p => p.Name)
                .ToListAsync();
            return View(programs);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ProgramFormVm
            {
                Departments = await _db.Departments.Include(d => d.Campus).OrderBy(d => d.Name).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProgramFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Departments = await _db.Departments.Include(d => d.Campus).OrderBy(d => d.Name).ToListAsync();
                return View(vm);
            }
            _db.Programs.Add(new Models.Program
            {
                DepartmentId = vm.DepartmentId,
                Name = vm.Name,
                Code = vm.Code.ToUpper(),
                CourseType = vm.CourseType,
                EntryType = vm.EntryType,
                AdmissionMode = vm.AdmissionMode,
                DurationYears = vm.DurationYears
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Program '{vm.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Programs.FindAsync(id);
            if (p == null) return NotFound();
            return View(new ProgramFormVm
            {
                Id = p.Id,
                DepartmentId = p.DepartmentId,
                Name = p.Name,
                Code = p.Code,
                CourseType = p.CourseType,
                EntryType = p.EntryType,
                AdmissionMode = p.AdmissionMode,
                DurationYears = p.DurationYears,
                Departments = await _db.Departments.Include(d => d.Campus).OrderBy(d => d.Name).ToListAsync()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProgramFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Departments = await _db.Departments.Include(d => d.Campus).OrderBy(d => d.Name).ToListAsync();
                return View(vm);
            }
            var p = await _db.Programs.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = vm.Name;
            p.Code = vm.Code.ToUpper();
            p.DepartmentId = vm.DepartmentId;
            p.CourseType = vm.CourseType;
            p.EntryType = vm.EntryType;
            p.AdmissionMode = vm.AdmissionMode;
            p.DurationYears = vm.DurationYears;

            _db.Programs.Update(p);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Program updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Programs.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool confirmed)
        {
            var p = await _db.Programs.FindAsync(id);
            if (p == null) return NotFound();

            _db.Programs.Remove(p);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Program '{p.Name}' deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetProgram(int id)
        {
            var program = await _db.Programs.FindAsync(id);
            return Json(program);
        }
        [HttpPut]
        public async Task<JsonResult> UpdateProgram(int id, [FromBody] ProgramFormVm vm)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data." });
            var p = await _db.Programs.FindAsync(id);
            if (p == null) return Json(new { success = false, message = "Program not found." });
            p.Name = vm.Name;
            p.Code = vm.Code.ToUpper();
            p.DepartmentId = vm.DepartmentId;
            p.CourseType = vm.CourseType;
            p.EntryType = vm.EntryType;
            p.AdmissionMode = vm.AdmissionMode;
            p.DurationYears = vm.DurationYears;
            _db.Programs.Update(p);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Program updated." });
        }

    }
}

