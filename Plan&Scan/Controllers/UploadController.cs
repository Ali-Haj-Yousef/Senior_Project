using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using Plan_Scan.Data;
using Plan_Scan.Models;

namespace Plan_Scan.Controllers
{
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public UploadController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public List<StudentExamRegistration> ReadExcelData(string filePath)
        {
            var registrations = new List<StudentExamRegistration>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Get the first worksheet
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Assuming the first row is headers
                {
                    var registration = new StudentExamRegistration
                    {
                        StudentId = int.Parse(worksheet.Cells[row, 1].Text),
                        Name = worksheet.Cells[row, 2].Text,
                        Course = worksheet.Cells[row, 3].Text,
                        Lang = char.Parse(worksheet.Cells[row, 4].Text),
                        Room = worksheet.Cells[row, 5].Text,
                        SeatNb = int.Parse(worksheet.Cells[row, 6].Text),
                        Date = DateOnly.Parse(worksheet.Cells[row, 7].Text),
                        Time = TimeOnly.FromDateTime(worksheet.Cells[row, 8].GetValue<DateTime>()),
                        ExamCode = worksheet.Cells[row, 9].Text
                        // Map other properties accordingly
                    };
                    registrations.Add(registration);
                }
            }

            return registrations;
        }
        public void InsertDataToDatabase(string excelFilePath)
        {
            var registrations = ReadExcelData(excelFilePath);
            foreach (var registration in registrations)
            {
                // Add the registration to the context
                _context.StudentExamRegistrations.Add(registration);
            }

            // Save changes to the database
            _context.SaveChanges();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath; // this is the path of wwwroot folder
            if (file != null)
            {
                /* When the file is uploaded, file might have some weird name. 
                   So rather than keeping the same name, we can rename that to be a random guid
                */
                string fileName = Path.GetFileName(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"excel");
                // obtaining the path of "product" directory inside wwwroot foler where we have to save the image file  

                // Creating the file using FileStream Class
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                    string filePath = Path.Combine(productPath, fileName);
                    InsertDataToDatabase(filePath);
                }
                // using statement is used to automatically close the file or any resource once we have done with it, even if an error occurs
            }
            return View();
        }
    }
}
