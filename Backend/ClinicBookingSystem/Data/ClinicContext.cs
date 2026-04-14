using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Models;

namespace ClinicBookingSystem.Data
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
        {


        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Speciality> Specialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Prevent multiple appointments at same time for at same patient
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.PatientId, a.AppointmentDateTime })
                .IsUnique();


            modelBuilder.Entity<Patient>()
                .HasIndex(p => new { p.Email, p.BirthDate })
                .IsUnique();

            modelBuilder.Entity<Speciality>()
                .HasIndex(s => s.Name)
                .IsUnique();


            //Seed Appointments
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = new DateTime(2026, 12, 21, 10, 0, 0),
                    DurationMinutes = 30,
                    PatientId = 1,
                    DoctorId = 1,
                    ClinicId = 1
                }
            );

            //Seed Clinics
            modelBuilder.Entity<Clinic>().HasData(
                new Clinic { Id = 1, Name = "Oslo Clinic", Address = "Karl Johans gate 1, 0154 Oslo" },
                new Clinic { Id = 2, Name = "Bergen Clinic", Address = "Bryggen 3, 5003 Bergen" }
            );

            //Seed Doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Ola", LastName = "Nordmann", ClinicId = 1, SpecialityId = 1 },
                new Doctor { Id = 2, FirstName = "Kari", LastName = "Nordmann", ClinicId = 2, SpecialityId = 2 }
            );

            // Seed Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    BirthDate = new DateTime(1990, 5, 14),
                    Gender = "Male",
                    TaxNumber = "123456789",
                    Religion = "None"
                }
            );

            // Seed Specialities
            modelBuilder.Entity<Speciality>().HasData(
                new Speciality { Id = 1, Name = "Cardiology" },
                new Speciality { Id = 2, Name = "Dermatology" }
            );


        }

    }
}