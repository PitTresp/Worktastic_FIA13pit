using Microsoft.AspNetCore.Mvc;
using Worktastic.Data;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    public class JobPostingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var jobsFromDb = _context.JobPosts.Where(x => x.OwnerName == User.Identity.Name).ToList();
            return View(jobsFromDb);
        }

        public IActionResult CreateEditForm(int id)
        {
            if (id != 0)
            {
                var jobFromDb = _context.JobPosts.SingleOrDefault(x => x.Id == id);
                if (jobFromDb == null)
                {
                    return NotFound();
                }
                return View(jobFromDb);
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateEdit(JobPosting jobPosting, IFormFile? CompanyLogo)
        {
            jobPosting.OwnerName = User.Identity?.Name;

            if (jobPosting.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "Das Startdatum darf nicht in der Vergangenheit liegen.");
            }

            if (!ModelState.IsValid)
            {
                return View("CreateEditForm", jobPosting);
            }

            if (jobPosting.Id == 0)
            {
                if (CompanyLogo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        CompanyLogo.CopyTo(memoryStream);
                        jobPosting.CompanyLogo = memoryStream.ToArray();
                    }
                }

                _context.JobPosts.Add(jobPosting);
            }
            else
            {
                var jobFromDb = _context.JobPosts.SingleOrDefault(x => x.Id == jobPosting.Id);

                if (jobFromDb == null)
                {
                    return NotFound();
                }

                if (jobFromDb.OwnerName != User.Identity?.Name)
                {
                    return Forbid();
                }

                jobFromDb.JobTitle = jobPosting.JobTitle;
                jobFromDb.JobLocation = jobPosting.JobLocation;
                jobFromDb.JobDescription = jobPosting.JobDescription;
                jobFromDb.Salary = jobPosting.Salary;
                jobFromDb.StartDate = jobPosting.StartDate;
                jobFromDb.ContactName = jobPosting.ContactName;
                jobFromDb.ContactWebsite = jobPosting.ContactWebsite;
                jobFromDb.ContactEmail = jobPosting.ContactEmail;
                jobFromDb.ContactPhone = jobPosting.ContactPhone;
                jobFromDb.OwnerName = User.Identity?.Name;

                if (CompanyLogo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        CompanyLogo.CopyTo(memoryStream);
                        jobFromDb.CompanyLogo = memoryStream.ToArray();
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return BadRequest();

            var jobFromDb = _context.JobPosts.SingleOrDefault(x => x.Id == id);

            if (jobFromDb == null)
                return NotFound();

            if (jobFromDb.OwnerName != User.Identity?.Name)
                return Forbid();

            _context.JobPosts.Remove(jobFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}