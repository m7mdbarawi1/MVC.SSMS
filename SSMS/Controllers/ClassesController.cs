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
    public class ClassesController : Controller
    {
        
        private readonly SSMSContext _context;

        public ClassesController(SSMSContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Materials)
                    .ThenInclude(m => m.Teacher) // ✅ this loads the teacher
                .ToListAsync();

            return View(classes);
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            ViewBag.Materials = new MultiSelectList(_context.Materials, "MaterialId", "MaterialNameArabic");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class @class, int[] selectedMaterialIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Materials = new MultiSelectList(_context.Materials, "MaterialId", "MaterialNameArabic", selectedMaterialIds);
                return View(@class);
            }

            try
            {
                // ✅ Assign selected materials
                var materials = await _context.Materials
                    .Where(m => selectedMaterialIds.Contains(m.MaterialId))
                    .ToListAsync();
                @class.Materials = materials;

                // ✅ Add new class (ID will be auto-generated)
                _context.Classes.Add(@class);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // ✅ Handle unexpected DB errors
                ModelState.AddModelError("", "An unexpected database error occurred. Please try again.");
                ViewBag.Materials = new MultiSelectList(_context.Materials, "MaterialId", "MaterialNameArabic", selectedMaterialIds);
                return View(@class);
            }
        }


        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
            .Include(c => c.Materials)
            .FirstOrDefaultAsync(c => c.ClassId == id);

            if (@class == null)
            {
                return NotFound();
            }
            
            ViewBag.Materials = new MultiSelectList(
            _context.Materials,
            "MaterialId",
            "MaterialNameArabic",
            @class.Materials.Select(m => m.MaterialId) );
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Class @class, int[] selectedMaterialIds)
        {
            if (id != @class.ClassId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Materials = new MultiSelectList(_context.Materials, "MaterialId", "MaterialNameArabic", selectedMaterialIds);
                return View(@class);
            }

            try
            {
                var existingClass = await _context.Classes
                    .Include(c => c.Materials)
                    .FirstOrDefaultAsync(c => c.ClassId == id);

                if (existingClass == null)
                    return NotFound();

                // ✅ Update fields
                existingClass.ClassNameArabic = @class.ClassNameArabic;
                existingClass.ClassNameEnglish = @class.ClassNameEnglish;

                // ✅ Update materials
                existingClass.Materials.Clear();
                var materials = await _context.Materials
                    .Where(m => selectedMaterialIds.Contains(m.MaterialId))
                    .ToListAsync();
                foreach (var mat in materials)
                    existingClass.Materials.Add(mat);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(@class.ClassId)) return NotFound();
                else throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "An unexpected database error occurred while updating the class.");
                ViewBag.Materials = new MultiSelectList(_context.Materials, "MaterialId", "MaterialNameArabic", selectedMaterialIds);
                return View(@class);
            }
        }



        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ClassMaterials()
        {
            var classMaterials = await _context.Classes
                .Include(c => c.Materials)
                .ToListAsync();

            return View(classMaterials);
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }
    }
}
