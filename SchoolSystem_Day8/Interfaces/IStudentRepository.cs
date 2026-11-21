using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedStudentsAsync(
            string? searchName, int? departmentId, int pageNumber, int pageSize);
        Task<Student?> GetStudentWithDetailsAsync(int id);
        Task<IEnumerable<Student>> SearchByNameAsync(string? name);
    }
}
