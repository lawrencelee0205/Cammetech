﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace v3x.Models
{
    public class PaySlip
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int SalaryModificationId { get; set; }

    }
}
