using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using SchoolSystem.ViewModels;

namespace SchoolSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentController(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var departments = await _departmentRepo.GetAllWithStudentsAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetails(int id)
        {
            var department = await _departmentRepo.GetByIdWithStudentsAsync(id);
            if (department == null) return NotFound();

            int count = department.Students.Count;
            var vm = new DepartmentDetailsViewModel
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                MgrName = department.MgrName,
                TotalStudentsCount = count,
                DepartmentState = count > 50 ? "Main" : "Branch",
                StudentsOver25 = department.Students
                    .Where(s => s.Age > 25)
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Department());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Department department)
        {
            if (!ModelState.IsValid) return View(department);

            await _departmentRepo.AddAsync(department);
            await _departmentRepo.SaveChangesAsync();
            return RedirectToAction(nameof(ShowAll));
        }
    }
}
