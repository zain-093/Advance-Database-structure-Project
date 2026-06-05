using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSUP.Data;
using System.Security.Claims;

namespace HiSUP.Controllers
{
    public class AccountController : Controller
    {
        private readonly HiSUPContext _context;

        public AccountController(HiSUPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            // Look up the user account.
            // Direct comparison of PasswordHash as plain text to match the seeded data database.
            var account = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (account == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            int studentId = 0;
            int facultyId = 0;

            // Resolve references based on role
            if (account.RoleName == "Student")
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.RegistrationNo == username || s.Email == username);
                studentId = student?.StudentID ?? 0;
            }
            else if (account.RoleName == "Faculty")
            {
                var faculty = await _context.Faculty
                    .FirstOrDefaultAsync(f => f.Email == username);
                facultyId = faculty?.FacultyID ?? 0;
            }

            // Create claims for standard security and RLS mapping
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.RoleName)
            };

            if (studentId > 0)
            {
                claims.Add(new Claim("StudentID", studentId.ToString()));
            }

            if (facultyId > 0)
            {
                claims.Add(new Claim("FacultyID", facultyId.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect to role-based default pages
            if (account.RoleName == "Student")
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("Index", "AdminPortal");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
