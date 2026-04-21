using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Worktastic.Data;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    [Authorize]
    public class JobPostingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if(User.IsInRole("Admin"))
            {
                var allPostings = _context.JobPosts.ToList();
                return View(allPostings);
            }

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
                if (User.Identity.Name != jobFromDb.OwnerName && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                return View(jobFromDb);
            }
            return View(new JobPosting());
        }

        [HttpPost]
        public IActionResult CreateEdit(JobPosting jobPosting, IFormFile? companyLogoFile)
        {
            Console.WriteLine("POST CreateEdit wurde erreicht");
            Console.WriteLine($"Id: {jobPosting.Id}");
            Console.WriteLine($"JobTitle: {jobPosting.JobTitle}");
            Console.WriteLine($"JobLocation: {jobPosting.JobLocation}");
            Console.WriteLine($"ContactName: {jobPosting.ContactName}");
            Console.WriteLine($"ContactEmail: {jobPosting.ContactEmail}");
            Console.WriteLine($"companyLogoFile null? {companyLogoFile == null}");

            jobPosting.OwnerName = User.Identity?.Name;

            if (jobPosting.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "Das Startdatum darf nicht in der Vergangenheit liegen.");
            }

            if (!ModelState.IsValid)

            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"{entry.Key}: {error.ErrorMessage}");
                    }
                }

                return View("CreateEditForm", jobPosting);
            }

            if (jobPosting.Id == 0)
            {
                if (companyLogoFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        companyLogoFile.CopyTo(memoryStream);
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

                if (companyLogoFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        companyLogoFile.CopyTo(memoryStream);
                        jobFromDb.CompanyLogo = memoryStream.ToArray();
                    }
                }

                //jobFromDb.Ownername = jobPosting.OwnerName; nicht machen: weil Eigentümer wird im Nachhinein geändert,
                //bei Rollen wird dann im schlimmsten Fall der admin als Eigentümer eingetragen

            }
            Console.WriteLine("SaveChanges wird jetzt ausgeführt");
            _context.SaveChanges();
            Console.WriteLine("SaveChanges wurde ausgeführt");
            return RedirectToAction("Index");
        }
        [HttpPost]
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

            return Ok();
        }
    }
}