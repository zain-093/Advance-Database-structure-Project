using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;
using HiSUP.Models;
using System.Security.Claims;

namespace HiSUP.Controllers
{
    [Authorize(Roles = "Student")]
    public class EnrollmentController : Controller
    {
        private readonly HiSUPContext _context;

        public EnrollmentController(HiSUPContext context)
        {
            _context = context;
        }

        // GET: Display available courses for the current semester
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetching active sections for Spring 2025
            var sections = await _context.Sections
                .Include(s => s.Course)
                .Where(s => s.Semester == "Spring 2025")
                .ToListAsync();

            var availableSections = new List<dynamic>();
            foreach (var s in sections)
            {
                int enrolledCount = await _context.Enrollments.CountAsync(e => e.SectionID == s.SectionID);
                int availableSeats = (s.Capacity ?? 40) - enrolledCount;

                availableSections.Add(new {
                    CourseID = s.CourseID ?? 0,
                    SectionID = s.SectionID,
                    CourseCode = s.Course?.CourseCode ?? "N/A",
                    CourseName = s.Course?.CourseTitle ?? "Untitled Course",
                    Semester = s.Semester,
                    AvailableSeats = availableSeats > 0 ? availableSeats : 0
                });
            }

            ViewBag.Sections = availableSections;
            return View();
        }

        // POST: Attempt to enroll in a course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId, string semester)
        {
            var studentIdClaim = User.FindFirst("StudentID")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }
            int currentStudentId = int.Parse(studentIdClaim); 

            try
            {
                // Find matching section
                var section = await _context.Sections
                    .FirstOrDefaultAsync(s => s.CourseID == courseId && s.Semester == semester);

                if (section == null)
                {
                    TempData["ErrorMessage"] = "Enrollment Failed: No class section found for this course.";
                    return RedirectToAction(nameof(Index));
                }

                // Check duplicate enrollment
                bool isAlreadyEnrolled = await _context.Enrollments
                    .AnyAsync(e => e.StudentID == currentStudentId && e.SectionID == section.SectionID);

                if (isAlreadyEnrolled)
                {
                    TempData["ErrorMessage"] = "Enrollment Failed: You are already enrolled in this course.";
                    return RedirectToAction(nameof(Index));
                }

                // Check capacity
                int currentEnrolled = await _context.Enrollments.CountAsync(e => e.SectionID == section.SectionID);
                if (currentEnrolled >= (section.Capacity ?? 40))
                {
                    TempData["ErrorMessage"] = "Enrollment Failed: No seats available for this course in the current semester.";
                    return RedirectToAction(nameof(Index));
                }

                // Add enrollment record
                var enrollment = new Enrollment
                {
                    StudentID = currentStudentId,
                    SectionID = section.SectionID
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                // Trigger a log using stored procedure or just message
                TempData["SuccessMessage"] = "Successfully enrolled! Seat count has been updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Enrollment Failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}