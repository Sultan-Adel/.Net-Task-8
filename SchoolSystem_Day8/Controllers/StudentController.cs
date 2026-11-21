using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using SchoolSystem.ViewModels;

namespace SchoolSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public StudentController(IStudentRepository studentRepo, IDepartmentRepository departmentRepo)
        {
            _studentRepo = studentRepo;
            _departmentRepo = departmentRepo;
        }

        private async Task<List<SelectListItem>> GetDepartmentsSelectListAsync(int? selectedId = null)
        {
            var departments = await _departmentRepo.GetAllAsync();
            return departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = selectedId.HasValue && d.Id == selectedId.Value
                })
                .ToList();
        }

        // AJAX endpoint: /Student/AjaxSearch?name=ali
        [HttpGet]
        public async Task<IActionResult> AjaxSearch(string? name)
        {
            var students = await _studentRepo.SearchByNameAsync(name);
            var result = students.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                age = s.Age,
                departmentName = s.Department?.Name ?? "N/A"
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchName, int? departmentId, int pageNumber = 1, int pageSize = 5)
        {
            var (items, totalCount) = await _studentRepo.GetPagedStudentsAsync(searchName, departmentId, pageNumber, pageSize);

            var vm = new StudentListViewModel
            {
                Students = items.ToList(),
                SearchName = searchName,
                DepartmentId = departmentId,
                Departments = await GetDepartmentsSelectListAsync(departmentId),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetails(int id)
        {
            var student = await _studentRepo.GetStudentWithDetailsAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new StudentFormViewModel
            {
                Departments = await GetDepartmentsSelectListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(StudentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Departments = await GetDepartmentsSelectListAsync(vm.DepartmentId);
                return View(vm);
            }

            var student = new Student
            {
                Name = vm.Name.Trim(),
                Age = vm.Age,
                DepartmentId = vm.DepartmentId
            };

            await _studentRepo.AddAsync(student);
            await _studentRepo.SaveChangesAsync();
            return RedirectToAction(nameof(GetAll));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null) return NotFound();

            var vm = new StudentFormViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                DepartmentId = student.DepartmentId,
                Departments = await GetDepartmentsSelectListAsync(student.DepartmentId)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                vm.Departments = await GetDepartmentsSelectListAsync(vm.DepartmentId);
                return View(vm);
            }

            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null) return NotFound();

            student.Name = vm.Name.Trim();
            student.Age = vm.Age;
            student.DepartmentId = vm.DepartmentId;

            await _studentRepo.UpdateAsync(student);
            await _studentRepo.SaveChangesAsync();
            return RedirectToAction(nameof(GetAll));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentRepo.GetStudentWithDetailsAsync(id);
            if (student == null) return NotFound();
            return View("DeleteWarning", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _studentRepo.DeleteAsync(id);
            await _studentRepo.SaveChangesAsync();
            return RedirectToAction(nameof(GetAll));
        }
    }
}
