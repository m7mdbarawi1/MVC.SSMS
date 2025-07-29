using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSMS.Data;
using SSMS.Models;

namespace SSMS.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly SSMSContext _context;

        public UsersController(SSMSContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "3")]
        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
        [Authorize(Roles = "3")]
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [Authorize(Roles = "3")]
        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create([Bind("UserId,FullName,UserName,Password,UserType")] User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            try
            {
                // Optional manual PK check if UserId is user-supplied
                if (_context.Users.Any(u => u.UserId == user.UserId))
                {
                    ModelState.AddModelError("UserId", $"User with ID {user.UserId} already exists.");
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                // Handle SQL Server duplicate PK / constraint violations
                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("UserId", "A user with this ID already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred. Please try again.");
                }

                return View(user);
            }
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,UserName,Password,UserType")] User user)
        {
            if (id != user.UserId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(user);

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                    return NotFound();
                else
                    throw; // If it's a real concurrency issue
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                // Handle duplicate PK or constraint violations
                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("UserId", "A user with this ID already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred while updating the user.");
                }

                return View(user);
            }
        }


        // GET: Users/Delete/5
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var userId = int.Parse(User.FindFirst("UserID")!.Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var userId = int.Parse(User.FindFirst("UserID")!.Value);
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
                return NotFound();

            // Only update safe fields
            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.Success = "Profile updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error updating profile.");
            }

            return View(existingUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = int.Parse(User.FindFirst("UserID")!.Value);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Logout", "Account");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
