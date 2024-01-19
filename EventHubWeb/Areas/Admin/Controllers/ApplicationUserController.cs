using EventHub.Models;
using EventHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using System.Web;

namespace EventHubWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: User
        public ActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicationUser applicationUser, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (applicationUser.PasswordHash == null)
                {
                    ModelState.AddModelError("Password", "Паролата е задължително поле.");
                    return View();
                }

                if (applicationUser.PasswordHash != confirmPassword)
                {
                    ModelState.AddModelError("Password", "Паролите не съвпадат.");
                    return View();
                }

                var user = new ApplicationUser
                {
                    UserName = applicationUser.Email,
                    Email = applicationUser.Email,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName
                };

                var result = await _userManager.CreateAsync(user, applicationUser.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, StaticDetails.Role_User);
                    var userId = await _userManager.GetUserIdAsync(user);

                    return RedirectToAction("Index");
                }
            }
            return View(applicationUser);
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model, string newPassword, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword != confirmPassword)
                    {
                        ModelState.AddModelError("PasswordHash", "Паролите не съвпадат.");
                        return View(model);
                    }

                    var passwordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                    user.PasswordHash = passwordHash;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: User/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
