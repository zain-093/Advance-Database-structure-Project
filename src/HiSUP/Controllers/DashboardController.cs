using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;

namespace HiSUP.Controllers
{
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
            // Thanks to the Interceptor, this automatically filters to StudentID 1000!
            var dashboardData = await _context.StudentDashboard.ToListAsync();
            return View(dashboardData);
        }
    }
}