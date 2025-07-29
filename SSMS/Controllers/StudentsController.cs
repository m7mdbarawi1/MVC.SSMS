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
    public class StudentsController : Controller
    {
        private readonly SSMSContext _context;

        public StudentsController(SSMSContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var sSMSContext = _context.Students.Include(s => s.Class).Include(s => s.User);
            return View(await sSMSContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,ClassId,UserId,Gender,FullNameArabic,FullNameEnglish,Age")] Student student)
        {
            // Check for duplicate StudentId
            if (_context.Students.Any(s => s.StudentId == student.StudentId))
            {
                ModelState.AddModelError("StudentId", $"A student with ID {student.StudentId} already exists.");
            }

            // Check for duplicate UserId across Students and Teachers
            if (_context.Students.Any(s => s.UserId == student.UserId) ||
                _context.Teachers.Any(t => t.UserId == student.UserId))
            {
                ModelState.AddModelError("UserId", "This user is already assigned to a student or teacher.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", student.ClassId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", student.UserId);
                return View(student);
            }

            try
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("StudentId", "A student with this ID already exists in the database.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred while creating the student.");
                }

                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", student.ClassId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", student.UserId);
                return View(student);
            }
        }


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", student.ClassId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", student.UserId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,ClassId,UserId,Gender,FullNameArabic,FullNameEnglish,Age")] Student student)
        {
            if (id != student.StudentId)
                return NotFound();

            // Validate UserId uniqueness
            if (_context.Teachers.Any(t => t.UserId == student.UserId) ||
                _context.Students.Any(s => s.UserId == student.UserId && s.StudentId != student.StudentId))
            {
                ModelState.AddModelError("UserId", "This user is already assigned to a student or teacher.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", student.ClassId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", student.UserId);
                return View(student);
            }

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudentId))
                    return NotFound();
                else
                    throw;
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                if (message.Contains("PRIMARY KEY") || message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("StudentId", "A student with this ID already exists in the database.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected database error occurred while updating the student.");
                }

                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", student.ClassId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", student.UserId);
                return View(student);
            }
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
