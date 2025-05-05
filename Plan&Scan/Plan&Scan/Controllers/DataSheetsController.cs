using System.Diagnostics;
using System.Reflection.Metadata;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.ConditionalFormatting;
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

                Font regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                // Create a table for the header
                PdfPTable header = new PdfPTable(3);
                header.WidthPercentage = 100;

                // Left cell
                PdfPCell LU = new PdfPCell(new Phrase("Lebanese University", boldFont));
                LU.Border = PdfPCell.NO_BORDER;
                LU.HorizontalAlignment = Element.ALIGN_LEFT;
                header.AddCell(LU);

                // Center cell
                PdfPCell title = new PdfPCell(new Phrase("Students Attendance Sheet", regularFont));
                title.Border = PdfPCell.NO_BORDER;
                title.HorizontalAlignment = Element.ALIGN_CENTER;
                header.AddCell(title);


                // Right cell
                PdfPCell date = new PdfPCell(new Phrase("Date: 23/6/2025", regularFont));
                date.Border = PdfPCell.NO_BORDER;
                date.HorizontalAlignment = Element.ALIGN_RIGHT;
                header.AddCell(date);

                PdfPCell FS = new PdfPCell(new Phrase("Faculty of Science", regularFont));
                FS.Border = PdfPCell.NO_BORDER;
                FS.HorizontalAlignment = Element.ALIGN_LEFT;
                header.AddCell(FS);

                PdfPCell e1 = new PdfPCell();
                e1.Border = PdfPCell.NO_BORDER;
                header.AddCell(e1);

                PdfPCell time = new PdfPCell(new Phrase("Time: 8:00 AM", regularFont));
                time.Border = PdfPCell.NO_BORDER;
                time.HorizontalAlignment = Element.ALIGN_RIGHT;
                header.AddCell(time);

                // Add header table to the document
                document.Add(header);


                PdfPTable secondHeader = new PdfPTable(3);
                secondHeader.WidthPercentage = 100;

                document.Add(new Paragraph("\n"));

                Chunk roomText = new Chunk("Room: ", regularFont);
                Chunk roomValue = new Chunk("A", boldFont);
                Phrase roomCellContent = new Phrase();
                roomCellContent.Add(roomText);
                roomCellContent.Add(roomValue);
                PdfPCell roomCell = new PdfPCell(roomCellContent)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                secondHeader.AddCell(roomCell);

                // Course: I3300
                Chunk courseText = new Chunk("Course: ", regularFont);
                Chunk courseValue = new Chunk("I3300", boldFont);
                Phrase courseCellContent = new Phrase();
                courseCellContent.Add(courseText);
                courseCellContent.Add(courseValue);
                PdfPCell courseCell = new PdfPCell(courseCellContent)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                secondHeader.AddCell(courseCell);

                // Exam Code: 8 - 10
                Chunk examCodeText = new Chunk("Exam Code: ", regularFont);
                Chunk examCodeValue = new Chunk("8 - 10", boldFont);
                Phrase examCodeCellContent = new Phrase();
                examCodeCellContent.Add(examCodeText);
                examCodeCellContent.Add(examCodeValue);
                PdfPCell examCodeCell = new PdfPCell(examCodeCellContent)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                secondHeader.AddCell(examCodeCell);

                document.Add(secondHeader);

                // Add some spacing
                document.Add(new Paragraph("\n\n"));

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                List<String> attributes = ["Student ID", "Name", "Language", "Seat Number", "Presence"];
                foreach (String attribute in attributes)
                    table.AddCell(new PdfPCell(new Phrase(attribute, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD))) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER });

                List<StudentExamRegistration> registrations = _context.StudentExamRegistrations.ToList();

                PdfPTable sigField = new PdfPTable(1);
                PdfPCell sigCell = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 5)));
                sigField.AddCell(sigCell);

                

                foreach (var reg in registrations)
                {
                    
                    foreach (string attributeValue in reg.AttributesValues)
                        table.AddCell(new PdfPCell(new Phrase(attributeValue.ToString(), new Font(Font.FontFamily.HELVETICA, 8))) { Padding = 5, VerticalAlignment = Element.ALIGN_CENTER });
                    //table.AddCell(new PdfPCell(sigField) { PaddingLeft = 50, PaddingRight = 50, PaddingTop = 5, PaddingBottom = 5 });
                    table.AddCell(new PdfPCell());
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
