using HealthCare.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models {
    public class CredentialsDomainModel {
        public decimal Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public decimal? doctorId { get; set; }

        public decimal? secretaryId { get; set; }

        public decimal? managerId { get; set; }

        public decimal? patientId { get; set; }

        public decimal userRoleId { get; set; }

        public UserRoleDomainModel UserRole { get; set; }

        //public Manager Manager { get; set; }
        //public Doctor Doctor { get; set; }
        //public Patient Patient { get; set; }
        //public Secretary Secretary { get; set; }

    }
}
