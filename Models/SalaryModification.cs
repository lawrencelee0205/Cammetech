using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace v3x.Models
{
    public class SalaryModification
    {
        public int Id { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]

        public double Bonus { get; set; }

        [Required]
        public double TotalRate { get; set; }
        public int EPFId { get; set; }
        public int SocsoId { get; set; }
        public int EmployeeId { get; set; }

    }
}
