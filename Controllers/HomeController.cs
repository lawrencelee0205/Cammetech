using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using v3x.Models;
using v3x.Data;

namespace v3x.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly  v3xContext _context;
        

        public HomeController(ILogger<HomeController> logger, v3xContext context)
        {
            _logger = logger;
            _context = context;

        }

        public async Task<IActionResult> IndexAsync()
        {
            if (HttpContext.Session.GetInt32("Session_Id").HasValue)
            {
                var people = await _context.People
                 .FirstOrDefaultAsync(p => p.Id == HttpContext.Session.GetInt32("Session_Id"));


                ReadData(people.Role);

                return View("Profile", people);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        
        [HttpPost]
        public async Task<IActionResult> Login(string Email,string Password)
        {
            var people = await _context.People
                 .FirstOrDefaultAsync(m => m.Email == Email && m.Password == Password);            

            HttpContext.Session.SetInt32("Session_Id", people.Id);
            HttpContext.Session.SetString("Session_Role", people.Role);

            ReadData(people.Role);


            return View("Profile",people);
        }

        private void ReadData(string role)
        {
            if (role == "admin")
            {
                var emp = _context.People.Where(i => i.Role == "employee");
                ViewData["Employee"] = emp.ToList();
            }

            if (role == "superadmin")
            {
                var admin = _context.People.Where(i => i.Role == "admin");
                ViewData["Admin"] = admin.ToList();
            }
        }

        public IActionResult TestPage()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

    }
}
