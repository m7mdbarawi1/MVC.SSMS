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
    [Authorize(Roles = "3")]
    public class TeachersController : Controller
    {
        private readonly SSMSContext _context;

        public TeachersController(SSMSContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var sSMSContext = _context.Teachers.Include(t => t.Material).Include(t => t.User);
            return View(await sSMSContext.ToListAsync());
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Material)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherId,UserId,MaterialId,Gender,FullNameArabic,FullNameEnglish")] Teacher teacher)
        {
            // Check for duplicate TeacherId (manual PK check)
            if (_context.Teachers.Any(t => t.TeacherId == teacher.TeacherId))
            {
                ModelState.AddModelError("TeacherId", $"A teacher with ID {teacher.TeacherId} already exists.");
            }

            // Validate unique MaterialId
            if (_context.Teachers.Any(t => t.MaterialId == teacher.MaterialId))
            {
                ModelState.AddModelError("MaterialId", "This material is already assigned to another teacher.");
            }

            // Validate UserId uniqueness across Students and Teachers
            if (_context.Students.Any(s => s.UserId == teacher.UserId) ||
                _context.Teachers.Any(t => t.UserId == teacher.UserId))
            {
                ModelState.AddModelError("UserId", "This user is already assigned to a student or teacher.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", teacher.MaterialId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teacher.UserId);
                return View(teacher);
            }

            try
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("TeacherId", "A teacher with this ID already exists in the database.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred while creating the teacher.");
                }

                ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", teacher.MaterialId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teacher.UserId);
                return View(teacher);
            }
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", teacher.MaterialId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teacher.UserId);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,UserId,MaterialId,Gender,FullNameArabic,FullNameEnglish")] Teacher teacher)
        {
            if (id != teacher.TeacherId)
                return NotFound();

            // Validate MaterialId uniqueness
            if (_context.Teachers.Any(t => t.MaterialId == teacher.MaterialId && t.TeacherId != teacher.TeacherId))
            {
                ModelState.AddModelError("MaterialId", "This material is already assigned to another teacher.");
            }

            // Validate UserId uniqueness
            if (_context.Students.Any(s => s.UserId == teacher.UserId) ||
                _context.Teachers.Any(t => t.UserId == teacher.UserId && t.TeacherId != teacher.TeacherId))
            {
                ModelState.AddModelError("UserId", "This user is already assigned to a student or teacher.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", teacher.MaterialId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teacher.UserId);
                return View(teacher);
            }

            try
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(teacher.TeacherId))
                    return NotFound();
                else
                    throw;
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("TeacherId", "A teacher with this ID already exists in the database.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred while updating the teacher.");
                }

                ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", teacher.MaterialId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teacher.UserId);
                return View(teacher);
            }
        }



        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Material)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherId == id);
        }
    }
}
