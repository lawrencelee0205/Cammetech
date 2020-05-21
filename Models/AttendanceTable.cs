using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace v3x.Models
{
    public class AttendanceTable
    {
        public string Date { get; set; }
        public  IDictionary<string, string> Status { get; set; }

    }
}
