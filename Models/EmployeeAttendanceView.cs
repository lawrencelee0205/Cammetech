using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//This is for view only 
namespace v3x.Models
{
    public class EmployeeAttendanceView
    {
        //declare the property u want to view it in the view.cshtml
        public string Date { get; set; }

        public string Status { get; set; }


    }
}