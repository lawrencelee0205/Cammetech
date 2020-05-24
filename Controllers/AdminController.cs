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

namespace v3x.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly v3xContext _context;



        public AdminController(ILogger<AdminController> logger, v3xContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] List<Attendance> attendance)
        {
            var result = attendance.Count();
            var date_in_context = await _context.Attendance.Select(a => a.Date.ToShortDateString()).ToListAsync();
           
            foreach (var a in attendance)
            {

                if ( date_in_context.Contains(a.Date.AddDays(1).ToShortDateString()))
                {
                    DateTime dt;
                    var date = DateTime.TryParse(a.Date.AddDays(1).ToShortDateString(), out dt);
                    Debug.WriteLine("This date is already exists");
                    Debug.WriteLine($"Record {date} {a.Status} {a.EmployeeId}");
                    var exist_record = await _context.Attendance.FirstAsync(r => r.EmployeeId == a.EmployeeId && r.Date == dt);

                    exist_record.Status = a.Status;

                    
                    Debug.WriteLine($"Existing record {exist_record.EmployeeId} {exist_record.Date} {exist_record.Status}");
                }
                else 
                {
                    a.Date = a.Date.AddDays(1);
                    _context.Add(a);
                }
                await _context.SaveChangesAsync();
                a.Date = a.Date.AddDays(1);                
                
                
            }
            
            return Json(result);
        }

        public async Task<IActionResult> ManageSalary(int? id)
        {
            var job = await _context.Job.FirstOrDefaultAsync(j => j.PeopleId == id);
            var emp = await _context.People.FirstOrDefaultAsync(e => e.Id == id);

            ViewData["JobId"] = job.JobId;
            ViewData["EmpName"] = emp.Name;

            return View();
        }

        [HttpPost, ActionName("ManageSalary")]
        public async Task<IActionResult> ModifySalary([Bind("Date,Bonus,TotalRate,AdvancePay,EPFId,SocsoId,JobId")]SalaryModification salary)
        {
            _context.Add(salary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EmployeeTable));
        }

        public async Task<IActionResult> AttendanceTable()
        {
            IList<AttendanceTable> AttendanceBody = new List<AttendanceTable>();
            IDictionary<string, string> EmpStatus = new Dictionary<string, string>();
           
            var emp = (from p in _context.People
                       join a in _context.Attendance
                       on p.Id equals a.EmployeeId                       
                       select new { 
                            Name = p.Name,
                            Date = a.Date,
                            Status = a.Status
                       }).ToList();

            ViewData["EmpName"] =  _context.People
                                  .OrderBy(p => p.Id)
                                  .Where(p => p.Role == "employee")
                                  .Select(p => p.Name).ToList();

            List<DateTime> Unique_date = _context.Attendance.Select(e => e.Date).OrderBy(e => e.Date).Distinct().ToList();


            foreach(var date in Unique_date)
            {
                var current_attendance = emp.Where(e => e.Date.ToShortDateString() == date.ToShortDateString()).ToList();
                

                foreach (var a in current_attendance)
                {
                
                    EmpStatus.Add(a.Name,a.Status);                   
                }
                
                AttendanceBody.Add(new AttendanceTable() { Date = date.ToShortDateString(),Status = new Dictionary<string,string> (EmpStatus)});
                EmpStatus.Clear();
            }
            
            return View(AttendanceBody);
        }


        public IActionResult Attendance()
        {
            var emp = _context.People.Where(e => e.Role == "Employee");
            ViewData["Employee"] = emp.ToList();

            return View();
        }

        public IActionResult EmployeeTable()
        {
            var emp = _context.People.Where(e => e.Role == "Employee");
            ViewData["Employee"] = emp.ToList();

            return View();
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

            ViewData["EmpId"] = people.Id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id)
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

        public async Task<IActionResult> EmployeeDetails(int? id)
        {

            var emp = await _context.People.FirstOrDefaultAsync(e => e.Id == id && e.Role == "employee");
            var job = await _context.Job.FirstOrDefaultAsync(j => j.PeopleId == id);

            ViewData["BasePay"] = job.BasePay.ToString();
            ViewData["Position"] = job.Position.ToString();

            return View(emp);
        }

        public IActionResult AddEmp()
        {
            return View();
        }

        public async Task<IActionResult> DeleteEmp(int id)
        {
            var people = await _context.People.FindAsync(id);
            _context.People.Remove(people);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeTable));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Emp(string position, double basePay, string status, [Bind("Name,Password,Role,Tel,Email,Nationality,DateOfBirth,Address")] People people)
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

        private bool CheckExist(string Name)
        {
            var emp = _context.People.FirstOrDefault(e => e.Name == Name);



            if (emp != null)
            {

                return true;
            }

            return false;
        }
    }
}