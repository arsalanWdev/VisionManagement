using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisionManagement.Models;

namespace VisionManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly VisionManagementContext _context;

        public ProjectsController(VisionManagementContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        [Authorize(Roles = "FSO,SuperAdmin")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        [Authorize(Roles = "FSO,SuperAdmin")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            return Ok(project);
        }

        // ================= CREATE =================
        [HttpPost("create")]
        [Authorize(Roles = "FSO,SuperAdmin")]
        public async Task<IActionResult> CreateProject([FromForm] ProjectUploadModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = new Project
            {
                Timestamp = DateTime.UtcNow,
                Username = model.Username,
                StartupName = model.StartupName,
                FounderName = model.FounderName,
                Email = model.Email,
                Phone = model.Phone,
                WebsiteLink = model.WebsiteLink,
                MobileAppLink = model.MobileAppLink,
                StartupDescription = model.StartupDescription,
                StartupStatus = model.StartupStatus,
                SpotlightReason = model.SpotlightReason
            };

            // Save uploaded files
            project.StartupLogo = await SaveFile(model.StartupLogo, "logos");
            project.FounderPhoto = await SaveFile(model.FounderPhoto, "founders");
            project.DefaultVideo = await SaveFile(model.DefaultVideo, "videos");
            project.PitchVideo = await SaveFile(model.PitchVideo, "videos");
            project.Image1 = await SaveFile(model.Image1, "images");
            project.Image2 = await SaveFile(model.Image2, "images");
            project.Image3 = await SaveFile(model.Image3, "images");

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Project created successfully", project });
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        [Authorize(Roles = "FSO,SuperAdmin")]
        public async Task<IActionResult> UpdateProject(int id, [FromForm] ProjectUploadModel model)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            // Update only provided fields
            project.Username = model.Username ?? project.Username;
            project.StartupName = model.StartupName ?? project.StartupName;
            project.FounderName = model.FounderName ?? project.FounderName;
            project.Email = model.Email ?? project.Email;
            project.Phone = model.Phone ?? project.Phone;
            project.WebsiteLink = model.WebsiteLink ?? project.WebsiteLink;
            project.MobileAppLink = model.MobileAppLink ?? project.MobileAppLink;
            project.StartupDescription = model.StartupDescription ?? project.StartupDescription;
            project.StartupStatus = model.StartupStatus ?? project.StartupStatus;
            project.SpotlightReason = model.SpotlightReason ?? project.SpotlightReason;

            // Replace files if new ones uploaded
            if (model.StartupLogo != null)
            {
                DeleteFileIfExists(project.StartupLogo);
                project.StartupLogo = await SaveFile(model.StartupLogo, "logos");
            }
            if (model.FounderPhoto != null)
            {
                DeleteFileIfExists(project.FounderPhoto);
                project.FounderPhoto = await SaveFile(model.FounderPhoto, "founders");
            }
            if (model.DefaultVideo != null)
            {
                DeleteFileIfExists(project.DefaultVideo);
                project.DefaultVideo = await SaveFile(model.DefaultVideo, "videos");
            }
            if (model.PitchVideo != null)
            {
                DeleteFileIfExists(project.PitchVideo);
                project.PitchVideo = await SaveFile(model.PitchVideo, "videos");
            }
            if (model.Image1 != null)
            {
                DeleteFileIfExists(project.Image1);
                project.Image1 = await SaveFile(model.Image1, "images");
            }
            if (model.Image2 != null)
            {
                DeleteFileIfExists(project.Image2);
                project.Image2 = await SaveFile(model.Image2, "images");
            }
            if (model.Image3 != null)
            {
                DeleteFileIfExists(project.Image3);
                project.Image3 = await SaveFile(model.Image3, "images");
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ Project updated successfully", project });
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        [Authorize(Roles = "FSO,SuperAdmin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found.");

            // Delete physical files
            DeleteFileIfExists(project.StartupLogo);
            DeleteFileIfExists(project.FounderPhoto);
            DeleteFileIfExists(project.DefaultVideo);
            DeleteFileIfExists(project.PitchVideo);
            DeleteFileIfExists(project.Image1);
            DeleteFileIfExists(project.Image2);
            DeleteFileIfExists(project.Image3);

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Project deleted successfully" });
        }

        // ================= FILE DOWNLOAD =================
        [HttpGet("files/{folder}/{fileName}")]
        public IActionResult GetFile(string folder, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType);
        }

        // ================= HELPERS =================
        private async Task<string?> SaveFile(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // return relative path for serving
            return $"/Uploads/{folderName}/{uniqueFileName}";
        }

        private void DeleteFileIfExists(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            // relativePath looks like "/Uploads/folder/file.ext"
            var fileName = Path.GetFileName(relativePath);
            var folder = Path.GetFileName(Path.GetDirectoryName(relativePath));

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folder)) return;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }

    // ================= UPLOAD MODEL =================
    public class ProjectUploadModel
    {
        // Text fields
        public string? Username { get; set; }
        public string? StartupName { get; set; }
        public string? FounderName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? WebsiteLink { get; set; }
        public string? MobileAppLink { get; set; }
        public string? StartupDescription { get; set; }
        public string? StartupStatus { get; set; }
        public string? SpotlightReason { get; set; }

        // File uploads
        public IFormFile? StartupLogo { get; set; }
        public IFormFile? FounderPhoto { get; set; }
        public IFormFile? DefaultVideo { get; set; }
        public IFormFile? PitchVideo { get; set; }
        public IFormFile? Image1 { get; set; }
        public IFormFile? Image2 { get; set; }
        public IFormFile? Image3 { get; set; }
    }
}
