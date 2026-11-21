using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Connection String from configuration (supports appsettings.json, User Secrets, or Environment Variables)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection Registrations (Scoped per HTTP request)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Custom Routes handling all request scenarios:
// 1. Details via query string or route segment: /Student/ShowDetails?id=3 or /Student/ShowDetails/3
app.MapControllerRoute(
    name: "studentDetails",
    pattern: "Student/ShowDetails/{id:int}",
    defaults: new { controller = "Student", action = "ShowDetails" });

// 2. Filtered / Paged Student list route: /Students/page/{pageNumber}/size/{pageSize}
app.MapControllerRoute(
    name: "studentsPaged",
    pattern: "Students/page/{pageNumber:int}/size/{pageSize:int}",
    defaults: new { controller = "Student", action = "GetAll" });

// 3. Standard MVC route with optional id & query string support
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=GetAll}/{id?}");

// 4. Fallback route so unmatched paths cleanly route to default controller/action instead of failing silently
app.MapFallbackToController("GetAll", "Student");

app.Run();
