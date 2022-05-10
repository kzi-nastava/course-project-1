﻿using System;
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
        public DbSet<AntiTroll> AntiTrolls { get; set; }
        public DbSet<Anamnesis> Anamneses { get; set; }
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationApproval> ExaminationApprovals { get; set; }
        public DbSet<Ingredient> Ingridients { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ReferralLetter> ReferralLetters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Renovation> Renovations{ get; set; }

        public HealthCareContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Anamnesis>()
                .HasOne(x => x.Examination)
                .WithOne(x => x.Anamnesis)
                .IsRequired();
            modelBuilder.Entity<Examination>()
                .HasOne(x => x.Anamnesis)
                .WithOne(x => x.Examination)
                .IsRequired(false);

            modelBuilder.Entity<MedicalRecord>()
                .HasMany(x => x.ReferralLetters)
                .WithOne(x => x.MedicalRecord)
                .HasForeignKey(x => x.PatientId);

            
            modelBuilder.Entity<Anamnesis>().HasKey(x => x.Id);
            modelBuilder.Entity<Examination>().HasKey(x => x.Id);
            modelBuilder.Entity<ExaminationApproval>().HasKey(x => x.Id);
            modelBuilder.Entity<Inventory>().HasKey(x => new {x.RoomId, x.EquipmentId });
            modelBuilder.Entity<Transfer>().HasKey(x => new {x.Id});
            modelBuilder.Entity<Operation>().HasKey(x => new {x.Id});
            modelBuilder.Entity<MedicalRecord>().HasKey(x => x.PatientId);
            modelBuilder.Entity<EquipmentType>().HasKey(x => x.Id);
            modelBuilder.Entity<Equipment>().HasKey(x => x.Id);
            modelBuilder.Entity<Room>().HasKey(x => x.Id);
            modelBuilder.Entity<RoomType>().HasKey(x => x.Id);
            modelBuilder.Entity<ReferralLetter>().HasKey(x => x.Id);
            modelBuilder.Entity<Ingredient>().HasKey(x => x.Id);
            modelBuilder.Entity<Specialization>().HasKey(x => x.Id);
            modelBuilder.Entity<Renovation>().HasKey(x => x.Id);
        }
    }
}
