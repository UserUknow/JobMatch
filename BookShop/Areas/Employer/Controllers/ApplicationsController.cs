﻿using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer,Customer,Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApplicationModels
        [Authorize(Roles = "Employer,Customer,Admin")]
        public async Task<IActionResult> Index()
        {
            //var applications = await _context.ApplicationModels.Where(a => a.status == null).ToListAsync();
            var applications = await _context.ApplicationModels.ToListAsync();
            return View(applications);
        }

        // GET: ApplicationModels/Details/5
        [Authorize(Roles = "Employer,Customer,Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationModel = await _context.ApplicationModels.FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (applicationModel == null)
            {
                return NotFound();
            }

            return View(applicationModel);
        }

        // GET: ApplicationModels/Create
        [Authorize(Roles = "Employer,Customer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer,Customer")]
        public async Task<IActionResult> Create([Bind("ApplicationId,JobListingId,Message,Description")] ApplicationModel model)
        {
                var applications = new ApplicationModel { 
                    ApplicationId = Guid.NewGuid().ToString(), 
                    Description = model.Description,
                    JobListingId = model.JobListingId,
                    Message  = model.Message,
                };
                _context.Add(applications);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: ApplicationModels/Edit/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationModel = await _context.ApplicationModels.FindAsync(id);
            if (applicationModel == null)
            {
                return NotFound();
            }
            return View(applicationModel);
        }

        // POST: ApplicationModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(string id, [Bind("ApplicationId,JobListingId,Message,Description,DisplayOrder")] ApplicationModel applicationModel)
        {
            if (id != applicationModel.ApplicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationModelExists(applicationModel.ApplicationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicationModel);
        }

        // GET: ApplicationModels/Delete/5
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationModel = await _context.ApplicationModels.FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (applicationModel == null)
            {
                return NotFound();
            }

            return View(applicationModel);
        }

        // POST: ApplicationModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationModel = await _context.ApplicationModels.FindAsync(id);
            if (applicationModel != null)
            {
                _context.ApplicationModels.Remove(applicationModel);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationModelExists(string id)
        {
            return _context.ApplicationModels.Any(e => e.ApplicationId == id);
        }

        [Authorize(Roles = "Employer,Customer")]
        public async Task<IActionResult> Approve(string status, string id)
        {
            var application =  await _context.ApplicationModels.FirstOrDefaultAsync(a => a.ApplicationId == id);

            if(application != null)
            {
                try
                {
                    application.status = bool.Parse(status);
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
