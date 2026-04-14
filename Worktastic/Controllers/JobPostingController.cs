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
            return View();
        }

        public IActionResult CreateEditForm(int id)
        {
            //TODO
            return View();
        }

        public IActionResult CreateEdit(JobPosting jobPosting, IFormFile CompanyLogo)
        {
            jobPosting.OwnerName = User.Identity.Name;
            if(CompanyLogo != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    CompanyLogo.CopyTo(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    jobPosting.CompanyLogo = byteArray;
                }
            }
            
            if(jobPosting == null)
            {
                return NotFound();
            }
            if(jobPosting.Id == 0)
                _context.JobPosts.Add(jobPosting);
            else
            {
               var jobFromDB = _context.JobPosts.SingleOrDefault(x => x.Id == jobPosting.Id);
                if(jobFromDB == null)
                {
                    return NotFound();
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
