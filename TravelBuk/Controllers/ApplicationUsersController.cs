using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TravelBuk.Areas.Identity.Pages.Account;
using TravelBuk.Data;
using TravelBuk.Models;

namespace TravelBuk.Controllers {
    [Authorize(Roles = "Admin")]
    public class ApplicationUsersController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public ApplicationUsersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index() {
            return View(await _context.ApplicationUser.OrderByDescending(a => a.CreatedOn).ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string id) {
            if (id == null) {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null) {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create() {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,ConfirmPassword")] ApplicationUserInputModel input) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser { FirstName = input.FirstName, LastName = input.LastName, UserName = input.Email, Email = input.Email, IsApproved = false, CreatedOn = DateTime.Now };
                var result = await _userManager.CreateAsync(user, input.Password);
                if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _userManager.AddToRoleAsync(user, "Admin");
                    ModelState.AddModelError("AccountInactive", "Your Account is not activated yet. Please wait until administrator approves your account.");

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(input);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string id) {
            if (id == null) {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null) {
                return NotFound();
            }
            return View(new ApplicationUserInputModel { Id = applicationUser.Id, FirstName = applicationUser.FirstName, LastName = applicationUser.LastName, Password = "Unchanged", ConfirmPassword = "Unchanged", Email = applicationUser.Email, IsApproved = applicationUser.IsApproved });
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword,IsApproved")] ApplicationUserInputModel input) {
            if (id != input.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync(input.Id);

                if (user.FirstName != input.FirstName) {
                    user.FirstName = input.FirstName;
                }

                if (user.LastName != input.LastName) {
                    user.LastName = input.LastName;
                }

                var approvedChanged = false;
                if (user.IsApproved != input.IsApproved) {
                    user.IsApproved = input.IsApproved;
                    approvedChanged = true;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded) {
                    _logger.LogInformation("User updated.");

                    if (approvedChanged) {
                        if (input.IsApproved) {
                            await _emailSender.SendEmailAsync(input.Email, "Account Activated",
                                $"Your account has been activated.");
                        } else {
                            await _emailSender.SendEmailAsync(input.Email, "Account Deactivated",
                                $"Your account has been deactivated, please contact Administrator.");
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(input);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id) {
            if (id == null) {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null) {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) {
            var applicationUser = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUser.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id) {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
