using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Plan_Scan.Models
{
    public class StudentExamRegistration
    {
        [Key]
        public int RegId { get; set; }

        [Required]
        [Range(1, 1000000, ErrorMessage = "Student Id must be between 1 and 1000000.")]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Course { get; set; }

        [Required]
        [RegularExpression("^[EF]$", ErrorMessage = "Lang must be 'E' or 'F'.")]
        public char Lang { get; set; }

        [Required]
        [StringLength(10)]
        public string Room { get; set; }

        [Range(1, 100, ErrorMessage = "Seat Number must be between 1 and 100.")]
        public int SeatNb { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        [Required]
        [StringLength(20)]
        public string ExamCode { get; set; }
    }
}
