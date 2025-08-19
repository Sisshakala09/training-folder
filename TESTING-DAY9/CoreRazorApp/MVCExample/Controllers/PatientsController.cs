using Microsoft.AspNetCore.Mvc;
using MVCExample.Data;
using MVCExample.Models;

namespace MVCExample.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crete(string Name,string Allergies)
        {
            Patient p = new Patient();
            p.Allergies = Allergies;
            p.Name = Name;
            _context.patients.Add(p);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> GetPatients()
        {
            return View();
        }
    }
}
