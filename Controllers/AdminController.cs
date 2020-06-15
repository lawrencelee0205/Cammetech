using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using v3x.Models;
using v3x.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using SelectPdf;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace v3x.Controllers
{
    public class AdminController : Controller
    {
        const string veryrole = "ADMIN";
        private readonly ILogger<AdminController> _logger;
        private readonly v3xContext _context;



        public AdminController(ILogger<AdminController> logger, v3xContext context)
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

        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] List<Attendance> attendance)
        {
            var result = attendance.Count();
            var attendance_record = _context.Attendance.Where(e => e.EmployeeId == attendance[0].EmployeeId);
            var date = attendance_record.Select(a => a.Date.ToShortDateString()).ToList();
            var empId = attendance[0].EmployeeId;

            foreach (var a in attendance)
            {
                var input_date = a.Date.AddDays(1).Date;
                if(date.Contains(input_date.ToShortDateString()))
                {
                    Debug.WriteLine($"This part run {input_date}");
                    var update_row = await attendance_record.FirstAsync(r => r.Date.Date == input_date);
                    update_row.Status = a.Status;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    a.Date = a.Date.AddDays(1);
                    _context.Add(a);
                    await _context.SaveChangesAsync();
                }
                
            }

            return Json(result);
        }


        public async Task<IActionResult> AttendanceTable()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                IList<AttendanceTable> AttendanceBody = new List<AttendanceTable>();
            IDictionary<string, string> EmpStatus = new Dictionary<string, string>();

            var emp = (from p in _context.People
                       join a in _context.Attendance
                       on p.Id equals a.EmployeeId
                       select new
                       {
                           Name = p.Name,
                           Date = a.Date,
                           Status = a.Status
                       }).ToList();

            ViewData["EmpName"] = _context.People
                                  .OrderBy(p => p.Id)
                                  .Where(p => p.Role == "employee")
                                  .Select(p => p.Name).ToList();

            List<DateTime> Unique_date = _context.Attendance.Select(e => e.Date).OrderBy(e => e.Date).Distinct().ToList();


            foreach (var date in Unique_date)
            {
                var current_attendance = emp.Where(e => e.Date.ToShortDateString() == date.ToShortDateString()).ToList();


                foreach (var a in current_attendance)
                {

                    EmpStatus.Add(a.Name, a.Status);
                }

                AttendanceBody.Add(new AttendanceTable() { Date = date.ToShortDateString(), Status = new Dictionary<string, string>(EmpStatus) });
                EmpStatus.Clear();
            }

            return View(AttendanceBody);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult Attendance()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var emp = _context.People.Where(e => e.Role == "Employee");
                ViewData["Employee"] = emp.ToList();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EmployeeTable()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var emp = _context.People.Where(e => e.Role == "Employee");

                ViewData["Employee"] = emp.ToList();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> UpdateEmployee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.People.FindAsync(id);

            if (people == null)
            {
                return NotFound();
            }

            //ViewData["EmpId"] = people.Id;

            return View(people);
        }

        //Post for updating employee
        [HttpPost]
        public async Task<IActionResult> Update(int? id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var empToUpdate = await _context.People.FirstOrDefaultAsync(e => e.Id == id);
                if (await TryUpdateModelAsync<People>(
                    empToUpdate,
                    "",
                    e => e.Tel, e => e.Email, e => e.Nationality, e => e.Address, e => e.DateOfBirth))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(EmployeeTable));
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                    }
                }
                return View("UpdateEmployee", empToUpdate);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> EmployeeDetails(int? id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var emp = await _context.People.FirstOrDefaultAsync(e => e.Id == id && e.Role == "employee");
                var job = await _context.Job.FirstOrDefaultAsync(j => j.PeopleId == id);

                ViewData["BasePay"] = job.BasePay.ToString();
                ViewData["Position"] = job.Position.ToString();

                return View(emp);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult AddEmp()
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

        public async Task<IActionResult> DeleteEmp(int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var people = await _context.People.FindAsync(id);
                _context.People.Remove(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EmployeeTable));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Emp(string position, double basePay, string status, [Bind("Name,Password,Role,Tel,Email,Nationality,DateOfBirth,Address")] People people)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                Debug.WriteLine($"Value : {position} {basePay} {status} {people.Name}");

                if (CheckExist(people.Name))
                {
                    Debug.WriteLine("This run");
                    return RedirectToAction(nameof(EmployeeTable));
                }



                _context.Add(people);
                await _context.SaveChangesAsync();

                var emp = await _context.People.FirstOrDefaultAsync(e => e.Name == people.Name);

                Debug.WriteLine($"Emp Id: {emp.Id}");
                var job = new Job
                {

                    Position = position,
                    BasePay = basePay,
                    Status = status,
                    PeopleId = emp.Id
                };
                _context.Add(job);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(EmployeeTable));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }



        }

        private bool CheckExist(string Name)
        {
            var emp = _context.People.FirstOrDefault(e => e.Name == Name);



            if (emp != null)
            {

                return true;
            }

            return false;
        }
        //This one is display only not function
        public IActionResult DoSalary()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                //Choosing Job and then select job entity then create a new payslip that connect from job 
                var emp = _context.Job
                .Include(x => x.People)
                .Select(j => new PaySlip
                {
                    Basic = j.BasePay ,
                    //Date = new DateTime(2019, 3, 1, 7, 0, 0),
                    Date = DateTime.Now,
                    Job = j,
                    JobId = j.JobId
                }).ToList();

                return View(emp);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Payroll(List<v3x.Models.PaySlip> model)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                foreach (var item in model)
                {
                    item.Job = _context.Job.Where(x => x.JobId == item.JobId).Include(y => y.People).FirstOrDefault();
                }
                model = CalAllPaySlip(model);

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private List<v3x.Models.PaySlip> CalAllPaySlip(List<v3x.Models.PaySlip> model)
        {
            var result = new List<PaySlip>();

            foreach (var item in model)
            {
                result.Add(CalSinglePaySlip(item));
            }


            return result;
        }

        private PaySlip CalSinglePaySlip(PaySlip pay)
        {
            var total = pay.Basic + pay.Bonus + pay.AdvancePay;
            Tuple<double, double> EPF = CalEPF(total);
            Tuple<double, double> SOCSO = CalSocso(total);
            double NetSalary = total - EPF.Item1 - SOCSO.Item1;

            pay.EmployeeEPF = EPF.Item1;
            pay.EmployerEPF = EPF.Item2;
            pay.EmployeeSocso = SOCSO.Item1;
            pay.EmployerSocso = SOCSO.Item2;
            pay.NetSalary = NetSalary;

            return pay;
        }

        private Tuple<double, double> CalEPF(double total)
        {
            double employee = total * 0.07;
            double employer = 0;
            if (total <= 5000)
            {
                employer = total * 0.13;
            }
            else
            {
                employer = total * 0.12;
            }
            return new Tuple<double, double>(employee, employer);
        }

        private Tuple<double, double> CalSocso(double total)
        {
            //var list = new List<string>();
            //list
            double employee = 0;
            double employer = 0;
            if (total > 0 && total <= 30)
            {
                employee = 0.1;
                employer = 0.4;
            }
            else if (total > 30 && total <= 50)
            {
                employee = 0.2;
                employer = 0.7;
            }
            else if (total > 50 && total <= 70)
            {
                employee = 0.3;
                employer = 1.1;
            }
            else if (total > 70 && total <= 100)
            {
                employee = 0.4;
                employer = 1.5;
            }
            else if (total > 100 && total <= 140)
            {
                employee = 0.6;
                employer = 2.1;
            }
            else if (total > 140 && total <= 200)
            {
                employee = 0.85;
                employer = 2.95;
            }
            else if (total > 200 && total <= 300)
            {
                employee = 1.25;
                employer = 4.35;
            }
            else if (total > 300 && total <= 400)
            {
                employee = 1.75;
                employer = 6.15;
            }
            else if (total > 400 && total <= 500)
            {
                employee = 2.25;
                employer = 7.85;
            }
            else if (total > 500 && total <= 600)
            {
                employee = 2.75;
                employer = 9.65;
            }
            else if (total > 600 && total <= 700)
            {
                employee = 3.25;
                employer = 11.35;
            }
            else if (total > 700 && total <= 800)
            {
                employee = 3.75;
                employer = 13.15;
            }
            else if (total > 800 && total <= 900)
            {
                employee = 4.25;
                employer = 14.85;
            }
            else if (total > 900 && total <= 1000)
            {
                employee = 4.75;
                employer = 16.65;
            }
            else if (total > 1000 && total <= 1100)
            {
                employee = 5.25;
                employer = 18.35;
            }
            else if (total > 1100 && total <= 1200)
            {
                employee = 5.75;
                employer = 20.15;
            }
            else if (total > 1200 && total <= 1300)
            {
                employee = 6.25;
                employer = 21.85;
            }
            else if (total > 1300 && total <= 1400)
            {
                employee = 6.75;
                employer = 23.65;
            }
            else if (total > 1400 && total <= 1500)
            {
                employee = 7.25;
                employer = 25.35;
            }
            else if (total > 1500 && total <= 1600)
            {
                employee = 7.75;
                employer = 27.15;
            }
            else if (total > 1600 && total <= 1700)
            {
                employee = 8.25;
                employer = 28.85;
            }
            else if (total > 1700 && total <= 1800)
            {
                employee = 8.75;
                employer = 30.65;
            }
            else if (total > 1800 && total <= 1900)
            {
                employee = 9.25;
                employer = 32.35;
            }
            else if (total > 1900 && total <= 2000)
            {
                employee = 9.75;
                employer = 34.15;
            }
            else if (total > 2000 && total <= 2100)
            {
                employee = 10.25;
                employer = 35.85;
            }
            else if (total > 2100 && total <= 2200)
            {
                employee = 10.75;
                employer = 37.65;
            }
            else if (total > 2200 && total <= 2300)
            {
                employee = 11.25;
                employer = 39.35;
            }
            else if (total > 2300 && total <= 2400)
            {
                employee = 11.75;
                employer = 41.15;
            }
            else if (total > 2400 && total <= 2500)
            {
                employee = 12.25;
                employer = 42.85;
            }
            else if (total > 2500 && total <= 2600)
            {
                employee = 12.75;
                employer = 44.65;
            }
            else if (total > 2600 && total <= 2700)
            {
                employee = 13.25;
                employer = 46.35;
            }
            else if (total > 2700 && total <= 2800)
            {
                employee = 13.75;
                employer = 48.15;
            }
            else if (total > 2800 && total <= 2900)
            {
                employee = 14.25;
                employer = 49.85;
            }
            else if (total > 2900 && total <= 3000)
            {
                employee = 14.75;
                employer = 51.65;
            }
            else if (total > 3000 && total <= 3100)
            {
                employee = 15.25;
                employer = 53.35;
            }
            else if (total > 3100 && total <= 3200)
            {
                employee = 15.75;
                employer = 55.15;
            }
            else if (total > 3200 && total <= 3300)
            {
                employee = 16.25;
                employer = 56.85;
            }
            else if (total > 3300 && total <= 3400)
            {
                employee = 16.75;
                employer = 58.65;
            }
            else if (total > 3400 && total <= 3500)
            {
                employee = 17.25;
                employer = 60.35;
            }
            else if (total > 3500 && total <= 3600)
            {
                employee = 17.75;
                employer = 62.15;
            }
            else if (total > 3600 && total <= 3700)
            {
                employee = 18.25;
                employer = 63.85;
            }
            else if (total > 3700 && total <= 3800)
            {
                employee = 18.75;
                employer = 65.65;
            }
            else if (total > 3800 && total <= 3900)
            {
                employee = 19.25;
                employer = 67.35;
            }
            else if (total > 3900 && total <= 4000)
            {
                employee = 19.75;
                employer = 69.05;
            }
            else
            {
                employee = 19.75;
                employer = 69.05;
            }
            return new Tuple<double, double>(employee, employer);
        }

        public IActionResult PaySave(List<v3x.Models.PaySlip> model)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var existDates = _context.PaySlip.Select(d => d.Date.ToString("MMMM yyyy")).Distinct().ToList();
                var anyDate = model.FirstOrDefault().Date.ToString("MMMM yyyy");

                if (existDates.Where(x => x == anyDate).FirstOrDefault() != null)
                {
                    // redirect
                    return RedirectToAction(nameof(Index));
                }

                foreach (var payslip in model)
                {
                    _context.Add(payslip);
                    _context.SaveChanges();
                }

                //IDictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
                //var JobId =  _context.Job.Select(j => j.JobId).ToList();
                //int yes = 0;
                //foreach (var item in model)
                //{
                //    item.Job = _context.Job.Where(x => x.JobId == item.JobId).Include(y => y.People).FirstOrDefault();
                //    var verifydate = item.Date.ToString("MMMM yyyy");
                //    foreach (var c in JobId)
                //    {
                //        if(item.JobId == c)
                //        {
                //            var getdate = _context.PaySlip.Where(x => x.JobId == c).Select(j => j.Date.ToString("MMMM yyyy")).ToList();
                //            foreach(var d in getdate)
                //            {
                //                if(verifydate == d)
                //                {
                //                    yes = 1;
                //                }
                //                else
                //                {
                //                    yes = 0;
                //                }
                //                if(yes == 1)
                //                {
                //                    return RedirectToAction(nameof(Index));
                //                }

                //            }
                //        }

                //    }

                //    _context.Add(item);
                //    _context.SaveChanges();
                //}

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult PaySlip(String Name, string paySlipDate)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                Debug.WriteLine($"My input : {paySlipDate} {Name}");
                var paySlipView = new PaySlipView();

                if (paySlipDate != null && Name != null)
                {
                    var parsedDate = DateTime.ParseExact(paySlipDate, "MMMM yyyy", CultureInfo.InvariantCulture);
                    var paySlip = _context.PaySlip
                        .Include(x => x.Job)
                        .Include(x => x.Job.People)
                        .Where(x => x.Job.People.Name == Name)
                        .Where(x => x.Date.Year == parsedDate.Year && x.Date.Month == parsedDate.Month)
                        .FirstOrDefault();

                    paySlip.NetSalary = Math.Round(paySlip.NetSalary,2);
                    paySlip.Basic = Math.Round(paySlip.Basic, 2);
                    paySlip.EmployeeEPF = Math.Round(paySlip.EmployeeEPF, 2);
                    paySlip.EmployerEPF = Math.Round(paySlip.EmployerEPF, 2);
                    paySlip.Bonus = Math.Round(paySlip.Bonus, 2);
                    paySlip.AdvancePay = Math.Round(paySlip.AdvancePay, 2);
                    paySlip.EmployeeSocso = Math.Round(paySlip.EmployeeSocso, 2);
                    paySlip.EmployerSocso = Math.Round(paySlip.EmployerSocso, 2);

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
                    var empName = _context.People.Where(e => e.Role == "employee").Select(e => e.Name);

                    paySlipView = new PaySlipView
                    {
                        Date = new SelectList(date.ToList()),
                        EmpName = new SelectList(empName.ToList())
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


                var HtmlString = "<!DOCTYPE html><html lang='en'><head><meta charset='utf - 8' /><meta name = 'viewport' content = 'width=device-width, initial-scale=1.0' /><link href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'></head><body><br><br><br><br><br><br><div><div class='card-header text-center'><b>V3X Company</b></div><div class='card-body'><div class='text-center'><b>" + date + "</b></div><table class='table'><tbody><tr><th>Name:</th><th><th>" + name + "</th><th></th></tr><tr><th>Position:</th><th></th><th>" + position + "</th><th></th></tr><th>Address:</th><th></th><th>" + address + "</th><th></th></tr><tr><th>Basic:</th><th>RM" + basic + "</th><th>EPF:</th><th>RM" + employeeepf + "</th></tr><tr><th>Bonus:</th><th>RM" + bonus + "</th><th>CompanyEPF:</th><th>RM" + employerepf + "</th></tr><tr><th>AdvancePay:</th><th>RM" + Advancepay + "</th><th>SOCSO:</th><th>RM" + employeesocso + "</th></tr><tr><th>NetSalary:</th><th></th><th>RM" + net + "</th><th></th></tr></tbody></table></div></div></body></html>";

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

        public IActionResult EmailPeople(PaySlipView model)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var id = model.PaySlip.Id;
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
                var email = model.PaySlip.Job.People.Email;


                var HtmlString = "<!DOCTYPE html><html lang='en'><head><meta charset='utf - 8' /><meta name = 'viewport' content = 'width=device-width, initial-scale=1.0' /><link href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'></head><body><br><br><br><br><br><br><div><div class='card-header text-center'><b>V3X Company</b></div><div class='card-body'><div class='text-center'><b>" + date + "</b></div><table class='table'><tbody><tr><th>Name:</th><th><th>" + name + "</th><th></th></tr><tr><th>Position:</th><th></th><th>" + position + "</th><th></th></tr><th>Address:</th><th></th><th>" + address + "</th><th></th></tr><tr><th>Basic:</th><th>RM" + basic + "</th><th>EPF:</th><th>RM" + employeeepf + "</th></tr><tr><th>Bonus:</th><th>RM" + bonus + "</th><th>CompanyEPF:</th><th>RM" + employerepf + "</th></tr><tr><th>AdvancePay:</th><th>RM" + Advancepay + "</th><th>SOCSO:</th><th>RM" + employeesocso + "</th></tr><tr><th>NetSalary:</th><th></th><th>RM" + net + "</th><th></th></tr></tbody></table></div></div></body></html>";

                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertHtmlString(HtmlString);
                MemoryStream pdfStream = new MemoryStream();

                doc.Save(pdfStream);

                pdfStream.Position = 0;

                // create email message
                MailMessage message = new MailMessage("sulphate1997@gmail.com", email);
                //message.From = new MailAddress("otj_1997@hotmail.com");
                //message.To.Add(new MailAddress(email));
                message.Subject = "Payroll";
                message.Body = "The following attachment is payslip for the payroll";
                message.Attachments.Add(new Attachment(pdfStream, "PaySlip" + name + id + ".pdf"));

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential("sulphate1997@gmail.com", "noob1997");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;

                // send email
                smtp.Send(message);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}