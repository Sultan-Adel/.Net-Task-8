using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolSystem.Models;

namespace SchoolSystem.ViewModels
{
    public class StudentListViewModel
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public string? SearchName { get; set; }
        public int? DepartmentId { get; set; }
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
    }
}
