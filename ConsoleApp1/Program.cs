using System;
using System.Collections.Generic;
using IronOcr;
using ClosedXML.Excel;
using System.Numerics;
using BitMiracle.LibTiff.Classic;

class Student
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Language { get; set; }
    public string Seat { get; set; }
    public bool Present { get; set; }
}

public class OcrService
{
    public static void ReadAndGenerate(String pdf)
    {
        var Ocr = new IronTesseract();
        var students = new List<Student>();

        using (var input = new OcrInput())
        {
            input.AddPdf(pdf);

            var result = Ocr.Read(input);
            var lines = result.Text.Split('\n');
            Console.WriteLine("=== OCR Raw Output ===");
            Console.WriteLine(result.Text);
            Console.WriteLine("=======================");

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                var tokens = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0)
                    continue; // Skip empty or whitespace-only lines

                var firstToken = tokens[0];


                if (int.TryParse(firstToken, out int idNumber) && idNumber >= 1 && idNumber <= 1000000)
                {
                    var columns = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (columns.Length >= 5)
                    {
                        // Try to detect "Presence" column visually
                        var present = columns.Length > 5 && !string.IsNullOrWhiteSpace(columns[5]);

                        students.Add(new Student
                        {
                            ID = columns[0],
                            Name = $"{columns[1]} {columns[2]}",
                            Language = columns[3],
                            Seat = columns[4],
                            Present = present
                        });
                    }
                }
            }
        }

        // Export to Excel
        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Attendance");

            ws.Cell(1, 1).Value = "Student ID";
            ws.Cell(1, 2).Value = "Name";
            ws.Cell(1, 3).Value = "Language";
            ws.Cell(1, 4).Value = "Seat";
            ws.Cell(1, 5).Value = "Present";

            for (int i = 0; i < students.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = students[i].ID;
                ws.Cell(i + 2, 2).Value = students[i].Name;
                ws.Cell(i + 2, 3).Value = students[i].Language;
                ws.Cell(i + 2, 4).Value = students[i].Seat;
                ws.Cell(i + 2, 5).Value = students[i].Present ? "Yes" : "No";
            }

            workbook.SaveAs("Attendance_Export.xlsx");
        }

        Console.WriteLine("✅ Attendance data saved to Attendance_Export.xlsx");
        
    }
    public static void Main(string[] args)
    {
        IronOcr.License.LicenseKey = "IRONSUITE.MOHAMMADMOBARAK317.GMAIL.COM.31983-B9048D4606-BH3NP4I-J2RIF2B2XK5D-UG6VFW3D2VPR-CMQKRNPVHZUD-PWDHRJWX55GV-WC4LRRVVXTZE-XRQ3RD2DI37U-FPT6WY-TWBHR65DO46PEA-DEPLOYMENT.TRIAL-SVY4RG.TRIAL.EXPIRES.31.MAY.2025";
        OcrService.ReadAndGenerate(@"C:\\Users\\user\\Desktop\\GITHUB\\Senior_Project\\Plan&Scan\\ConsoleApp1\\Students_Attendance_Sheet2.pdf");
    }
}