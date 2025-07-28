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
            var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return Unauthorized("Teacher not found.");

            var marks = await _context.Marks
                .Include(m => m.Class)
                .Include(m => m.Material)
                .Include(m => m.Student)
                .Where(m => m.MaterialId == teacher.MaterialId)
                .ToListAsync();

            return View(marks);
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
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return Unauthorized();

            var teacherMaterialId = teacher.MaterialId;

            // ✅ Get class IDs that have this material (from ClassMaterials join table)
            var classIdsWithThisMaterial = await _context.Classes
                .Where(c => c.Materials.Any(m => m.MaterialId == teacherMaterialId))
                .Select(c => c.ClassId)
                .ToListAsync();

            // ✅ Filter classes that are assigned this material
            var filteredClasses = await _context.Classes
                .Where(c => classIdsWithThisMaterial.Contains(c.ClassId))
                .ToListAsync();

            // ✅ Filter students who belong to these classes
            var filteredStudents = await _context.Students
                .Where(s => classIdsWithThisMaterial.Contains(s.ClassId))
                .ToListAsync();

            ViewData["ClassId"] = new SelectList(filteredClasses, "ClassId", "ClassId");
            ViewData["MaterialId"] = new SelectList(_context.Materials.Where(m => m.MaterialId == teacherMaterialId), "MaterialId", "MaterialId");
            ViewData["StudentId"] = new SelectList(filteredStudents, "StudentId", "StudentId");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Create([Bind("StudentId,ClassId,MaterialId,Marks")] Mark mark)
        {
            var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return Unauthorized();

            if (mark.MaterialId != teacher.MaterialId)
            {
                ModelState.AddModelError("", "Unauthorized to add mark for this material.");
            }

            if (MarkExists(mark.StudentId, mark.ClassId, mark.MaterialId))
            {
                ModelState.AddModelError("", "This mark already exists.");
            }

            // ✅ Check: Material is assigned to the selected class
            bool isMaterialAssignedToClass = await _context.Classes
                .AnyAsync(c =>
                    c.ClassId == mark.ClassId &&
                    c.Materials.Any(m => m.MaterialId == mark.MaterialId));
            if (!isMaterialAssignedToClass)
            {
                ModelState.AddModelError("", "The selected material is not assigned to the selected class.");
            }

            // ✅ Check: Student is in the selected class
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == mark.StudentId);
            if (student == null || student.ClassId != mark.ClassId)
            {
                ModelState.AddModelError("", "The selected student does not belong to the selected class.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", mark.ClassId);
                ViewData["MaterialId"] = new SelectList(_context.Materials.Where(m => m.MaterialId == teacher.MaterialId), "MaterialId", "MaterialId", mark.MaterialId);
                ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", mark.StudentId);
                return View(mark);
            }

            _context.Add(mark);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Edit(int studentId, int classId, int materialId, [Bind("StudentId,ClassId,MaterialId,Marks")] Mark mark)
        {
            if (studentId != mark.StudentId || classId != mark.ClassId || materialId != mark.MaterialId)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return Unauthorized();

            if (mark.MaterialId != teacher.MaterialId)
            {
                ModelState.AddModelError("", "Unauthorized to edit mark for this material.");
            }

            // ✅ Check: Material is assigned to the selected class
            bool isMaterialAssignedToClass = await _context.Classes
                .AnyAsync(c =>
                    c.ClassId == mark.ClassId &&
                    c.Materials.Any(m => m.MaterialId == mark.MaterialId));
            if (!isMaterialAssignedToClass)
            {
                ModelState.AddModelError("", "The selected material is not assigned to the selected class.");
            }

            // ✅ Check: Student is in the selected class
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == mark.StudentId);
            if (student == null || student.ClassId != mark.ClassId)
            {
                ModelState.AddModelError("", "The selected student does not belong to the selected class.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", mark.ClassId);
                ViewData["MaterialId"] = new SelectList(_context.Materials.Where(m => m.MaterialId == teacher.MaterialId), "MaterialId", "MaterialId", mark.MaterialId);
                ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", mark.StudentId);
                return View(mark);
            }

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
