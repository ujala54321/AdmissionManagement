using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdmissionManagement.Models.ViewModels;

namespace AdmissionManagement.Controllers
{
 

    public class DashboardController : Controller
    {
        private readonly AdmitProDbContext _db;
        public DashboardController(AdmitProDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var currentYear = await _db.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent);
            if (currentYear == null)
                return View(new DashboardVm());

            var applicants = await _db.Applicants
                .Where(a => a.AcademicYearId == currentYear.Id)
                .Include(a => a.Documents)
                .ToListAsync();

            var seatData = await _db.SeatAvailability
                .Where(s => s.AcademicYearId == currentYear.Id)
                .ToListAsync();

            var vm = new DashboardVm
            {
                TotalIntake = seatData.Sum(s => s.TotalIntake),
                TotalAllocated = applicants.Count(a => a.SeatStatus is "Allocated" or "Confirmed"),
                TotalConfirmed = applicants.Count(a => a.SeatStatus == "Confirmed"),
                FeePending = applicants.Count(a => a.SeatStatus == "Allocated" && a.FeeStatus == "Pending"),
                FeePaidCount = applicants.Count(a => a.FeeStatus == "Paid"),
                DocsPending = applicants.Count(a => a.Documents.Any(d => d.Status == "Pending")),
                UnallocatedCount = applicants.Count(a => a.SeatStatus == "Pending"),
                ProgramWise = seatData,
            };

            // Quota summaries
            foreach (var quota in new[] { "KCET", "COMEDK", "Management" })
            {
                int total = quota switch
                {
                    "KCET" => seatData.Sum(s => s.KcetSeats),
                    "COMEDK" => seatData.Sum(s => s.ComedkSeats),
                    "Management" => seatData.Sum(s => s.ManagementSeats),
                    _ => 0
                };
                int used = applicants.Count(a => a.QuotaType == quota
                            && a.SeatStatus is "Allocated" or "Confirmed");
                vm.QuotaSummaries.Add(new QuotaSummaryVm { Quota = quota, Total = total, Used = used });
            }

            return View(vm);
        }
    }
}

