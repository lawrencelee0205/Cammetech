using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using v3x.Models;
using v3x.Data;
using SelectPdf;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace v3x.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly v3xContext _context;
        const string veryrole = "EMPLOYEE";

        public EmployeeController(ILogger<EmployeeController> logger, v3xContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult PaySlip(string paySlipDate)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var paySlipView = new PaySlipView();

                if (paySlipDate != null)
                {
                    var parsedDate = DateTime.ParseExact(paySlipDate, "MMMM yyyy", CultureInfo.InvariantCulture);
                    var paySlip = _context.PaySlip
                        .Include(x => x.Job)
                        .Include(x => x.Job.People)
                        .Where(x => x.Job.People.Name == HttpContext.Session.GetString("Session_Name"))
                        .Where(x => x.Date.Year == parsedDate.Year && x.Date.Month == parsedDate.Month)
                        .FirstOrDefault();

                    //var empId = _context.People.Where(p => p.Name == Name).Select(e => e.Id).FirstOrDefault();
                    //var jobId = _context.Job.Where(j => j.PeopleId == empId).Select(j => j.JobId).FirstOrDefault();
                    //var paySlip = _context.PaySlip
                    //    .Where(ps => ps.JobId == jobId)
                    //    .Where(ps => ps.Date.ToString("MMMM yyyy") == paySlipDate)
                    //    .FirstOrDefault();
                    var date = _context.PaySlip.Select(d => d.Date.ToString("MMMM yyyy")).Distinct();
                    var empName = _context.People.Where(e => e.Role == "employee").Select(e => e.Name);
                    paySlipView = new PaySlipView
                    {
                        PaySlip = paySlip,
                        Date = new SelectList(date.ToList()),
                        EmpName = new SelectList(empName.ToList()),
                        Name = paySlip.Job.People.Name,
                        paySlipDate = paySlip.Date.ToString("MMMM yyyy")
                    };

                }
                else
                {
                    var date = _context.PaySlip.Select(d => d.Date.ToString("MMMM yyyy")).Distinct();

                    paySlipView = new PaySlipView
                    {
                        Date = new SelectList(date.ToList())
                    };
                }
                return View(paySlipView);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult PdfPaySlip(PaySlipView model)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var name = model.PaySlip.Job.People.Name;
                var position = model.PaySlip.Job.Position;
                var address = model.PaySlip.Job.People.Address;
                var basic = model.PaySlip.Basic;
                var employeeepf = model.PaySlip.EmployeeEPF;
                var bonus = model.PaySlip.Bonus;
                var employerepf = model.PaySlip.EmployerEPF;
                var Advancepay = model.PaySlip.AdvancePay;
                var employeesocso = model.PaySlip.EmployeeSocso;
                var net = model.PaySlip.NetSalary;
                var date = model.PaySlip.Date.ToString("MMMM yyyy");


                var HtmlString = "<!DOCTYPE html><html lang='en'>" +
                    "<head><meta charset='utf - 8' />" +
                    "<meta name = 'viewport' content = 'width=device-width, initial-scale=1.0' />" +
                    "<link href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>" +
                    "</head>" +
                    "<body>" +
                    "<br><br><br><br><br><br>" +
                    "<div>" +
                    "<div class='card-header text-center'>" +
                    "<b>V3X Company</b>" +
                    "</div>" +
                    "<div class='card-body'>" +
                    "<div class='text-center'>" +
                    "<b>" + date + "</b>" +
                    "</div>" +
                    "<table class='table'>" +
                    "<tbody>" +
                    "<tr><th>Name:</th>" +
                    "<th><th>" + name + "</th><th>" +
                    "</th></tr><tr><th>Position:</th><th></th><th>" + position + "</th><th></th></tr><th>Address:</th><th></th><th>" + address + "</th><th></th></tr><tr><th>Basic:</th><th>RM" + basic + "</th><th>EPF:</th><th>RM" + employeeepf + "</th></tr><tr><th>Bonus:</th><th>RM" + bonus + "</th><th>CompanyEPF:</th><th>RM" + employerepf + "</th></tr><tr><th>AdvancePay:</th><th>RM" + Advancepay + "</th><th>SOCSO:</th><th>RM" + employeesocso + "</th></tr><tr><th>NetSalary:</th><th></th><th>RM" + net + "</th><th></th></tr></tbody></table></div></div></body></html>";

                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertHtmlString(HtmlString);
                MemoryStream pdfStream = new MemoryStream();


                doc.Save(pdfStream);
                pdfStream.Position = 0;
                doc.Close();
                return File(pdfStream.ToArray(), "application/pdf");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Attendance()
        {
            return View("ViewAttendance");
        }


        public async Task<IActionResult> ViewAttendance(DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {

                //Retreive use id
                var empId = HttpContext.Session.GetInt32("Session_Id");

                //Retrive relevant data 
                var emp = await _context.Attendance.Where(e => e.EmployeeId == empId).ToListAsync();

                //Declare model to hold the retrieved data
                IList<EmployeeAttendanceView> view = new List<EmployeeAttendanceView>();

                //Check whether user got choose date or not
                //Default startDate is 1/1/0001
                //If we don't do this, it will compare the default date to your DB data, 
                //backend will loop and consume your time to load the page.
                //So, we only want to compare the chosen date instead of default date.
                if (startDate.ToString("M/d/yyyy") != "1/1/0001")
                {
                    Debug.WriteLine($"Range: { startDate <= endDate }");
                    DateTime firstDate = emp[0].Date;
                    DateTime lastDate = emp[emp.Count - 1].Date.Date;

                    Debug.WriteLine($"EF: { endDate < firstDate }");
                    Debug.WriteLine($"SL: { startDate > lastDate }");

                    if (startDate > endDate)
                    {
                        ViewBag.message = "Invalid range";
                    }
                    else if((startDate <= endDate) && ( (endDate < firstDate) || (startDate > lastDate ) ))
                    {
                        ViewBag.message = "Out of range";
                    }
                    else
                    {
                        int startIndex = 0;
                        int endIndex = 0;

                        if (startDate > firstDate)
                        {
                            startIndex = emp.IndexOf(emp.Find(e => e.Date.Date == startDate.Date));
                        }
                        

                        if(endDate >= lastDate)
                        {
                            endIndex = emp.Count - 1;
                        }
                        else
                        {
                            endIndex = emp.IndexOf(emp.Find(e => e.Date.Date == endDate.Date));
                        }

                        for (int x = startIndex; x <= endIndex; x++)
                        {
                            var e = emp[x];
                            view.Add(new EmployeeAttendanceView { Date = e.Date.ToShortDateString(), Status = e.Status });
                        }
                    }
                }

                //return the model to the view
                return View(view);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}