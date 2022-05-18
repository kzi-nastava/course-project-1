﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.DTOs
{
    public class DrugSuggestionDTO
    {

        public decimal Id { get; set; }

        public decimal DrugId { get; set; }

        public string State { get; set; }

        public string? Comment { get; set; }

    }
}