using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan_Scan.Data;
using Plan_Scan.Models;

namespace Plan_Scan.Controllers
{
    public class ScannerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ScannerController(ApplicationDbContext db)
        {
            _context = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(FileUploadViewModel? fileUploadViewModel)
        {
            // Check if the file is uploaded
            if (fileUploadViewModel?.File != null)
            {
                var extension = Path.GetExtension(fileUploadViewModel.File.FileName);
                if (extension != ".jpg")
                {
                    ModelState.AddModelError("File", "The file must be an Image file (.jpg).");
                }
            }

            // If ModelState is valid, proceed with the database operations
            if (ModelState.IsValid)
            {
                
                return RedirectToAction("Index");
            }

            // Return the view with the current ModelState if validation fails
            return View();
        }
    }
}
