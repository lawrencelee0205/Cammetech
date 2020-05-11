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
            return View("Index");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Ad([Bind("Id,Name,Password,Role,Tel,Email")] People people)
        {
            if (CheckExist(people.Name))
            {
                return RedirectToAction(nameof(AdminTable));
            }

            if (ModelState.IsValid)
            {
                _context.Add(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminTable));
            }
            return RedirectToAction(nameof(AdminTable));
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