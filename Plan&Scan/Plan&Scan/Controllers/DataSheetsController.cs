using Microsoft.AspNetCore.Mvc;
using Plan_Scan.Data;
using Plan_Scan.Models;

namespace Plan_Scan.Controllers
{
    public class DataSheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DataSheetsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<StudentExamRegistration> registrations = _context.StudentExamRegistrations.ToList();
            return View(registrations);
        }
    }
}
