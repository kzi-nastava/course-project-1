using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Data.Context
{
    public class HealthCareContext: DbContext
    {
        public DbSet<Anamnesis> Anamneses { get; set; }
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationApproval> ExaminationApprovals { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public HealthCareContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Anamnesis>()
                .HasOne(x => x.Examination)
                .WithOne(x => x.Anamnesis)
                .HasForeignKey<Examination>(x => new { x.patientId, x.roomId, x.doctorId, x.StartTime })
                .IsRequired();
            modelBuilder.Entity<Examination>()
                .HasOne(x => x.Anamnesis)
                .WithOne(x => x.Examination);
            modelBuilder.Entity<Anamnesis>().HasKey(x => new {x.patientId, x.roomId, x.doctorId, x.StartTime});
            modelBuilder.Entity<Examination>().HasKey(x => new {x.patientId, x.roomId, x.doctorId, x.StartTime});


            modelBuilder.Entity<ExaminationApproval>()
                .HasOne(x => x.Examination)
                .WithOne(x => x.ExaminationApproval)
                .HasForeignKey<Examination>(x => new { x.patientId, x.roomId, x.doctorId, x.StartTime });
            modelBuilder.Entity<Examination>()
                .HasOne(x => x.ExaminationApproval)
                .WithOne(x => x.Examination);
            modelBuilder.Entity<ExaminationApproval>().HasKey(x => new {x.PatientId, x.RoomId, x.DoctorId, x.StartTime});



            //modelBuilder.Entity<Equipment>()
            //    .HasOne(x => x.EquipmentType)
            //    .WithMany(x => x.Equipments)
            //    .IsRequired();
            //modelBuilder.Entity<EquipmentType>()
            //    .HasMany(x => x.Equipments)
            //    .WithOne(x => x.EquipmentType);


            //modelBuilder.Entity<Equipment>()
            //    .HasMany(x => x.Transfers)
            //    .WithOne(x => x.Equipment);
            //modelBuilder.Entity<Transfer>()
            //    .HasOne(x => x.Equipment)
            //    .WithMany(x => x.Transfers)
            //    .IsRequired();


            //modelBuilder.Entity<Equipment>()
            //    .HasMany(x => x.Inventories)
            //    .WithOne(x => x.Equipment);
            //modelBuilder.Entity<Inventory>()
            //    .HasOne(x => x.Equipment)
            //    .WithMany(x => x.Inventories)
            //    .IsRequired();
            modelBuilder.Entity<Inventory>().HasKey(x => new {x.roomId, x.equipmentId });


            //modelBuilder.Entity<Room>()
            //    .HasMany(x => x.Inventories)
            //    .WithOne(x => x.Room);
            //modelBuilder.Entity<Inventory>()
            //    .HasOne(x => x.Room)
            //    .WithMany(x => x.Inventories)
            //    .IsRequired();


            //modelBuilder.Entity<Room>()
            //    .HasMany(x => x.TransfersFrom)
            //    .WithOne(x => x.RoomFrom);
            //modelBuilder.Entity<Transfer>()
            //    .HasOne(x => x.RoomFrom)
            //    .WithMany(x => x.TransfersFrom)
            //    .IsRequired();
            modelBuilder.Entity<Transfer>().HasKey(x => new {x.RoomFromId, x.EquipmentId, x.RoomToId, x.TransferTime});


            //modelBuilder.Entity<Room>()
            //    .HasMany(x => x.TransfersTo)
            //    .WithOne(x => x.RoomTo);
            //modelBuilder.Entity<Transfer>()
            //    .HasOne(x => x.RoomTo)
            //    .WithMany(x => x.TransfersTo)
            //    .IsRequired();


            //modelBuilder.Entity<Room>()
            //    .HasOne(x => x.RoomType)
            //    .WithMany(x => x.Rooms)
            //    .IsRequired();
            //modelBuilder.Entity<RoomType>()
            //    .HasMany(x => x.Rooms)
            //    .WithOne(x => x.RoomType);


            //modelBuilder.Entity<Room>()
            //    .HasMany(x => x.Operations)
            //    .WithOne(x => x.Room);
            //modelBuilder.Entity<Operation>()
            //    .HasOne(x => x.Room)
            //    .WithMany(x => x.Operations)
            //    .IsRequired();
            modelBuilder.Entity<Operation>().HasKey(x => new {x.RoomId, x.DoctorId, x.PatientId, x.Duration});


            //modelBuilder.Entity<Operation>()
            //    .HasOne(x => x.Patient)
            //    .WithMany(x => x.Operations)
            //    .IsRequired();
            //modelBuilder.Entity<Patient>()
            //    .HasMany(x => x.Operations)
            //    .WithOne(x => x.Patient);


            //modelBuilder.Entity<Operation>()
            //    .HasOne(x => x.Doctor)
            //    .WithMany(x => x.Operations)
            //    .IsRequired();
            //modelBuilder.Entity<Doctor>()
            //    .HasMany(x => x.Operations)
            //    .WithOne(x => x.Doctor);

            //modelBuilder.Entity<Patient>()
            //    .HasMany(x => x.Examinations)
            //    .WithOne(x => x.Patient);
            //modelBuilder.Entity<Examination>()
            //    .HasOne(x => x.Patient)
            //    .WithMany(x => x.Examinations)
            //    .IsRequired();


            //modelBuilder.Entity<Doctor>()
            //    .HasMany(x => x.Examinations)
            //    .WithOne(x => x.Doctor);
            //modelBuilder.Entity<Examination>()
            //    .HasOne(x => x.Doctor)
            //    .WithMany(x => x.Examinations)
            //    .IsRequired();


            //modelBuilder.Entity<MedicalRecord>()
            //    .HasOne(x => x.Patient)
            //    .WithOne(x => x.MedicalRecord)
            //    .IsRequired();
            //modelBuilder.Entity<Patient>()
            //    .HasOne(x => x.MedicalRecord)
            //    .WithOne(x => x.Patient)
            //    .HasForeignKey<MedicalRecord>(x => x.PatientId);
            modelBuilder.Entity<MedicalRecord>().HasKey(x => x.PatientId);


            //modelBuilder.Entity<Credentials>()
            //    .HasOne(x => x.UserRole)
            //    .WithMany(x => x.Credentials)
            //    .HasForeignKey(x => x.userRoleId)
            //    .IsRequired();
            //modelBuilder.Entity<UserRole>()
            //    .HasMany(x => x.Credentials)
            //    .WithOne(x => x.UserRole);


            //modelBuilder.Entity<Credentials>()
            //    .HasOne(x => x.Doctor)
            //    .WithOne(x => x.Credentials)
            //    .HasForeignKey<Doctor>(x => x.Id)
            //    .IsRequired();
            //modelBuilder.Entity<Doctor>()
            //    .HasOne(x => x.Credentials)
            //    .WithOne(x => x.Doctor)
            //    .HasForeignKey<Credentials>(x => x.doctorId);


            //modelBuilder.Entity<Credentials>()
            //    .HasOne(x => x.Patient)
            //    .WithOne(x => x.Credentials)
            //    .HasForeignKey<Patient>(x => x.Id)
            //    .IsRequired();
            //modelBuilder.Entity<Patient>()
            //    .HasOne(x => x.Credentials)
            //    .WithOne(x => x.Patient)
            //    .HasForeignKey<Credentials>(x => x.patientId);



            //modelBuilder.Entity<Credentials>()
            //    .HasOne(x => x.Secretary)
            //    .WithOne(x => x.Credentials)
            //    .HasForeignKey<Secretary>(x => x.Id)
            //    .IsRequired();
            //modelBuilder.Entity<Secretary>()
            //    .HasOne(x => x.Credentials)
            //    .WithOne(x => x.Secretary)
            //    .HasForeignKey<Credentials>(x => x.secretaryId);


            //modelBuilder.Entity<Credentials>()
            //    .HasOne(x => x.Manager)
            //    .WithOne(x => x.Credentials)
            //    .HasForeignKey<Manager>(x => x.Id)
            //    .IsRequired();
            //modelBuilder.Entity<Manager>()
            //    .HasOne(x => x.Credentials)
            //    .WithOne(x => x.Manager)
            //    .HasForeignKey<Credentials>(x => x.managerId);


        }
    }
}
