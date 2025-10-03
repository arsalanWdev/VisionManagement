using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisionManagement.Models
{
    public class ProjectAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
