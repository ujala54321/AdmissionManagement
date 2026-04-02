using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionManagement.Models
{
    public class Models
    {
    }
 
    public class AcademicYear
    {

        public int Id { get; set; }
        [Required, MaxLength(20)] public string Label { get; set; } = "";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SeatMatrix> SeatMatrices { get; set; } = new List<SeatMatrix>();
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
    }

    public class Institution
    {
        public int Id { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = "";
        [Required, MaxLength(20)] public string Code { get; set; } = "";
        [MaxLength(200)] public string? Location { get; set; }
        [MaxLength(50)] public string? Type { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Campus> Campuses { get; set; } = new List<Campus>();
    }

    public class Campus
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = "";
        [MaxLength(500)] public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(InstitutionId))]
        public Institution Institution { get; set; } = null!;
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }

    public class Department
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = "";
        [Required, MaxLength(20)] public string Code { get; set; } = "";
        [MaxLength(200)] public string? HeadName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CampusId))]
        public Campus Campus { get; set; } = null!;
        public ICollection<Program> Programs { get; set; } = new List<Program>();
    }

    public class Program
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = "";
        [Required, MaxLength(20)] public string Code { get; set; } = "";
        [Required, MaxLength(10)] public string CourseType { get; set; } = "UG";
        [Required, MaxLength(20)] public string EntryType { get; set; } = "Regular";
        [Required, MaxLength(20)] public string AdmissionMode { get; set; } = "Both";
        public int DurationYears { get; set; } = 4;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;
        public ICollection<SeatMatrix> SeatMatrices { get; set; } = new List<SeatMatrix>();
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
    }

    // ─────────────────────────────────────────────────────────────
    // SEAT MATRIX
    // ─────────────────────────────────────────────────────────────

    //public class SeatMatrix
    //{
    //    public int Id { get; set; }
    //    public int ProgramId { get; set; }
    //    public int AcademicYearId { get; set; }
    //    [Range(1, 1000)] public int TotalIntake { get; set; }
    //    public int KcetSeats { get; set; }
    //    public int ComedkSeats { get; set; }
    //    public int ManagementSeats { get; set; }
    //    public int SupernumerarySeats { get; set; }
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //    [ForeignKey(nameof(ProgramId))]
    //    public Program Program { get; set; } = null!;
    //    [ForeignKey(nameof(AcademicYearId))]
    //    public AcademicYear AcademicYear { get; set; } = null!;
    //}

    //// ─────────────────────────────────────────────────────────────
    //// APPLICANT
    //// ─────────────────────────────────────────────────────────────

    public class Applicant
    {
        public int Id { get; set; }

        [Required, MaxLength(20)] public string ApplicationNo { get; set; } = "";
        [Required, MaxLength(100)] public string FirstName { get; set; } = "";
        [Required, MaxLength(100)] public string LastName { get; set; } = "";
        [Required] public DateOnly DateOfBirth { get; set; }
        [Required, MaxLength(10)] public string Gender { get; set; } = "";
        [Required, MaxLength(15)] public string Mobile { get; set; } = "";
        [Required, MaxLength(200), EmailAddress] public string Email { get; set; } = "";
        [MaxLength(10)] public string Category { get; set; } = "GM";
        [MaxLength(50)] public string Nationality { get; set; } = "Indian";
        [MaxLength(100)] public string? QualifyingExam { get; set; }
        public decimal? Marks { get; set; }
        [MaxLength(20)] public string EntryType { get; set; } = "Regular";
        [MaxLength(20)] public string QuotaType { get; set; } = "KCET";
        public int ProgramId { get; set; }
        public int AcademicYearId { get; set; }
        [MaxLength(50)] public string? AadharNo { get; set; }
        [MaxLength(500)] public string? Address { get; set; }
        [MaxLength(100)] public string? AllotmentNo { get; set; }
        [MaxLength(20)] public string SeatStatus { get; set; } = "Pending";
        [MaxLength(20)] public string FeeStatus { get; set; } = "Pending";
        [MaxLength(100)] public string? AdmissionNo { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProgramId))]
        public Program Program { get; set; } = null!;
        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear AcademicYear { get; set; } = null!;
        public ICollection<ApplicantDocument> Documents { get; set; } = new List<ApplicantDocument>();

        [NotMapped] public string FullName => $"{FirstName} {LastName}";
    }

    // ─────────────────────────────────────────────────────────────
    // DOCUMENTS
    // ─────────────────────────────────────────────────────────────

    public class DocumentType
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        public ICollection<ApplicantDocument> ApplicantDocuments { get; set; } = new List<ApplicantDocument>();
    }

    public class ApplicantDocument
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int DocumentTypeId { get; set; }
        [MaxLength(20)] public string Status { get; set; } = "Pending";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ApplicantId))]
        public Applicant Applicant { get; set; } = null!;
        [ForeignKey(nameof(DocumentTypeId))]
        public DocumentType DocumentType { get; set; } = null!;
    }

    // ─────────────────────────────────────────────────────────────
    // READ-ONLY: SQL View projection
    // ─────────────────────────────────────────────────────────────

    public class SeatAvailabilityView
    {
        public int SeatMatrixId { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = "";
        public string ProgramCode { get; set; } = "";
        public string CourseType { get; set; } = "";
        public string EntryType { get; set; } = "";
        public int AcademicYearId { get; set; }
        public string AcademicYearLabel { get; set; } = "";
        public int TotalIntake { get; set; }
        public int KcetSeats { get; set; }
        public int ComedkSeats { get; set; }
        public int ManagementSeats { get; set; }
        public int SupernumerarySeats { get; set; }
        public int KcetAllocated { get; set; }
        public int ComedkAllocated { get; set; }
        public int MgmtAllocated { get; set; }
        public int KcetRemaining { get; set; }
        public int ComedkRemaining { get; set; }
        public int MgmtRemaining { get; set; }
        public int TotalAllocated { get; set; }
        public int TotalRemaining { get; set; }
    }
}

