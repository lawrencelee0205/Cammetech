using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace v3x.Models
{
    public class PaySlipView
    {
        public SelectList Date { get; set; }
        public SelectList EmpName { get; set; }
        public string paySlipDate { get; set; }
        public string Name { get; set; }
        public PaySlip PaySlip { get; set; }

    }
}
