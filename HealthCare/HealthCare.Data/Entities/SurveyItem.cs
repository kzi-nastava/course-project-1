using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    public class SurveyItem
    {
        [Column("question_id")]
        public string QuestionId { get; set; }

        [Column("survey_id")]
        public string SurveyId { get; set; }

        public Survey Survey { get; set; }
        public Question Question { get; set; }
    }
}
