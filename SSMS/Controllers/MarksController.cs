using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSMS.Models;

namespace SSMS.Controllers
{
    
    public class MarksController : Controller
    {
        private readonly SSMSContext _context;

        public MarksController(SSMSContext context)
        {
            _context = context;
        }

        // GET: Marks
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Index()
        {
            var sSMSContext = _context.Marks.Include(m => m.Class).Include(m => m.Material).Include(m => m.Student);
            return View(await sSMSContext.ToListAsync());
        }

        // GET: Marks/Details/5
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Details(int studentId, int classId, int materialId)
        {
            var mark = await _context.Marks
                .Include(m => m.Class)
                .Include(m => m.Material)
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.ClassId == classId &&
                    m.MaterialId == materialId);

            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // GET: Marks/Create
        [Authorize(Roles = "2")]
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId");
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Create([Bind("StudentId,ClassId,MaterialId,Mark1")] Mark mark)
        {
            Console.WriteLine($"TRYING TO CREATE: S:{mark.StudentId} C:{mark.ClassId} M:{mark.MaterialId} => {mark.Mark1}");

            if (MarkExists(mark.StudentId, mark.ClassId, mark.MaterialId))
            {
                ModelState.AddModelError("", "This mark already exists.");
                Console.WriteLine("DUPLICATE");
                return View(mark);
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("MODEL STATE INVALID");
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"Error for {modelError.Key}: {error.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(mark);
                await _context.SaveChangesAsync();
                Console.WriteLine("SAVED");
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", mark.ClassId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", mark.MaterialId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", mark.StudentId);
            return View(mark);
        }


        // GET: Marks/Edit
        public async Task<IActionResult> Edit(int studentId, int classId, int materialId)
        {
            var mark = await _context.Marks.FindAsync(studentId, classId, materialId);
            if (mark == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", mark.ClassId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", mark.MaterialId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", mark.StudentId);
            return View(mark);
        }

        // POST: Marks/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int studentId, int classId, int materialId, [Bind("StudentId,ClassId,MaterialId,Mark1")] Mark mark)
        {
            if (studentId != mark.StudentId || classId != mark.ClassId || materialId != mark.MaterialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(studentId, classId, materialId))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            // Reload dropdowns
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", mark.ClassId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "MaterialId", "MaterialId", mark.MaterialId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", mark.StudentId);
            return View(mark);
        }

        // GET: Marks/Delete/5
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Delete(int studentId, int classId, int materialId)
        {
            var mark = await _context.Marks
                .Include(m => m.Class)
                .Include(m => m.Material)
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.ClassId == classId &&
                    m.MaterialId == materialId);

            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [Authorize(Roles = "2")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteConfirmed(int studentId, int classId, int materialId)
        {
            var mark = await _context.Marks.FindAsync(studentId, classId, materialId);
            if (mark != null)
            {
                _context.Marks.Remove(mark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "1")]
        public async Task<IActionResult> MyMarks()
        {
            var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
                return NotFound("Student not found for the current user.");

            var marks = await _context.Marks
                .Include(m => m.Class)
                .Include(m => m.Material)
                .Where(m => m.StudentId == student.StudentId)
                .ToListAsync();

            return View(marks);
        }

        private bool MarkExists(int studentId, int classId, int materialId)
        {
            return _context.Marks.Any(e =>
                e.StudentId == studentId &&
                e.ClassId == classId &&
                e.MaterialId == materialId);
        }

    }
}
