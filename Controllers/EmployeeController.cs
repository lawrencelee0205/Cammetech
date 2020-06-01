using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using v3x.Data;
using v3x.Models;

namespace v3x.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly v3xContext _context;

        public EmployeeController(ILogger<EmployeeController> logger, v3xContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewAttendance(DateTime startDate, DateTime endDate)
        {
            //Useful for debugging (But to use this u must run F5)
            Debug.WriteLine($"Start date : {startDate}");
            Debug.WriteLine($"End date : {endDate}");

            //Retreive use id
            var empId = HttpContext.Session.GetInt32("Session_Id");

            //Retrive data 
            var emp = await _context.Attendance.Where(e => e.EmployeeId == empId).ToListAsync();



            //Declare model to hold the retrieved data
            IList<EmployeeAttendanceView> view = new List<EmployeeAttendanceView>();

            //Check whether user got choose date or not
            //Default startDate is 1/1/0001
            //If we don't do this, it will compare the default date to your DB data, backend will loop and consume your time to load the page.
            //So, we only want to compare the chosen date instead of default date.
            if (startDate.ToString("M/d/yyyy") != "1/1/0001")
            {
                DateTime firstdate = emp[0].Date;
                int res1 = DateTime.Compare(startDate, firstdate);
                int res2 = DateTime.Compare(endDate, firstdate);
                int res = DateTime.Compare(startDate, endDate);

                //First condition
                if (res1 < 0 && res2 < 0)//Check whether the startDate and endDate is located between the existing date or not
                {
                    ViewBag.message = "Not found";
                }
                else if (res > 0)
                {
                    ViewBag.message = "Invalid range";
                }
                else
                {
                    //Loop each data in emp into view
                    foreach (var e in emp)
                    {
                        DateTime date = e.Date;
                        string status = e.Status;
                        int res3 = DateTime.Compare(startDate, date);
                        int res4 = DateTime.Compare(startDate, endDate);
                        //second condition
                        if (res3 <= 0 && res4 <= 0)//Check whether the startDate is smaller than the first e.Date
                        {
                            while (res3 < 0)//If startDate smaller than the first e.Date, loop the startDate to match the first e.Date
                            {
                                startDate = startDate.AddDays(1);
                                res3 = DateTime.Compare(startDate, date);
                            }
                            view.Add(new EmployeeAttendanceView { Date = date.ToString("M/d/yyyy"), Status = status });  //If startDate <= e.Date, then add it into view.
                            startDate = startDate.AddDays(1);

                        }
                        //Third condition
                        else if (res3 >= 0 && res4 <= 0) //If startDate >= e.Date and startDate <= endDate
                        {
                            //Fourth condition
                            if (startDate == date)//If startDate == e.Date, then add it into view.Else, we do nothing (No need declare 'else')
                            {
                                view.Add(new EmployeeAttendanceView { Date = date.ToString("M/d/yyyy"), Status = status });
                                startDate = startDate.AddDays(1);
                            }
                        }
                    }
                }
            }

            //return the model to the view
            return View(view);
        }

    }
}