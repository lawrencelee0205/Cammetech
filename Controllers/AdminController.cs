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

            foreach(var a in attendance)
            {
                _context.Add(a);
                await _context.SaveChangesAsync();                
            }

            return Json(result);
        }

        public async Task<IActionResult> AttendanceList()
        {

            return View(await _context.Attendance.ToListAsync());
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
        public async Task<IActionResult> Create_Emp([Bind("Id,Name,Password,Role,Tel,Email")] People people)
        {
            if (CheckExist(people.Name))
            {
                return RedirectToAction(nameof(EmployeeTable));
            }

            if (ModelState.IsValid)
            {
                _context.Add(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EmployeeTable));
            }

            return RedirectToAction(nameof(EmployeeTable));
        }

        private bool CheckExist(string Name)
        {
            var emp = _context.People.Where(e => e.Name == Name);

            if (emp != null)
            {
                return true;
            }

            return false;
        }
    }
}