﻿using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public CustomerController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            _context = context;
        }



        // GET: CustomerController

        public IActionResult Index()
        {
            var jobListings = _context.JobListingModels.ToList();
            return View(jobListings);
        }

        public IActionResult JobDetails(string id)
        {
            var job = _context.JobListingModels.FirstOrDefault(j => j.JobListingId == id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }
        [Authorize(Roles = "Employer,Customer")]
        public IActionResult Apply(string JobListingId)
        {
            ViewBag.JobListingId = JobListingId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplicationModel model)
        {
            var applications = new ApplicationModel
            {
                ApplicationId = Guid.NewGuid().ToString(),
                Description = model.Description,
                JobListingId = model.JobListingId,
                Message = model.Message,
                status = null,
                ImageUrl = model.ImageUrl,
            };
            //var displayOrder = _context.ApplicationModels.Where(a => a.JobListingId == 1).Count();
            _context.Add(applications);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Customer")]
        public IActionResult Edit(string id)
        {
            var job = _context.JobListingModels.FirstOrDefault(j => j.JobListingId == id);
            if (job == null)
            {
                return NotFound();
            }
            var CategoryName = _context.Categories.FirstOrDefault(c => c.CategoryId == job.CategoryId)?.Name;
            ViewBag.CategoryName = CategoryName;
            return View(job);
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Edit(int id, JobListingModel model)
        {
            var job = await _context.JobListingModels.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            job.Title = model.Title;
            job.Description = model.Description;
            job.Location = model.Location;
            job.ApplicationDeadline = model.ApplicationDeadline;
            job.ImagePath = model.ImagePath;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }


}