using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;
using HiSUP.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiSUP.Controllers
{
    [Authorize(Roles = "Admin,Faculty,Finance,Library")]
    public class AdminPortalController : Controller
    {
        private readonly HiSUPContext _context;

        public AdminPortalController(HiSUPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            bool isFaculty = User.IsInRole("Faculty");
            int currentFacultyId = 0;

            if (isFaculty)
            {
                var fIdClaim = User.FindFirst("FacultyID")?.Value;
                if (!string.IsNullOrEmpty(fIdClaim))
                    currentFacultyId = int.Parse(fIdClaim);
            }

            // Load all base data
            var allSections = await _context.Sections.Include(s => s.Course).Include(s => s.Faculty).ToListAsync();
            var allEnrollments = await _context.Enrollments.Include(e => e.Student).Include(e => e.Section).ThenInclude(sec => sec.Course).ToListAsync();
            var allGrades = await _context.Grades.Include(g => g.Enrollment).ThenInclude(e => e.Student).Include(g => g.Enrollment).ThenInclude(e => e.Section).ThenInclude(sec => sec.Course).ToListAsync();
            var allAttendance = await _context.AttendanceRecords.Include(ar => ar.Student).Include(ar => ar.Section).ThenInclude(sec => sec.Course).ToListAsync();

            // If Faculty, filter to their assigned sections only
            if (isFaculty && currentFacultyId > 0)
            {
                var facultySectionIds = allSections.Where(s => s.FacultyID == currentFacultyId).Select(s => s.SectionID).ToHashSet();
                allSections = allSections.Where(s => s.FacultyID == currentFacultyId).ToList();
                allEnrollments = allEnrollments.Where(e => facultySectionIds.Contains(e.SectionID ?? 0)).ToList();
                allGrades = allGrades.Where(g => g.Enrollment != null && facultySectionIds.Contains(g.Enrollment.SectionID ?? 0)).ToList();
                allAttendance = allAttendance.Where(a => facultySectionIds.Contains(a.SectionID ?? 0)).ToList();
            }

            var model = new AdminPortalViewModel
            {
                Departments = await _context.Departments.ToListAsync(),
                Programs = await _context.Programs.Include(p => p.Department).ToListAsync(),
                Students = await _context.Students.Include(s => s.Program).ToListAsync(),
                Faculty = await _context.Faculty.ToListAsync(),
                Staffs = await _context.Staffs.ToListAsync(),
                Courses = await _context.Courses.ToListAsync(),
                CoursePrerequisites = await _context.CoursePrerequisites
                    .Include(cp => cp.Course)
                    .Include(cp => cp.PrerequisiteCourse)
                    .ToListAsync(),
                Sections = allSections,
                Enrollments = allEnrollments,
                Grades = allGrades,
                AttendanceRecords = allAttendance,
                FeeStructures = await _context.FeeStructures.Include(fs => fs.Program).ToListAsync(),
                FeePayments = await _context.FeePayments.Include(fp => fp.Student).ToListAsync(),
                LibraryItems = await _context.LibraryItems.ToListAsync(),
                LibraryIssues = await _context.LibraryIssues.Include(li => li.Student).Include(li => li.LibraryItem).ToListAsync(),
                Hostels = await _context.Hostels.ToListAsync(),
                HostelAllotments = await _context.HostelAllotments.Include(ha => ha.Student).Include(ha => ha.Hostel).ToListAsync(),
                ExamSchedules = await _context.ExamSchedules.Include(es => es.Course).ToListAsync(),
                Results = await _context.Results.Include(r => r.Student).ToListAsync(),
                UserAccounts = await _context.UserAccounts.ToListAsync(),
                AuditLogs = await _context.AuditLogs.OrderByDescending(al => al.ActionDate).Take(30).ToListAsync(),
                Notifications = await _context.Notifications.ToListAsync(),
                StudentDocuments = await _context.StudentDocuments.Include(sd => sd.Student).ToListAsync()
            };

            // Compute Admin KPIs
            model.TotalFeeCollected = model.FeePayments.Sum(fp => fp.AmountPaid ?? 0);
            model.HostelOccupancy = model.HostelAllotments.Count;
            model.TotalLibraryBooks = model.LibraryItems.Sum(li => li.CopiesAvailable ?? 0);

            // Faculty-specific stats
            if (isFaculty && currentFacultyId > 0)
            {
                var faculty = model.Faculty.FirstOrDefault(f => f.FacultyID == currentFacultyId);
                model.FacultyName = faculty != null ? $"{faculty.FirstName} {faculty.LastName}" : "Faculty";
                model.FacultySections = model.Sections.Count;
                model.FacultyAssignedCourses = model.Sections.Select(s => s.CourseID).Distinct().Count();
                model.FacultyStudentCount = model.Enrollments.Select(e => e.StudentID).Distinct().Count();
            }

            return View(model);
        }

        // ==========================================
        // ACADEMICS SECTION
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var item = await _context.Departments.FindAsync(id);
            if (item != null)
            {
                _context.Departments.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProgram(AcademicProgram program)
        {
            if (ModelState.IsValid)
            {
                _context.Programs.Add(program);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Program added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var item = await _context.Programs.FindAsync(id);
            if (item != null)
            {
                _context.Programs.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Program deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var item = await _context.Courses.FindAsync(id);
            if (item != null)
            {
                _context.Courses.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSection(Section section)
        {
            if (ModelState.IsValid)
            {
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Section added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var item = await _context.Sections.FindAsync(id);
            if (item != null)
            {
                _context.Sections.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Section deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // PEOPLE SECTION
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student registered successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var item = await _context.Students.FindAsync(id);
            if (item != null)
            {
                _context.Students.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFaculty(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Faculty.Add(faculty);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Faculty member added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            var item = await _context.Faculty.FindAsync(id);
            if (item != null)
            {
                _context.Faculty.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Faculty member deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Staff member added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var item = await _context.Staffs.FindAsync(id);
            if (item != null)
            {
                _context.Staffs.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Staff member deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // REGISTRATIONS & ACADEMICS
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEnrollment(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student enrolled successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var item = await _context.Enrollments.FindAsync(id);
            if (item != null)
            {
                _context.Enrollments.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Enrollment removed successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> CreateGrade(Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Grades.Add(grade);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Grade recorded successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var item = await _context.Grades.FindAsync(id);
            if (item != null)
            {
                _context.Grades.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Grade deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> CreateAttendance(AttendanceRecord attendance)
        {
            attendance.AttendanceDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                _context.AttendanceRecords.Add(attendance);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Attendance record added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var item = await _context.AttendanceRecords.FindAsync(id);
            if (item != null)
            {
                _context.AttendanceRecords.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Attendance record deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // FINANCE SECTION
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Finance")]
        public async Task<IActionResult> CreateFeeStructure(FeeStructure feeStructure)
        {
            if (ModelState.IsValid)
            {
                _context.FeeStructures.Add(feeStructure);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fee structure added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Finance")]
        public async Task<IActionResult> DeleteFeeStructure(int id)
        {
            var item = await _context.FeeStructures.FindAsync(id);
            if (item != null)
            {
                _context.FeeStructures.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fee structure deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Finance")]
        public async Task<IActionResult> CreateFeePayment(FeePayment feePayment)
        {
            feePayment.PaymentDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.FeePayments.Add(feePayment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fee payment processed successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Finance")]
        public async Task<IActionResult> DeleteFeePayment(int id)
        {
            var item = await _context.FeePayments.FindAsync(id);
            if (item != null)
            {
                _context.FeePayments.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fee payment transaction deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // SERVICES (LIBRARY & HOSTELS)
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Library")]
        public async Task<IActionResult> CreateLibraryItem(LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                _context.LibraryItems.Add(libraryItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Library book added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Library")]
        public async Task<IActionResult> DeleteLibraryItem(int id)
        {
            var item = await _context.LibraryItems.FindAsync(id);
            if (item != null)
            {
                _context.LibraryItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Library book deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Library")]
        public async Task<IActionResult> CreateLibraryIssue(LibraryIssue libraryIssue)
        {
            libraryIssue.IssueDate = DateTime.Now.Date;
            libraryIssue.ReturnDate = null;
            if (ModelState.IsValid)
            {
                _context.LibraryIssues.Add(libraryIssue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Library book issued successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Library")]
        public async Task<IActionResult> ReturnLibraryBook(int id)
        {
            var item = await _context.LibraryIssues.FindAsync(id);
            if (item != null)
            {
                item.ReturnDate = DateTime.Now.Date;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Library book marked as returned!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHostel(Hostel hostel)
        {
            if (ModelState.IsValid)
            {
                _context.Hostels.Add(hostel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hostel hall created successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHostel(int id)
        {
            var item = await _context.Hostels.FindAsync(id);
            if (item != null)
            {
                _context.Hostels.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hostel hall deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHostelAllotment(HostelAllotment hostelAllotment)
        {
            if (ModelState.IsValid)
            {
                _context.HostelAllotments.Add(hostelAllotment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hostel room allotted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHostelAllotment(int id)
        {
            var item = await _context.HostelAllotments.FindAsync(id);
            if (item != null)
            {
                _context.HostelAllotments.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hostel room allotment removed!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // SCHEDULES & USER ACCOUNTS & DOCUMENTS & NOTIFICATIONS
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateExam(ExamSchedule exam)
        {
            if (ModelState.IsValid)
            {
                _context.ExamSchedules.Add(exam);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exam schedule added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var item = await _context.ExamSchedules.FindAsync(id);
            if (item != null)
            {
                _context.ExamSchedules.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exam schedule deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateResult(Result result)
        {
            if (ModelState.IsValid)
            {
                _context.Results.Add(result);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Semester result added successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var item = await _context.Results.FindAsync(id);
            if (item != null)
            {
                _context.Results.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Semester result deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserAccount(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                _context.UserAccounts.Add(account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User account registered successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAccount(int id)
        {
            var item = await _context.UserAccounts.FindAsync(id);
            if (item != null)
            {
                _context.UserAccounts.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User account deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Portal notification published!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var item = await _context.Notifications.FindAsync(id);
            if (item != null)
            {
                _context.Notifications.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Portal notification deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDocument(StudentDocument document)
        {
            if (ModelState.IsValid)
            {
                _context.StudentDocuments.Add(document);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student document uploaded successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var item = await _context.StudentDocuments.FindAsync(id);
            if (item != null)
            {
                _context.StudentDocuments.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student document removed!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
