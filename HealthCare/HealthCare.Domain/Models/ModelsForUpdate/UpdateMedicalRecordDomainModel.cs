namespace HealthCare.Domain.Models.ModelsForUpdate {
    public class UpdateMedicalRecordDomainModel {
        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public string BedriddenDiseases { get; set; }

        public string Allergies { get; set; }

        public bool isDeleted { get; set; }
    }
}