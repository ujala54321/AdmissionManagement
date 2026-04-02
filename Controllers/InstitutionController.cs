using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.Models;
using static AdmissionManagement.Models.ViewModels;

namespace AdmissionManagement.Controllers
{
    public class InstitutionController : Controller
    {
        private readonly AdmitProDbContext _db;
        public InstitutionController(AdmitProDbContext db) => _db = db;

        // GET: Institution/Index
        public async Task<IActionResult> Index()
        {
            var vm = new MasterSetupVm
            {
                Institutions = await _db.Institutions.OrderBy(i => i.Name).ToListAsync(),
                Campuses = await _db.Campuses.Include(c => c.Institution).OrderBy(c => c.Name).ToListAsync(),
                Departments = await _db.Departments.Include(d => d.Campus).ThenInclude(c => c.Institution).OrderBy(d => d.Name).ToListAsync(),
                AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ThenByDescending(y => y.Id).ToListAsync(),
            };
            ViewBag.InstitutionList = vm.Institutions;
            ViewBag.CampusList = vm.Campuses;
            return View(vm);
        }

        // POST: Institution/SaveInstitution (Create)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveInstitution(InstitutionFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }
            if (await _db.Institutions.AnyAsync(i => i.Code == f.Code.ToUpper()))
            { TempData["Error"] = $"Code '{f.Code.ToUpper()}' already exists."; return RedirectToAction(nameof(Index)); }

            _db.Institutions.Add(new Models.Institution { Name = f.Name, Code = f.Code.ToUpper(), Location = f.Location, Type = f.Type });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Institution '{f.Name}' created successfully.";
            return RedirectToAction(nameof(Index), new { tab = "institutions" });
        }

        // POST: Institution/UpdateInstitution (Update)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInstitution(int id, InstitutionFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            var institution = await _db.Institutions.FindAsync(id);
            if (institution == null) return NotFound();

            var codeExists = await _db.Institutions.AnyAsync(i => i.Code == f.Code.ToUpper() && i.Id != id);
            if (codeExists) { TempData["Error"] = $"Code '{f.Code.ToUpper()}' already exists."; return RedirectToAction(nameof(Index)); }

            institution.Name = f.Name;
            institution.Code = f.Code.ToUpper();
            institution.Location = f.Location;
            institution.Type = f.Type;

            _db.Institutions.Update(institution);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Institution '{f.Name}' updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "institutions" });
        }

        // POST: Institution/DeleteInstitution
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstitution(int id)
        {
            var institution = await _db.Institutions.FindAsync(id);
            if (institution == null) return NotFound();

            _db.Institutions.Remove(institution);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Institution '{institution.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index), new { tab = "institutions" });
        }

        // POST: Institution/SaveCampus (Create)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCampus(CampusFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            _db.Campuses.Add(new Models.Campus { InstitutionId = f.InstitutionId, Name = f.Name, Location = f.Location });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Campus '{f.Name}' created successfully.";
            return RedirectToAction(nameof(Index), new { tab = "campuses" });
        }

        // POST: Institution/UpdateCampus (Update)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCampus(int id, CampusFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            var campus = await _db.Campuses.FindAsync(id);
            if (campus == null) return NotFound();

            campus.InstitutionId = f.InstitutionId;
            campus.Name = f.Name;
            campus.Location = f.Location;

            _db.Campuses.Update(campus);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Campus '{f.Name}' updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "campuses" });
        }

        // POST: Institution/DeleteCampus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCampus(int id)
        {
            var campus = await _db.Campuses.FindAsync(id);
            if (campus == null) return NotFound();

            _db.Campuses.Remove(campus);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Campus '{campus.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index), new { tab = "campuses" });
        }

        // POST: Institution/SaveDept (Create)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDept(DepartmentFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            _db.Departments.Add(new Models.Department { CampusId = f.CampusId, Name = f.Name, Code = f.Code.ToUpper(), HeadName = f.HeadName });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Department '{f.Name}' created successfully.";
            return RedirectToAction(nameof(Index), new { tab = "departments" });
        }

        // POST: Institution/UpdateDept (Update)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDept(int id, DepartmentFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            var department = await _db.Departments.FindAsync(id);
            if (department == null) return NotFound();

            department.CampusId = f.CampusId;
            department.Name = f.Name;
            department.Code = f.Code.ToUpper();
            department.HeadName = f.HeadName;

            _db.Departments.Update(department);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Department '{f.Name}' updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "departments" });
        }

        // POST: Institution/DeleteDept
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDept(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department == null) return NotFound();

            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Department '{department.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index), new { tab = "departments" });
        }

        // POST: Institution/SaveYear (Create)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveYear(AcademicYearFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            if (f.IsCurrent)
                await _db.AcademicYears.Where(y => y.IsCurrent)
                    .ExecuteUpdateAsync(s => s.SetProperty(y => y.IsCurrent, false));

            _db.AcademicYears.Add(new Models.AcademicYear { Label = f.Label, StartDate = f.StartDate, EndDate = f.EndDate, IsCurrent = f.IsCurrent });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Academic Year '{f.Label}' created successfully.";
            return RedirectToAction(nameof(Index), new { tab = "years" });
        }

        // POST: Institution/UpdateYear (Update)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateYear(int id, AcademicYearFormVm f)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Please fill all required fields."; return RedirectToAction(nameof(Index)); }

            var year = await _db.AcademicYears.FindAsync(id);
            if (year == null) return NotFound();

            if (f.IsCurrent && !year.IsCurrent)
                await _db.AcademicYears.Where(y => y.IsCurrent && y.Id != id)
                    .ExecuteUpdateAsync(s => s.SetProperty(y => y.IsCurrent, false));

            year.Label = f.Label;
            year.StartDate = f.StartDate;
            year.EndDate = f.EndDate;
            year.IsCurrent = f.IsCurrent;

            _db.AcademicYears.Update(year);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Academic Year '{f.Label}' updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "years" });
        }

        // POST: Institution/DeleteYear
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteYear(int id)
        {
            var year = await _db.AcademicYears.FindAsync(id);
            if (year == null) return NotFound();

            _db.AcademicYears.Remove(year);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Academic Year '{year.Label}' deleted successfully.";
            return RedirectToAction(nameof(Index), new { tab = "years" });
        }

        // JSON API endpoints for edit modal population
        [HttpGet]
        public async Task<JsonResult> GetInstitution(int id)
        {
            var inst = await _db.Institutions.FindAsync(id);
            return Json(inst);
        }

        [HttpGet]
        public async Task<JsonResult> GetCampus(int id)
        {
            var campus = await _db.Campuses.FindAsync(id);
            return Json(campus);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartment(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            return Json(dept);
        }

        [HttpGet]
        public async Task<JsonResult> GetAcademicYear(int id)
        {
            var year = await _db.AcademicYears.FindAsync(id);
            return Json(year);
        }
     

    }
}

