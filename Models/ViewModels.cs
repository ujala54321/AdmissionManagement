using System.ComponentModel.DataAnnotations;
using static AdmissionManagement.Models.Models;

namespace AdmissionManagement.Models
{
    public class ViewModels
    {

    public class DashboardVm
    {
        public int TotalIntake { get; set; }
        public int TotalAllocated { get; set; }
        public int TotalConfirmed { get; set; }
        public int FeePending { get; set; }
        public int DocsPending { get; set; }
        public int UnallocatedCount { get; set; }
        public int FillPercent => TotalIntake > 0 ? (int)Math.Round((double)TotalAllocated / TotalIntake * 100) : 0;

        public List<SeatAvailabilityView> ProgramWise { get; set; } = new();
        public List<QuotaSummaryVm> QuotaSummaries { get; set; } = new();
        public int FeePaidCount { get; set; }
        public int FeePayPercent => (FeePaidCount + FeePending) > 0
                                        ? (int)Math.Round((double)FeePaidCount / (FeePaidCount + FeePending) * 100) : 0;
    }

    public class QuotaSummaryVm
    {
        public string Quota { get; set; } = "";
        public int Total { get; set; }
        public int Used { get; set; }
        public int Remaining => Total - Used;
        public int Percent => Total > 0 ? (int)Math.Round((double)Used / Total * 100) : 0;
    }

    // ─────────────────────────────────────────────────────────────
    // MASTER SETUP
    // ─────────────────────────────────────────────────────────────

    public class MasterSetupVm
    {
        public List<Institution> Institutions { get; set; } = new();
        public List<Campus> Campuses { get; set; } = new();
        public List<Department> Departments { get; set; } = new();
        public List<AcademicYear> AcademicYears { get; set; } = new();
        // Forms
        public InstitutionFormVm InstForm { get; set; } = new();
        public CampusFormVm CampForm { get; set; } = new();
        public DepartmentFormVm DeptForm { get; set; } = new();
        public AcademicYearFormVm YearForm { get; set; } = new();
    }

    public class InstitutionFormVm
    {
        [Required, MaxLength(200), Display(Name = "Institution Name")]
        public string Name { get; set; } = "";
        [Required, MaxLength(20), Display(Name = "Code")]
        public string Code { get; set; } = "";
        [MaxLength(200)] public string? Location { get; set; }
        [MaxLength(50)] public string? Type { get; set; }
    }

    public class CampusFormVm
    {
        [Required] public int InstitutionId { get; set; }
        [Required, MaxLength(200), Display(Name = "Campus Name")]
        public string Name { get; set; } = "";
        [MaxLength(500)] public string? Location { get; set; }
    }

    public class DepartmentFormVm
    {
        [Required] public int CampusId { get; set; }
        [Required, MaxLength(200), Display(Name = "Department Name")]
        public string Name { get; set; } = "";
        [Required, MaxLength(20)]
        public string Code { get; set; } = "";
        [MaxLength(200), Display(Name = "Head of Department")]
        public string? HeadName { get; set; }
    }

    public class AcademicYearFormVm
    {
            internal int Id;

            [Required, MaxLength(20), Display(Name = "Year Label (e.g. 2026-27)")]
        public string Label { get; set; } = "";
        [Required, Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [Required, Display(Name = "End Date")]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
        public bool IsCurrent { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // PROGRAM
    // ─────────────────────────────────────────────────────────────

    public class ProgramFormVm
    {
        public int Id { get; set; }
        [Required, Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Required, MaxLength(200), Display(Name = "Program Name")]
        public string Name { get; set; } = "";
        [Required, MaxLength(20), Display(Name = "Short Code")]
        public string Code { get; set; } = "";
        [Required, Display(Name = "Course Type")]
        public string CourseType { get; set; } = "UG";
        [Required, Display(Name = "Entry Type")]
        public string EntryType { get; set; } = "Regular";
        [Required, Display(Name = "Admission Mode")]
        public string AdmissionMode { get; set; } = "Both";
        [Range(1, 5), Display(Name = "Duration (Years)")]
        public int DurationYears { get; set; } = 4;
        // Dropdown data
        public List<Department> Departments { get; set; } = new();
    }

    // ─────────────────────────────────────────────────────────────
    // SEAT MATRIX
    // ─────────────────────────────────────────────────────────────

    public class SeatMatrixFormVm
    {
        public int Id { get; set; }
        [Required, Display(Name = "Program")]
        public int ProgramId { get; set; }
        [Required, Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; }
        [Required, Range(1, 1000), Display(Name = "Total Intake")]
        public int TotalIntake { get; set; }
        [Range(0, 500), Display(Name = "KCET Seats")]
        public int KcetSeats { get; set; }
        [Range(0, 500), Display(Name = "COMEDK Seats")]
        public int ComedkSeats { get; set; }
        [Range(0, 500), Display(Name = "Management Seats")]
        public int ManagementSeats { get; set; }
        [Range(0, 100), Display(Name = "Supernumerary Seats")]
        public int SupernumerarySeats { get; set; }
        // Dropdown data
        public List<Program> Programs { get; set; } = new();
        public List<AcademicYear> AcademicYears { get; set; } = new();
        // Validation helper
        [System.Text.Json.Serialization.JsonIgnore]
        public bool QuotaValid => KcetSeats + ComedkSeats + ManagementSeats == TotalIntake;
    }

    // ─────────────────────────────────────────────────────────────
    // APPLICANT
    // ─────────────────────────────────────────────────────────────

    public class ApplicantFormVm
    {
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100), Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [Required, Display(Name = "Date of Birth")]
        public DateOnly DateOfBirth { get; set; } = new(2006, 1, 1);

        [Required, Display(Name = "Gender")]
        public string Gender { get; set; } = "Male";

        [Required, MaxLength(15), Display(Name = "Mobile")]
        public string Mobile { get; set; } = "";

        [Required, MaxLength(200), EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required, Display(Name = "Category")]
        public string Category { get; set; } = "GM";

        [Display(Name = "Nationality")]
        public string Nationality { get; set; } = "Indian";

        [Display(Name = "Qualifying Exam")]
        public string? QualifyingExam { get; set; }

        [Range(0, 100), Display(Name = "Marks / Percentage")]
        public decimal? Marks { get; set; }

        [Required, Display(Name = "Entry Type")]
        public string EntryType { get; set; } = "Regular";

        [Required, Display(Name = "Quota Type")]
        public string QuotaType { get; set; } = "KCET";

        [Required, Display(Name = "Program")]
        public int ProgramId { get; set; }

        [Required, Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; }

        [MaxLength(50), Display(Name = "Aadhar / ID No")]
        public string? AadharNo { get; set; }

        [MaxLength(500), Display(Name = "Address")]
        public string? Address { get; set; }

        // Document statuses: DocumentTypeId → status string
        public Dictionary<int, string> DocStatuses { get; set; } = new();

        // Dropdowns
        public List<Program> Programs { get; set; } = new();
        public List<AcademicYear> AcademicYears { get; set; } = new();
        public List<DocumentType> DocumentTypes { get; set; } = new();
    }

    public class ApplicantListItemVm
    {
        public int Id { get; set; }
        public string ApplicationNo { get; set; } = "";
        public string FullName { get; set; } = "";
        public string ProgramCode { get; set; } = "";
        public string ProgramName { get; set; } = "";
        public string QuotaType { get; set; } = "";
        public string Category { get; set; } = "";
        public string SeatStatus { get; set; } = "";
        public string FeeStatus { get; set; } = "";
        public string? AdmissionNo { get; set; }
        public bool AllDocsVerified { get; set; }
        public bool AnyDocPending { get; set; }
        public string? AllotmentNo { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // SEAT ALLOCATION
    // ─────────────────────────────────────────────────────────────

    public class AllocationPageVm
    {
        public List<Applicant> PendingApplicants { get; set; } = new();
        public List<Applicant> AllocatedApplicants { get; set; } = new();
        public List<Program> Programs { get; set; } = new();
        public List<SeatAvailabilityView> SeatAvailability { get; set; } = new();
    }

    public class AllocateSeatFormVm
    {
        [Required] public int ApplicantId { get; set; }
        [Required] public int ProgramId { get; set; }
        [Required] public string QuotaType { get; set; } = "";
        public string? AllotmentNo { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // DOCUMENT VERIFICATION
    // ─────────────────────────────────────────────────────────────

    public class DocVerifyVm
    {
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; } = "";
        public List<DocItemVm> Items { get; set; } = new();
    }

    public class DocItemVm
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = "";
        public string Status { get; set; } = "Pending";
    }
}
}
