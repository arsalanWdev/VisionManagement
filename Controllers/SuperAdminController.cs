using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisionManagement.Models;

namespace VisionManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly VisionManagementContext _context;

        public SuperAdminController(VisionManagementContext context)
        {
            _context = context;
        }

        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "User")
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.PasswordHash,
                    u.RoleId,
                    RoleName = u.Role.RoleName
                })
                .ToListAsync();

            if (!users.Any())
                return NotFound("No users with role 'User' found.");

            return Ok(users);
        }

        [HttpPost("assignProject")]
        public async Task<IActionResult> AssignProject([FromBody] AssignProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null)
                return NotFound("Project not found.");

            var evaluators = await _context.Users
                .Include(u => u.Role)
                .Where(u => dto.UserIds.Contains(u.UserId) && u.Role.RoleName == "User")
                .ToListAsync();

            if (!evaluators.Any())
                return BadRequest("No valid evaluators found.");

            var assignments = evaluators.Select(e => new ProjectAssignment
            {
                ProjectId = project.Id,
                UserId = e.UserId,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            await _context.ProjectAssignments.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Project '{project.StartupName}' assigned to {evaluators.Count} evaluator(s).",
                assignedUsers = evaluators.Select(e => e.Username).ToList()
            });
        }

        [HttpPut("updateAssignment")]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null)
                return NotFound("Project not found.");

            var existingAssignments = await _context.ProjectAssignments
                .Where(pa => pa.ProjectId == dto.ProjectId)
                .ToListAsync();

            var alreadyAssignedUserIds = existingAssignments.Select(pa => pa.UserId).ToHashSet();

            var newUserIds = dto.UserIds.Where(id => !alreadyAssignedUserIds.Contains(id)).ToList();

            if (!newUserIds.Any())
                return BadRequest("All selected users are already assigned to this project.");

            var evaluators = await _context.Users
                .Include(u => u.Role)
                .Where(u => newUserIds.Contains(u.UserId) && u.Role.RoleName == "User")
                .ToListAsync();

            if (!evaluators.Any())
                return BadRequest("No valid new evaluators found.");

            var newAssignments = evaluators.Select(e => new ProjectAssignment
            {
                ProjectId = project.Id,
                UserId = e.UserId,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            await _context.ProjectAssignments.AddRangeAsync(newAssignments);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Project '{project.StartupName}' updated. {newAssignments.Count} new evaluator(s) added.",
                addedUsers = evaluators.Select(e => e.Username).ToList()
            });
        }

        [HttpGet("getAssignedUsers/{projectId}")]
        public async Task<IActionResult> GetAssignedUsers(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound("Project not found.");

            var assignedUsers = await _context.ProjectAssignments
                .Include(pa => pa.User)
                .Where(pa => pa.ProjectId == projectId)
                .Select(pa => new
                {
                    pa.User.UserId,
                    pa.User.Username,
                   
                    pa.AssignedAt
                })
                .ToListAsync();

            if (!assignedUsers.Any())
                return NotFound("No users assigned to this project.");

            return Ok(new
            {
                project = project.StartupName,
                assignedUsers
            });
        }

        // ✅ Remove (Unassign) a user from a project
        [HttpDelete("unassignUser/{projectId}/{userId}")]
        public async Task<IActionResult> UnassignUser(int projectId, int userId)
        {
            var assignment = await _context.ProjectAssignments
                .FirstOrDefaultAsync(pa => pa.ProjectId == projectId && pa.UserId == userId);

            if (assignment == null)
                return NotFound($"No assignment found for user {userId} in project {projectId}.");

            _context.ProjectAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User {userId} unassigned from project {projectId} successfully."
            });
        }

    }

    // ✅ DTO
    public class AssignProjectDto
    {
        public int ProjectId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
