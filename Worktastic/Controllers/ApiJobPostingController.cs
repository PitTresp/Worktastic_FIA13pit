using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SQLitePCL;
using Worktastic.Data;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    [Route("api/jobposting")]
    [ApiController]

    public class ApiJobPostingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApiJobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var jobsFromDb = _context.JobPosts.ToArray();
            return Ok(jobsFromDb);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            var jobFromDb = _context.JobPosts.SingleOrDefault(x => x.Id == id);
            if (jobFromDb == null) return NotFound();
            return Ok(jobFromDb);
        }

        [HttpPost("Create")]
        public IActionResult Create(JobPosting job)
        {
            //Todo eintragen in Database
        }
    }
}
