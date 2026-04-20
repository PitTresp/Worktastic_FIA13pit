namespace Worktastic.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? JobDescription { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int? Salary { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? ContactWebsite { get; set; }
        public byte[]? CompanyLogo { get; set; }

        public string? OwnerName { get; set; }
    }
}
