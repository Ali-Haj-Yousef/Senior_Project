using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plan_Scan.Models
{
    public class PlanSheetViewModel
    {

        [DisplayName("Exam Code")]
        public int? ExamCode { get; set; }

        public string? Room { get; set; }

        [Required]
        [DisplayName("Start Date")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$",
        ErrorMessage = "Date must be in the format dd/mm/yyyy.")]
        public DateOnly StartDate { get; set; }

        [DisplayName("End Date")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$",
        ErrorMessage = "Date must be in the format dd/mm/yyyy.")]
        public DateOnly? EndDate { get; set; }
    }
}
