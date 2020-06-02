using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using v3x.Models;
using System.ComponentModel.DataAnnotations;


namespace v3x.ViewModel
{
    public class JobViewModel : Models.Job
    {
        public string Name { get; set; }
        public People People { get; set; }
    }
}
