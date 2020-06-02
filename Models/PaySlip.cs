using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace v3x.Models
{
    public class PaySlip
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double AdvancePay { get; set; } = 0.0;
        public double Bonus { get; set; } = 0.0;
        public double Basic { get; set; }
        public double NetSalary { get; set; }
        public double EmployeeEPF { get; set; } = 0.0;
        public double EmployerEPF { get; set; } = 0.0;
        public double EmployeeSocso { get; set; } = 0.0;
        public double EmployerSocso { get; set; } = 0.0;
        public int JobId { get; set; }
        public Job Job { get; set; }



    }
}
