using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;
using HiSUP.Models;
using System.Security.Claims;

namespace HiSUP.Controllers
{
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly HiSUPContext _context;

        public DashboardController(HiSUPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var studentIdClaim = User.FindFirst("StudentID")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            int currentStudentId = int.Parse(studentIdClaim);

            var student = await _context.Students
                .Include(s => s.Program)
                .ThenInclude(p => p.Department)
                .FirstOrDefaultAsync(s => s.StudentID == currentStudentId);

            if (student == null)
            {
                // Sign out if student record is deleted
                return RedirectToAction("Login", "Account");
            }

            // Fetch calculated stats using Raw SQL queries to call HITECUNI_DB scalar functions
            decimal cgpa = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT dbo.fn_CalculateCGPA(@StudentID)";
                    var param = command.CreateParameter();
                    param.ParameterName = "@StudentID";
                    param.Value = currentStudentId;
                    command.Parameters.Add(param);
                    
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();
                        
                    var val = await command.ExecuteScalarAsync();
                    if (val != null && val != DBNull.Value)
                        cgpa = Convert.ToDecimal(val);
                }
            }
            catch { }

            decimal outstandingFee = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT dbo.fn_GetOutstandingFee(@StudentID)";
                    var param = command.CreateParameter();
                    param.ParameterName = "@StudentID";
                    param.Value = currentStudentId;
                    command.Parameters.Add(param);
                    
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();
                        
                    var val = await command.ExecuteScalarAsync();
                    if (val != null && val != DBNull.Value)
                        outstandingFee = Convert.ToDecimal(val);
                }
            }
            catch { }

            decimal attendancePercentage = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT dbo.fn_GetAttendancePercentage(@StudentID)";
                    var param = command.CreateParameter();
                    param.ParameterName = "@StudentID";
                    param.Value = currentStudentId;
                    command.Parameters.Add(param);
                    
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();
                        
                    var val = await command.ExecuteScalarAsync();
                    if (val != null && val != DBNull.Value)
                        attendancePercentage = Convert.ToDecimal(val);
                }
            }
            catch { }

            // Fetch enrollments (Automatically filtered by RLS or StudentID)
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentID == currentStudentId)
                .Select(e => new EnrollmentDetailsViewModel
                {
                    EnrollmentID = e.EnrollmentID,
                    CourseCode = e.Section.Course.CourseCode,
                    CourseTitle = e.Section.Course.CourseTitle,
                    CreditHours = e.Section.Course.CreditHours ?? 0,
                    FacultyName = e.Section.Faculty.FirstName + " " + e.Section.Faculty.LastName,
                    Semester = e.Section.Semester,
                    Marks = _context.Grades.FirstOrDefault(g => g.EnrollmentID == e.EnrollmentID).Marks,
                    LetterGrade = _context.Grades.FirstOrDefault(g => g.EnrollmentID == e.EnrollmentID).LetterGrade
                })
                .ToListAsync();

            // Fetch Fee Payments (Filtered by RLS or StudentID)
            var payments = await _context.FeePayments
                .Where(p => p.StudentID == currentStudentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            // Fetch Hostel Allotment
            var hostelAllotment = await _context.HostelAllotments
                .Include(h => h.Hostel)
                .FirstOrDefaultAsync(h => h.StudentID == currentStudentId);

            // Fetch Library Issues
            var libraryIssues = await _context.LibraryIssues
                .Include(li => li.LibraryItem)
                .Where(li => li.StudentID == currentStudentId)
                .Select(li => new LibraryIssueDetailsViewModel
                {
                    IssueID = li.IssueID,
                    Title = li.LibraryItem.Title,
                    Author = li.LibraryItem.Author,
                    IssueDate = li.IssueDate ?? DateTime.MinValue,
                    ReturnDate = li.ReturnDate
                })
                .ToListAsync();

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                ProgramName = student.Program?.ProgramName ?? "Undeclared",
                DepartmentName = student.Program?.Department?.DepartmentName ?? "Undeclared",
                CGPA = cgpa,
                OutstandingFee = outstandingFee,
                AttendancePercentage = attendancePercentage,
                EnrolledCoursesCount = enrollments.Count,
                HostelName = hostelAllotment?.Hostel?.HostelName ?? "No Hostel Allotment",
                RoomNo = hostelAllotment?.RoomNo ?? "N/A",
                Enrollments = enrollments,
                FeePayments = payments,
                LibraryIssues = libraryIssues
            };

            return View(viewModel);
        }
    }
}