using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;

namespace HiSUP.Controllers
{
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
            // Fetching active sections. In a real app, 'Spring 2025' would be dynamic.
            var availableSections = await _context.Courses
                .Join(_context.Set<Models.Section>(), // Assuming you add Section to DbContext
                    c => c.CourseID,
                    s => s.CourseID,
                    (c, s) => new { c.CourseName, c.CourseCode, s.Semester, s.AvailableSeats, s.CourseID })
                .Where(s => s.Semester == "Spring 2025")
                .ToListAsync();

            ViewBag.Sections = availableSections;
            return View();
        }

        // POST: Attempt to enroll in a course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId, string semester)
        {
            // Hardcoding StudentID 1000 for now. 
            // We will replace this with User.Identity.GetUserId() when we add Auth.
            int currentStudentId = 1000; 

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC EnrollInCourse @StudentID, @CourseID, @Semester",
                    new SqlParameter("@StudentID", currentStudentId),
                    new SqlParameter("@CourseID", courseId),
                    new SqlParameter("@Semester", semester)
                );

                TempData["SuccessMessage"] = "Successfully enrolled! Seat count has been updated.";
            }
            catch (SqlException ex)
            {
                // Catching our custom THROW 50002 or deadlocks from the database
                TempData["ErrorMessage"] = "Enrollment Failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}