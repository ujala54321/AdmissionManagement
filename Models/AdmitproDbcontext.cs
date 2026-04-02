using AdmissionManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using static AdmissionManagement.Models.Models;

namespace AdmissionManagement;

public class AdmitProDbContext : DbContext
{
    public AdmitProDbContext(DbContextOptions<AdmitProDbContext> options) : base(options) { }

    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Campus> Campuses { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Models.Program> Programs { get; set; }
    public DbSet<SeatMatrix> SeatMatrices { get; set; }
    public DbSet<Applicant> Applicants { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<ApplicantDocument> ApplicantDocuments { get; set; }
    public DbSet<SeatAvailabilityView> SeatAvailability { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Seat matrix: unique per program+year
        mb.Entity<SeatMatrix>()
            .HasIndex(s => new { s.ProgramId, s.AcademicYearId }).IsUnique();

        // Applicant document: unique per applicant+doctype
        mb.Entity<ApplicantDocument>()
            .HasIndex(d => new { d.ApplicantId, d.DocumentTypeId }).IsUnique();

        // Unique admission number (nullable — filter index)
        mb.Entity<Applicant>()
            .HasIndex(a => a.AdmissionNo).IsUnique()
            .HasFilter("[AdmissionNo] IS NOT NULL");

        // Unique application number
        mb.Entity<Applicant>()
            .HasIndex(a => a.ApplicationNo).IsUnique();

        // Map read-only SQL view — no key
        mb.Entity<SeatAvailabilityView>(e =>
        {
            e.HasNoKey();
            e.ToView("vw_SeatAvailability");
        });
    }
}
