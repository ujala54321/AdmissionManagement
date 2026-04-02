using AdmissionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.Models;
using static AdmissionManagement.Models.ViewModels;

namespace AdmissionManagement.Controllers
{
  
    public class ApplicantController : Controller
    {
        private readonly AdmitProDbContext _db;
        public ApplicantController(AdmitProDbContext db) => _db = db;

        // ── LIST ─────────────────────────────────────────────────

        public async Task<IActionResult> Index(string? q, string? seatStatus, string? quota, string? feeStatus)
        {
            var query = _db.Applicants
                .Include(a => a.Program)
                .Include(a => a.Documents)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(a =>
                    a.FirstName.Contains(q) || a.LastName.Contains(q) ||
                    a.ApplicationNo.Contains(q) || a.Mobile.Contains(q));

            if (!string.IsNullOrWhiteSpace(seatStatus)) query = query.Where(a => a.SeatStatus == seatStatus);
            if (!string.IsNullOrWhiteSpace(quota)) query = query.Where(a => a.QuotaType == quota);
            if (!string.IsNullOrWhiteSpace(feeStatus)) query = query.Where(a => a.FeeStatus == feeStatus);

            var list = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
            var vm = list.Select(a => new ApplicantListItemVm
            {
                Id = a.Id,
                ApplicationNo = a.ApplicationNo,
                FullName = a.FullName,
                ProgramCode = a.Program?.Code ?? "—",
                ProgramName = a.Program?.Name ?? "—",
                QuotaType = a.QuotaType,
                Category = a.Category,
                SeatStatus = a.SeatStatus,
                FeeStatus = a.FeeStatus,
                AdmissionNo = a.AdmissionNo,
                AllotmentNo = a.AllotmentNo,
                AllDocsVerified = a.Documents.Any() && a.Documents.All(d => d.Status == "Verified"),
                AnyDocPending = a.Documents.Any(d => d.Status == "Pending"),
            }).ToList();

            ViewBag.Q = q;
            ViewBag.SeatStatus = seatStatus;
            ViewBag.Quota = quota;
            ViewBag.FeeStatus = feeStatus;
            return View(vm);
        }

        // ── CREATE ────────────────────────────────────────────────

        public async Task<IActionResult> Create()
            => View(await BuildFormVm(null));

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicantFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdowns(vm);
                return View(vm);
            }

            // Generate sequential application number
            var count = await _db.Applicants.CountAsync();
            var appNo = $"APP{(count + 1):D4}";
            while (await _db.Applicants.AnyAsync(a => a.ApplicationNo == appNo))
                appNo = $"APP{(++count + 1):D4}";

            var applicant = Map(vm);
            applicant.ApplicationNo = appNo;

            _db.Applicants.Add(applicant);
            await _db.SaveChangesAsync();

            // Create document records
            var docTypes = await _db.DocumentTypes.ToListAsync();
            foreach (var dt in docTypes)
            {
                vm.DocStatuses.TryGetValue(dt.Id, out var status);
                _db.ApplicantDocuments.Add(new Models.ApplicantDocument
                {
                    ApplicantId = applicant.Id,
                    DocumentTypeId = dt.Id,
                    Status = string.IsNullOrEmpty(status) ? "Pending" : status
                });
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Applicant {appNo} created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ── DETAILS ───────────────────────────────────────────────

        public async Task<IActionResult> Details(int id)
        {
            var a = await _db.Applicants
                .Include(x => x.Program).ThenInclude(p => p.Department).ThenInclude(d => d.Campus)
                .Include(x => x.AcademicYear)
                .Include(x => x.Documents).ThenInclude(d => d.DocumentType)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // ── EDIT ─────────────────────────────────────────────────

        public async Task<IActionResult> Edit(int id)
        {
            var a = await _db.Applicants.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();
            if (a.SeatStatus == "Confirmed")
            {
                TempData["Error"] = "Cannot edit a confirmed admission.";
                return RedirectToAction(nameof(Index));
            }
            return View(await BuildFormVm(a));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicantFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdowns(vm);
                return View(vm);
            }
            var a = await _db.Applicants.FindAsync(vm.Id);
            if (a == null) return NotFound();

            a.FirstName = vm.FirstName; a.LastName = vm.LastName; a.DateOfBirth = vm.DateOfBirth;
            a.Gender = vm.Gender; a.Mobile = vm.Mobile; a.Email = vm.Email;
            a.Category = vm.Category; a.Nationality = vm.Nationality;
            a.QualifyingExam = vm.QualifyingExam; a.Marks = vm.Marks;
            a.EntryType = vm.EntryType; a.QuotaType = vm.QuotaType;
            a.ProgramId = vm.ProgramId; a.AcademicYearId = vm.AcademicYearId;
            a.AadharNo = vm.AadharNo; a.Address = vm.Address;
            a.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Applicant updated.";
            return RedirectToAction(nameof(Index));
        }
        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var applicant = await _db.Applicants.FindAsync(id);
            if (applicant == null) return NotFound();
            return View(applicant);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicant = await _db.Applicants.FindAsync(id);
            if (applicant != null)
            {
                _db.Applicants.Remove(applicant);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ── DOCUMENTS ─────────────────────────────────────────────

        public async Task<IActionResult> Documents(int id)
        {
            var a = await _db.Applicants
                .Include(x => x.Documents).ThenInclude(d => d.DocumentType)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            var vm = new DocVerifyVm
            {
                ApplicantId = a.Id,
                ApplicantName = a.FullName,
                Items = a.Documents
                    .OrderBy(d => d.DocumentType.Name)
                    .Select(d => new DocItemVm
                    {
                        DocumentTypeId = d.DocumentTypeId,
                        DocumentTypeName = d.DocumentType.Name,
                        Status = d.Status
                    }).ToList()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDocuments(DocVerifyVm vm)
        {
            foreach (var item in vm.Items)
            {
                var doc = await _db.ApplicantDocuments
                    .FirstOrDefaultAsync(d => d.ApplicantId == vm.ApplicantId
                                           && d.DocumentTypeId == item.DocumentTypeId);
                if (doc != null)
                {
                    doc.Status = item.Status;
                    doc.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Documents updated.";
            return RedirectToAction(nameof(Details), new { id = vm.ApplicantId });
        }

        // ── HELPERS ───────────────────────────────────────────────

        private async Task<ApplicantFormVm> BuildFormVm(Applicant? a)
        {
            var currentYear = await _db.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent);
            var vm = new ApplicantFormVm
            {
                Programs = await _db.Programs.Where(p => p.IsActive).Include(p => p.Department).OrderBy(p => p.Name).ToListAsync(),
                AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync(),
                DocumentTypes = await _db.DocumentTypes.OrderBy(d => d.Name).ToListAsync(),
                AcademicYearId = currentYear?.Id ?? 0
            };
            if (a != null)
            {
                vm.Id = a.Id; vm.FirstName = a.FirstName; vm.LastName = a.LastName;
                vm.DateOfBirth = a.DateOfBirth; vm.Gender = a.Gender;
                vm.Mobile = a.Mobile; vm.Email = a.Email;
                vm.Category = a.Category; vm.Nationality = a.Nationality;
                vm.QualifyingExam = a.QualifyingExam; vm.Marks = a.Marks;
                vm.EntryType = a.EntryType; vm.QuotaType = a.QuotaType;
                vm.ProgramId = a.ProgramId; vm.AcademicYearId = a.AcademicYearId;
                vm.AadharNo = a.AadharNo; vm.Address = a.Address;
                vm.DocStatuses = a.Documents.ToDictionary(d => d.DocumentTypeId, d => d.Status);
            }
            return vm;
        }

        private async Task FillDropdowns(ApplicantFormVm vm)
        {
            vm.Programs = await _db.Programs.Where(p => p.IsActive).Include(p => p.Department).OrderBy(p => p.Name).ToListAsync();
            vm.AcademicYears = await _db.AcademicYears.OrderByDescending(y => y.IsCurrent).ToListAsync();
            vm.DocumentTypes = await _db.DocumentTypes.OrderBy(d => d.Name).ToListAsync();
        }

        private static Applicant Map(ApplicantFormVm vm) => new()
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            DateOfBirth = vm.DateOfBirth,
            Gender = vm.Gender,
            Mobile = vm.Mobile,
            Email = vm.Email,
            Category = vm.Category,
            Nationality = vm.Nationality,
            QualifyingExam = vm.QualifyingExam,
            Marks = vm.Marks,
            EntryType = vm.EntryType,
            QuotaType = vm.QuotaType,
            ProgramId = vm.ProgramId,
            AcademicYearId = vm.AcademicYearId,
            AadharNo = vm.AadharNo,
            Address = vm.Address
        };
    }
}
