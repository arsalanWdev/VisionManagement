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
                    u.Email,
                    Role = u.Role.RoleName
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
    }

    public class AssignProjectDto
    {
        public int ProjectId { get; set; }
        public List<int> UserIds { get; set; }
    }
}
