using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using v3x.Models;
using v3x.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace v3x.Controllers
{
    public class SuperadminController : Controller
    {
        private readonly ILogger<SuperadminController> _logger;
        private readonly v3xContext _context;

        public SuperadminController(ILogger<SuperadminController> logger, v3xContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AttendanceTable()
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
        public async Task<IActionResult> AdminDetails(int? id)
        {
                var admin = await _context.People.FirstOrDefaultAsync(e => e.Id == id && e.Role == "admin");
                            
                return View(admin);         
        }

        public IActionResult AdminTable()
        {
            var admin = _context.People.Where(a => a.Role == "Admin");
            ViewData["Admin"] = admin.ToList();

            return View();
        }

        public IActionResult AddAdmin()
        {
            return View();
        }
        
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var people = await _context.People.FindAsync(id);
            _context.People.Remove(people);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminTable));
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Ad([Bind("Id,Name,Password,Role,Tel,Email,Nationality,DateOfBirth,Address")] People people)
        {
            Debug.WriteLine($"Value: {people.Name}, {people.Email}");
            if (CheckExist(people.Name))
            {
                Debug.WriteLine("This run");
                return RedirectToAction(nameof(AdminTable));
            }

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model valid");
                _context.Add(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminTable));
            }
            return RedirectToAction(nameof(AdminTable));
        }

        private bool CheckExist(string Name)
        {
            var admin = _context.People.FirstOrDefault(a => a.Name == Name);

            if (admin != null)
            {
                return true;
            }

            return false;
        }

        public async Task<IActionResult> UpdateAdmin(int? id)
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

                if (id == null)
                {
                    return NotFound();
                }
                var adminToUpdate = await _context.People.FirstOrDefaultAsync(a => a.Id == id);
                if (await TryUpdateModelAsync<People>(
                    adminToUpdate,
                    "",
                    e => e.Tel, e => e.Email, e => e.Nationality, e => e.Address, e => e.DateOfBirth))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(AdminTable));
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                    }
                }
                return View("UpdateEmployee", adminToUpdate);

        }
    }
}