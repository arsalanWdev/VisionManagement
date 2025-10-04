using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisionManagement.Models;

namespace VisionManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly VisionManagementContext _context;

        public EvaluationsController(VisionManagementContext context)
        {
            _context = context;
        }

        // ================== EVALUATOR: GET ASSIGNED PROJECTS ==================
        [HttpGet("assigned")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAssignedProjects()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var projects = await _context.ProjectAssignments
                .Where(a => a.UserId == userId)
                .Include(a => a.Project)
                .Select(a => a.Project)
                .ToListAsync();

            return Ok(projects);
        }

        // ================== EVALUATOR: SUBMIT EVALUATION ==================
        [HttpPost("{projectId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubmitEvaluation(int projectId, [FromBody] EvaluationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // check if assigned
            var assigned = await _context.ProjectAssignments
                .AnyAsync(a => a.ProjectId == projectId && a.UserId == userId);

            if (!assigned)
                return Forbid("You are not assigned to this project.");

            // check if already evaluated
            var existing = await _context.Evaluations
                .AnyAsync(e => e.ProjectId == projectId && e.UserId == userId);

            if (existing)
                return BadRequest("You have already evaluated this project. Use PUT to update.");

            var evaluation = new Evaluation
            {
                ProjectId = projectId,
                UserId = userId,
                ProblemSignificance = dto.ProblemSignificance,
                InnovationTechnical = dto.InnovationTechnical,
                MarketScalability = dto.MarketScalability,
                TractionImpact = dto.TractionImpact,
                BusinessModel = dto.BusinessModel,
                TeamExecution = dto.TeamExecution,
                EthicsEquity = dto.EthicsEquity,
                Strengths = dto.Strengths,
                Weaknesses = dto.Weaknesses,
                Recommendation = dto.Recommendation,
                EvaluatedAt = DateTime.UtcNow
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Evaluation submitted successfully", evaluation });
        }

        // ================== EVALUATOR: EDIT/UPDATE EVALUATION ==================
        [HttpPut("{projectId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditEvaluation(int projectId, [FromBody] EvaluationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.ProjectId == projectId && e.UserId == userId);

            if (evaluation == null)
                return NotFound("You have not submitted an evaluation for this project yet.");

            // Update fields
            evaluation.ProblemSignificance = dto.ProblemSignificance;
            evaluation.InnovationTechnical = dto.InnovationTechnical;
            evaluation.MarketScalability = dto.MarketScalability;
            evaluation.TractionImpact = dto.TractionImpact;
            evaluation.BusinessModel = dto.BusinessModel;
            evaluation.TeamExecution = dto.TeamExecution;
            evaluation.EthicsEquity = dto.EthicsEquity;
            evaluation.Strengths = dto.Strengths;
            evaluation.Weaknesses = dto.Weaknesses;
            evaluation.Recommendation = dto.Recommendation;
            evaluation.EvaluatedAt = DateTime.UtcNow; // mark update time

            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Evaluation updated successfully", evaluation });
        }

        // ================== EVALUATOR: GET MY EVALUATIONS ==================
        [HttpGet("my")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyEvaluations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var evaluations = await _context.Evaluations
                .Where(e => e.UserId == userId)
                .Include(e => e.Project)
                .ToListAsync();

            return Ok(evaluations);
        }

        // ================== SUPERADMIN/FSO: GET ALL EVALUATIONS OF A PROJECT ==================
        [HttpGet("project/{projectId}")]
        [Authorize(Roles = "SuperAdmin,FSO")]
        public async Task<IActionResult> GetProjectEvaluations(int projectId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.ProjectId == projectId)
                .Include(e => e.User)
                .Include(e => e.Project)
                .ToListAsync();

            return Ok(evaluations);
        }
    }

    // DTO for evaluation input
    public class EvaluationDto
    {
        public int ProblemSignificance { get; set; }
        public int InnovationTechnical { get; set; }
        public int MarketScalability { get; set; }
        public int TractionImpact { get; set; }
        public int BusinessModel { get; set; }
        public int TeamExecution { get; set; }
        public int EthicsEquity { get; set; }

        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Recommendation { get; set; }
    }
}
