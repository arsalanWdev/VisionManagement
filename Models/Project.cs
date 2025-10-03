using System.ComponentModel.DataAnnotations;

namespace VisionManagement.Models
{
    public class Project
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(200)]
        public string? StartupName { get; set; }

        [MaxLength(150)]
        public string? FounderName { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? WebsiteLink { get; set; }

        [MaxLength(250)]
        public string? MobileAppLink { get; set; }

        [MaxLength(1000)]
        public string? StartupDescription { get; set; }

        [MaxLength(100)]
        public string? StartupStatus { get; set; }

        [MaxLength(250)]
        public string? StartupLogo { get; set; }

        [MaxLength(250)]
        public string? ProjectDemoVideoLink { get; set; }

        [MaxLength(250)]
        public string? FounderPhoto { get; set; }

        [MaxLength(1000)]
        public string? SpotlightReason { get; set; }


        [MaxLength(250)]
        public string? DefaultVideo { get; set; }

        [MaxLength(250)]
        public string? PitchVideo { get; set; }

        [MaxLength(250)]
        public string? Image1 { get; set; }

        [MaxLength(250)]
        public string? Image2 { get; set; }

        [MaxLength(250)]
        public string? Image3 { get; set; }
    }
}
