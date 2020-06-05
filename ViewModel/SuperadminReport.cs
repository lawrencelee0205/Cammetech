using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace v3x.ViewModel
{
    public class SuperadminReport
    {
        public DateTime Date { get; set; }
        public double AdvancePay { get; set; }
        public double Bonus { get; set; }
        public double Basic { get; set; }
        public double NetSalary { get; set; }
        public double EmployeeEPF { get; set; }
        public double EmployerEPF { get; set; }
        public double EmployeeSocso { get; set; }
        public double EmployerSocso { get; set; }
        public string Name { get; set; }
    }
}
