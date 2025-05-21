using System.Diagnostics;
using System.Reflection.Metadata;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.ConditionalFormatting;
using Org.BouncyCastle.Utilities;
using Plan_Scan.Data;
using Plan_Scan.Models;
using Document = iTextSharp.text.Document;

namespace Plan_Scan.Controllers
{
    public class PlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        Font regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        public PlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<DateOnly> GetDateRange(DateOnly startDate, DateOnly? endDate)
        {
            List<DateOnly> dates = new List<DateOnly>();

            for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            return dates;
        }

        private PdfPTable CreateTopHeader(DateOnly dateOnly, TimeOnly timeOnly)
        {
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
            PdfPCell date = new PdfPCell(new Phrase("Date: " + dateOnly, regularFont));
            date.Border = PdfPCell.NO_BORDER;
            date.HorizontalAlignment = Element.ALIGN_RIGHT;
            header.AddCell(date);

            PdfPCell FS = new PdfPCell(new Phrase("Faculty of Science (FS1)", regularFont));
            FS.Border = PdfPCell.NO_BORDER;
            FS.HorizontalAlignment = Element.ALIGN_LEFT;
            header.AddCell(FS);

            PdfPCell e1 = new PdfPCell();
            e1.Border = PdfPCell.NO_BORDER;
            header.AddCell(e1);

            PdfPCell time = new PdfPCell(new Phrase("Time: " + timeOnly, regularFont));
            time.Border = PdfPCell.NO_BORDER;
            time.HorizontalAlignment = Element.ALIGN_RIGHT;
            header.AddCell(time);

            return header;
        }

        private PdfPTable CreateIdentifiersHeader(String room, String course, int examCode)
        {
            PdfPTable identifiersHeader = new PdfPTable(3);
            identifiersHeader.WidthPercentage = 100;

            // Exam Code: 8 - 10
            Chunk examCodeText = new Chunk("Exam Code: ", regularFont);
            Chunk examCodeValue = new Chunk(examCode.ToString(), boldFont);
            Phrase examCodeCellContent = new Phrase();
            examCodeCellContent.Add(examCodeText);
            examCodeCellContent.Add(examCodeValue);
            PdfPCell examCodeCell = new PdfPCell(examCodeCellContent)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            identifiersHeader.AddCell(examCodeCell);

            // Course: I3300
            Chunk courseText = new Chunk("Course: ", regularFont);
            Chunk courseValue = new Chunk(course, boldFont);
            Phrase courseCellContent = new Phrase();
            courseCellContent.Add(courseText);
            courseCellContent.Add(courseValue);
            PdfPCell courseCell = new PdfPCell(courseCellContent)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            identifiersHeader.AddCell(courseCell);

            Chunk roomText = new Chunk("Room: ", regularFont);
            Chunk roomValue = new Chunk(room, boldFont);
            Phrase roomCellContent = new Phrase();
            roomCellContent.Add(roomText);
            roomCellContent.Add(roomValue);
            PdfPCell roomCell = new PdfPCell(roomCellContent)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };
            identifiersHeader.AddCell(roomCell);
            
            return identifiersHeader;
        }

        /*private PdfPTable CreateRegistrationsTable(List<StudentExamRegistration> studentExamRegistrations)
        {
            PdfPTable registrationsTable = new PdfPTable(5);
            registrationsTable.WidthPercentage = 100;

            // Configure Arabic font with RTL support
            BaseFont arabicBaseFont = BaseFont.CreateFont(
                Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\arial.ttf", // Times New Roman
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );
            Font arabicFont = new Font(arabicBaseFont, 12, Font.NORMAL, BaseColor.BLACK);



            // Configure header font
            Font headerFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);

            // Add headers
            string[] headers = { "Student ID", "Name", "Language", "Seat Number", "Presence" };
            foreach (string header in headers)
            {
                PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                headerCell.Padding = 5;
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.VerticalAlignment = Element.ALIGN_CENTER;
                registrationsTable.AddCell(headerCell);
            }

            // Add student data
            foreach (var reg in studentExamRegistrations)
            {
                foreach (string attributeValue in reg.AttributesValues)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(attributeValue, arabicFont));
                    cell.Padding = 5;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    // Enable RTL for Arabic text cells
                    if (attributeValue.Any(c => c >= 0x0600 && c <= 0x06FF)) // Check if text contains Arabic characters
                    {
                        cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    }

                    registrationsTable.AddCell(cell);
                }
                registrationsTable.AddCell(new PdfPCell()); // Empty presence cell
            }

            return registrationsTable;
        }*/

        private PdfPTable CreateRegistrationsTable(List<StudentExamRegistration> studentExamRegistrations)
        {
            PdfPTable registrationsTable = new PdfPTable(5);
            float[] columnWidths = new float[] { 1f, 1.5f, 0.5f, 0.5f, 1f }; // Set widths for each column
            registrationsTable.SetWidths(columnWidths);
            // Specify the path to the OCR-B font file
            string fontPath = @"C:\Users\DELL\Downloads\Compressed\ocr-b-regular_freefontdownload_org\ocr-b-regular.ttf"; // Adjust the filename if necessary

            // Configure OCR-B font
            BaseFont ocrbBaseFont = BaseFont.CreateFont(
                fontPath,
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );
            Font ocrbFont = new Font(ocrbBaseFont, 14, Font.NORMAL, BaseColor.BLACK);

            BaseFont arabicBaseFont = BaseFont.CreateFont(
                Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\arial.ttf", // Times New Roman
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );
            Font arabicFont = new Font(arabicBaseFont, 12, Font.NORMAL, BaseColor.BLACK);

            // Configure header font (optional, keep existing or change as needed)
            Font headerFont = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);

            // Add headers
            string[] headers = { "Student ID", "Name", "Lang", "Seat", "Presence" };
            for(int i = 0; i < headers.Length; i++)
            {
                PdfPCell headerCell = new PdfPCell(new Phrase(headers[i], headerFont));
                headerCell.Padding = 5;
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.VerticalAlignment = Element.ALIGN_CENTER;
                registrationsTable.AddCell(headerCell);
            }

            // Add student data
            foreach (var reg in studentExamRegistrations)
            {
                for(int i = 0; i < reg.AttributesValues.Count(); i++)
                {
                    PdfPCell cell;
                    if (i == 1)
                        cell = new PdfPCell(new Phrase(reg.AttributesValues[i], arabicFont)); // Use OCR-B font
                    else
                        cell = new PdfPCell(new Phrase(reg.AttributesValues[i], ocrbFont));

                    cell.Padding = 7;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    // Enable RTL for Arabic text cells
                    if (reg.AttributesValues[i].Any(c => c >= 0x0600 && c <= 0x06FF)) // Check if text contains Arabic characters
                    {
                        cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    }

                    registrationsTable.AddCell(cell);
                }
                registrationsTable.AddCell(new PdfPCell()); // Empty presence cell
            }

            return registrationsTable;
        }

        public IActionResult GeneratePDF()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GeneratePDF(PlanSheetViewModel planSheetViewModel)
        {
            List<String> roomsInData = _context.StudentExamRegistrations.Select(r => r.Room).Distinct().ToList();

            if (planSheetViewModel.Room != null && !roomsInData.Contains(planSheetViewModel.Room))
                ModelState.AddModelError("room", "This room doesn't exist.");

            var examCodesInData = _context.StudentExamRegistrations.Select(r => r.ExamCode).Distinct().ToList();

            if (planSheetViewModel.ExamCode != null && !examCodesInData.Contains((int)planSheetViewModel.ExamCode))
                ModelState.AddModelError("examCode", "This exam code doesn't exist.");

            if (!ModelState.IsValid)
                return View();
            
            else
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    
                    document.Open();

                    PdfContentByte cb = writer.DirectContent;

                    if (planSheetViewModel.EndDate == null)
                        planSheetViewModel.EndDate = planSheetViewModel.StartDate;

                    List<DateOnly> datesInData = _context.StudentExamRegistrations.Select(r => r.Date).Distinct().ToList();

                    List<DateOnly> datesRange = GetDateRange(planSheetViewModel.StartDate, planSheetViewModel.EndDate);

                    foreach (DateOnly date in datesRange)
                    {
                        if (datesInData.Contains(date))
                        {
                            IQueryable<StudentExamRegistration> query = _context.StudentExamRegistrations.Where(reg => reg.Date == date);

                            if (planSheetViewModel.ExamCode != null)
                                query = query.Where(reg => reg.ExamCode == planSheetViewModel.ExamCode);

                            if (planSheetViewModel.Room != null)
                                query = query.Where(reg => reg.Room == planSheetViewModel.Room);
                            List<StudentExamRegistration> studentExamRegistrations = query.ToList();
                            var examCodes = studentExamRegistrations.Select(r => r.ExamCode).Distinct().ToList();

                            foreach (var examCode in examCodes)
                            {
                                List<String> courses = studentExamRegistrations.Where(r => r.ExamCode == examCode).Select(r => r.Course).Distinct().ToList();
                                foreach (String course in courses)
                                {
                                    List<String> rooms = studentExamRegistrations.Where(r => r.Course == course).Select(r => r.Room).Distinct().ToList();
                                    foreach (String room in rooms)
                                    {
                                        List<StudentExamRegistration> registrations = _context.StudentExamRegistrations
                                                                                        .Where(reg => reg.ExamCode == examCode &&
                                                                                                      reg.Course == course &&
                                                                                                      reg.Room == room)
                                                                                        .ToList();

                                        int pagesNb = (int)Math.Ceiling((float)registrations.Count / 23);

                                        for (int i = 1; i <= pagesNb; i++)
                                        {
                                            document.NewPage();
                                            document.Add(CreateTopHeader(registrations.First().Date, registrations.First().Time));

                                            document.Add(new Paragraph("\n"));

                                            document.Add(CreateIdentifiersHeader(room, course, examCode));

                                            // Add some spacing
                                            document.Add(new Paragraph("\n\n"));

                                            document.Add(CreateRegistrationsTable(registrations.Take(23).ToList()));

                                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase("Page " + i + "/" + pagesNb, regularFont), document.PageSize.Width / 2, document.Bottom - 20, 0);

                                            if (i != pagesNb)
                                                registrations.RemoveRange(0, 23);

                                        }

                                    }
                                }
                            }
                        }

                    }

                    // Add header table to the document

                    document.Close();
                    writer.Close();
                    var constant = ms.ToArray();
                    return File(constant, "application/vnd", "firstPdf.pdf");

                }
            }
            
        }
    }

        
    
}
