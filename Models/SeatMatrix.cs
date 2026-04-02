using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionManagement.Models
{
   
        public class SeatMatrix
        {
            public int Id { get; set; }
            public int ProgramId { get; set; }
            public int AcademicYearId { get; set; }
            [Range(1, 1000)] public int TotalIntake { get; set; }
            public int KcetSeats { get; set; }
            public int ComedkSeats { get; set; }
            public int ManagementSeats { get; set; }
            public int SupernumerarySeats { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            [ForeignKey(nameof(ProgramId))]
            public Program Program { get; set; } = null!;
            [ForeignKey(nameof(AcademicYearId))]
            public AcademicYear AcademicYear { get; set; } = null!;
        }
    }

