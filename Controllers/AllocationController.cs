using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.ViewModels;

namespace AdmissionManagement.Controllers
{
    public class AllocationController : Controller
    {
        private readonly AdmitProDbContext _db;
        public AllocationController(AdmitProDbContext db) => _db = db;

        // ── ALLOCATION PAGE ───────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var currentYear = await _db.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent);

            var vm = new AllocationPageVm
            {
                Programs = await _db.Programs.Include(p => p.Department).OrderBy(p => p.Code).ToListAsync(),
                SeatAvailability = currentYear != null
                    ? await _db.SeatAvailability.Where(s => s.AcademicYearId == currentYear.Id).ToListAsync()
                    : new(),
                PendingApplicants = await _db.Applicants
                    .Include(a => a.Program)
                    .Where(a => a.SeatStatus == "Pending")
                    .OrderBy(a => a.ApplicationNo).ToListAsync(),
                AllocatedApplicants = await _db.Applicants
                    .Include(a => a.Program)
                    .Include(a => a.Documents)
                    .Where(a => a.SeatStatus == "Allocated")
                    .OrderBy(a => a.ApplicationNo).ToListAsync()
            };
            return View(vm);
        }

        // ── ALLOCATE SEAT (calls sp_AllocateSeat) ─────────────────

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateSeat(AllocateSeatFormVm vm)
        {
            if (!ModelState.IsValid)
            { TempData["Error"] = "Invalid data. Please try again."; return RedirectToAction(nameof(Index)); }

            var result = await ExecSp(
                "sp_AllocateSeat",
                new SqlParameter("@ApplicantId", vm.ApplicantId),
                new SqlParameter("@ProgramId", vm.ProgramId),
                new SqlParameter("@QuotaType", vm.QuotaType),
                new SqlParameter("@AllotmentNo", (object?)vm.AllotmentNo ?? DBNull.Value)
            );

            if (result.StartsWith("SUCCESS"))
                TempData["Success"] = result.Replace("SUCCESS: ", "");
            else
                TempData["Error"] = result.Replace("ERROR: ", "");

            return RedirectToAction(nameof(Index));
        }

        // ── UPDATE FEE STATUS (calls sp_UpdateFeeStatus) ──────────

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFee(int applicantId, string feeStatus)
        {
            var result = await ExecSp(
                "sp_UpdateFeeStatus",
                new SqlParameter("@ApplicantId", applicantId),
                new SqlParameter("@FeeStatus", feeStatus)
            );

            if (result.StartsWith("SUCCESS"))
                TempData["Success"] = result.Replace("SUCCESS: ", "");
            else
                TempData["Error"] = result.Replace("ERROR: ", "");

            return RedirectToAction(nameof(Index));
        }

        // ── FEE STATUS PAGE ───────────────────────────────────────

        public async Task<IActionResult> FeeStatus()
        {
            var applicants = await _db.Applicants
                .Include(a => a.Program)
                .Where(a => a.SeatStatus == "Allocated" || a.SeatStatus == "Confirmed")
                .OrderBy(a => a.FeeStatus).ThenBy(a => a.ApplicationNo)
                .ToListAsync();
            return View(applicants);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFeeFromPage(int applicantId, string feeStatus)
        {
            var result = await ExecSp(
                "sp_UpdateFeeStatus",
                new SqlParameter("@ApplicantId", applicantId),
                new SqlParameter("@FeeStatus", feeStatus)
            );
            TempData[result.StartsWith("SUCCESS") ? "Success" : "Error"] =
                result.Replace("SUCCESS: ", "").Replace("ERROR: ", "");
            return RedirectToAction(nameof(FeeStatus));
        }

        // ── CONFIRM ADMISSION (calls sp_ConfirmAdmission) ─────────

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAdmission(int applicantId)
        {
            var result = await ExecSp(
                "sp_ConfirmAdmission",
                new SqlParameter("@ApplicantId", applicantId)
            );

            if (result.StartsWith("SUCCESS:"))
            {
                var admNo = result["SUCCESS:".Length..];
                TempData["Success"] = $"Admission confirmed! Number: {admNo}";
            }
            else
            {
                TempData["Error"] = result.Replace("ERROR: ", "");
            }
            return RedirectToAction(nameof(Index));
        }

        // ── CONFIRMED ADMISSIONS LIST ─────────────────────────────

        public async Task<IActionResult> Confirmed()
        {
            var confirmed = await _db.Applicants
                .Include(a => a.Program).ThenInclude(p => p.Department)
                .Include(a => a.AcademicYear)
                .Where(a => a.SeatStatus == "Confirmed")
                .OrderByDescending(a => a.ConfirmedOn)
                .ToListAsync();
            return View(confirmed);
        }

        // ── HELPER: Execute SP and return @Result output param ────

        private async Task<string> ExecSp(string spName, params SqlParameter[] inParams)
        {
            var connStr = _db.Database.GetConnectionString()!;
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (var p in inParams) cmd.Parameters.Add(p);
            var outParam = new SqlParameter("@Result", System.Data.SqlDbType.NVarChar, 200)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);
            await cmd.ExecuteNonQueryAsync();
            return outParam.Value?.ToString() ?? "ERROR: No response from stored procedure.";
        }
    }
}
