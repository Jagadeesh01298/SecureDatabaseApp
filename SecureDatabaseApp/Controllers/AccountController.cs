using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureDatabaseApp.Data;
using SecureDatabaseApp.Models;
using SecureDatabaseApp.Services;
using SecureDatabaseApp.ViewModels;

namespace SecureDatabaseApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly EncryptionService _encryptionService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ApplicationDbContext context,
            IPasswordHasher<AppUser> passwordHasher,
            EncryptionService encryptionService,
            ILogger<AccountController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool emailExists = await _context.AppUsers
                .AnyAsync(u => u.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            AppUser user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            user.EncryptedFinancialInfo = _encryptionService.Encrypt(model.FinancialInfo);

            user.FinancialInfoHmac = _encryptionService.GenerateHmac(user.EncryptedFinancialInfo);

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New user registered successfully. UserId: {UserId}", user.Id);

            TempData["Success"] = "Registration successful. Data stored securely.";

            return RedirectToAction("Register");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _context.AppUsers.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            AppUser? user = await _context.AppUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool isValid = _encryptionService.VerifyHmac(
                user.EncryptedFinancialInfo,
                user.FinancialInfoHmac
            );

            ViewBag.HmacStatus = isValid ? "Valid - Data not modified" : "Invalid - Data may be tampered";

            ViewBag.DecryptedFinancialInfo = isValid
                ? _encryptionService.Decrypt(user.EncryptedFinancialInfo)
                : "Cannot decrypt because HMAC validation failed";

            return View(user);
        }
    }
}