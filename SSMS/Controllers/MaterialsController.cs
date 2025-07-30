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
    public class MaterialsController : Controller
    {
        private readonly SSMSContext _context;

        public MaterialsController(SSMSContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Materials.ToListAsync());
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            if (material == null) return NotFound();

            return View(material);
        }

        // GET: Materials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Materials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialNameArabic,MaterialNameEnglish")] Material material)
        {
            // Optional: Check for duplicate Arabic name
            if (_context.Materials.Any(m => m.MaterialNameArabic == material.MaterialNameArabic))
            {
                ModelState.AddModelError("MaterialNameArabic", "A material with this Arabic name already exists.");
            }

            if (!ModelState.IsValid)
                return View(material);

            try
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "An unexpected database error occurred while creating the material.");
                return View(material);
            }
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materials.FindAsync(id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST: Materials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialId,MaterialNameArabic,MaterialNameEnglish")] Material material)
        {
            if (id != material.MaterialId) return NotFound();

            // Duplicate name check (excluding itself)
            if (_context.Materials.Any(m => m.MaterialNameArabic == material.MaterialNameArabic && m.MaterialId != material.MaterialId))
            {
                ModelState.AddModelError("MaterialNameArabic", "Another material with this Arabic name already exists.");
            }

            if (!ModelState.IsValid)
                return View(material);

            try
            {
                _context.Update(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(material.MaterialId)) return NotFound();
                else throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "An unexpected database error occurred while updating the material.");
                return View(material);
            }
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;

                if (message.Contains("FOREIGN KEY"))
                {
                    TempData["ErrorMessage"] = "Cannot delete this material because it is assigned to teachers or marks.";
                }
                else
                {
                    TempData["ErrorMessage"] = "An unexpected error occurred while deleting the material.";
                }

                return RedirectToAction(nameof(Index));
            }
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.MaterialId == id);
        }
    }
}
