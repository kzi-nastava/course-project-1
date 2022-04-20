using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models {
    public class CredentialsDomainModel {
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal userRoleId { get; set; }

    }
}
