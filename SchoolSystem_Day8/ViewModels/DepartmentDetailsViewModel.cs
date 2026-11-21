using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolSystem.ViewModels
{
    public class DepartmentDetailsViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string MgrName { get; set; } = string.Empty;
        public int TotalStudentsCount { get; set; }
        public string DepartmentState { get; set; } = string.Empty;
        public List<SelectListItem> StudentsOver25 { get; set; } = new List<SelectListItem>();
    }
}
