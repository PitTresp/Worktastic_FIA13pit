namespace Worktastic.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string? JobDescription { get; set; }  
        public string JobLocation { get; set; }
        public DateTime StartDate { get; set; }
        public int? Salary { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactWebsite { get; set; }
        public byte[]? CompanyLogo { get; set; }

        public string? OwnerName { get; set; }
    }
}
