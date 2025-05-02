using System.Reflection.Metadata;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Plan_Scan.Data;
using Plan_Scan.Models;
using Document = iTextSharp.text.Document;

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
        public IActionResult PDF()
        {
            using (MemoryStream ms  = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Create a table for the header
                PdfPTable header = new PdfPTable(3);
                header.WidthPercentage = 100;

                // Left cell
                PdfPCell LU = new PdfPCell(new Phrase("Lebanese University", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                LU.Border = PdfPCell.NO_BORDER;
                LU.HorizontalAlignment = Element.ALIGN_LEFT;
                header.AddCell(LU);

                // Center cell
                PdfPCell title = new PdfPCell(new Phrase("Students Attendance Sheet", new Font(Font.FontFamily.HELVETICA, 10)));
                title.Border = PdfPCell.NO_BORDER;
                title.HorizontalAlignment = Element.ALIGN_CENTER;
                header.AddCell(title);

                // Right cell
                PdfPCell date = new PdfPCell(new Phrase("Date: 23/6/2025", new Font(Font.FontFamily.HELVETICA, 10)));
                date.Border = PdfPCell.NO_BORDER;
                date.HorizontalAlignment = Element.ALIGN_RIGHT;
                header.AddCell(date);

                PdfPCell FS = new PdfPCell(new Phrase("Faculty of Science", new Font(Font.FontFamily.HELVETICA, 10)));
                FS.Border = PdfPCell.NO_BORDER;
                FS.HorizontalAlignment = Element.ALIGN_LEFT;
                header.AddCell(FS);

                PdfPCell e1 = new PdfPCell();
                e1.Border = PdfPCell.NO_BORDER;
                header.AddCell(e1);

                PdfPCell e2 = new PdfPCell();
                e2.Border = PdfPCell.NO_BORDER;
                header.AddCell(e2);

                // Add header table to the document
                document.Add(header);

                // Add some spacing
                document.Add(new Paragraph("\n\n\n"));

                PdfPTable table = new PdfPTable(10);
                table.WidthPercentage = 100;

                table.AddCell(new PdfPCell(new Phrase("Student ID", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Name", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Course", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Language", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Room", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Seat Number", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Date", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Time", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Code Exam Day", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Presence", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });

                List<StudentExamRegistration> registrations = _context.StudentExamRegistrations.ToList();

                PdfPTable sigField = new PdfPTable(1);
                sigField.AddCell(new PdfPCell(new Phrase(" ")));
                foreach (var item in registrations)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.StudentId.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Name, new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Course, new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Lang.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Room, new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.SeatNb.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Date.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Time.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.ExamCode, new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(sigField){ Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });
                }
                document.Add(table);
                document.Close();
                writer.Close();
                var constant = ms.ToArray();
                return File(constant, "application/vnd", "firstPdf.pdf");
            }
        }
    }
}
