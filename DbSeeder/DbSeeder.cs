using VisionManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace VisionManagement.DbSeeder
{
    public class DbSeeder
    {
        private readonly VisionManagementContext _context;

        public DbSeeder(VisionManagementContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // ========== SEED ROLES ==========
            if (!_context.Roles.Any())
            {
                _context.Roles.AddRange(
                    new Role { RoleName = "SuperAdmin" },
                    new Role { RoleName = "FSO" },
                    new Role { RoleName = "User" }
                );
                _context.SaveChanges();
            }

            // Get roles from DB
            var superAdminRole = _context.Roles.First(r => r.RoleName == "SuperAdmin");
            var fsoRole = _context.Roles.First(r => r.RoleName == "FSO");
            var userRole = _context.Roles.First(r => r.RoleName == "User");

            // ========== SEED USERS ==========
            if (!_context.Users.Any(u => u.Username == "SuperAdmin"))
            {
                _context.Users.Add(new User
                {
                    Username = "SuperAdmin",
                    PasswordHash = HashPassword("SuperAdmin@123"),
                    RoleId = superAdminRole.RoleId
                });
            }

            if (!_context.Users.Any(u => u.Username == "FSO"))
            {
                _context.Users.Add(new User
                {
                    Username = "FSO",
                    PasswordHash = HashPassword("FSO@123"),
                    RoleId = fsoRole.RoleId
                });
            }

            // Expert Evaluators with unique passwords
            var evaluators = new Dictionary<string, string>
            {
                { "FahadSikandar", "FS101" },
                { "MahvishSaad", "MS102" },
                { "SyedAbdulQadir", "SA103" },
                { "MuhammadAzamMughal", "MAM104" },
                { "SyedAzfarHussain", "SAH105" },
                { "KanwalMasroor", "KM106" }
            };

            foreach (var evaluator in evaluators)
            {
                if (!_context.Users.Any(u => u.Username == evaluator.Key))
                {
                    _context.Users.Add(new User
                    {
                        Username = evaluator.Key,
                        PasswordHash = HashPassword(evaluator.Value),
                        RoleId = userRole.RoleId
                    });
                }
            }

            // Save changes
            _context.SaveChanges();

            // ================== PROJECTS ==================
            if (!_context.Projects.Any())
            {
                var projects = new List<Project>
                {
                    new Project
                    {
                        Timestamp = DateTime.UtcNow,
                        Username = "startup1",
                        StartupName = "GreenTech Solutions",
                        FounderName = "Ali Khan",
                        Email = "ali@greentech.com",
                        Phone = "03001234567",
                        WebsiteLink = "https://greentech.com",
                        StartupDescription = "Sustainable energy startup focusing on solar solutions.",
                        StartupStatus = "Early Stage",
                        StartupLogo = "greentech_logo.png"
                    },
                    new Project
                    {
                        Timestamp = DateTime.UtcNow,
                        Username = "startup2",
                        StartupName = "EduNext",
                        FounderName = "Sara Ahmed",
                        Email = "sara@edunext.com",
                        Phone = "03007654321",
                        WebsiteLink = "https://edunext.com",
                        StartupDescription = "EdTech platform providing AI-powered learning.",
                        StartupStatus = "Growth",
                        StartupLogo = "edunext_logo.png"
                    }
                };

                _context.Projects.AddRange(projects);
                _context.SaveChanges();
            }
            // ================== PROJECT ASSIGNMENTS ==================
            if (!_context.ProjectAssignments.Any())
            {
                var firstProject = _context.Projects.First();
                var secondProject = _context.Projects.Skip(1).FirstOrDefault();

                var evaluator1 = _context.Users.First(u => u.Username == "FahadSikandar");
                var evaluator2 = _context.Users.First(u => u.Username == "MahvishSaad");

                _context.ProjectAssignments.AddRange(
                    new ProjectAssignment
                    {
                        ProjectId = firstProject.Id,
                        UserId = evaluator1.UserId,
                        AssignedAt = DateTime.UtcNow
                    },
                    new ProjectAssignment
                    {
                        ProjectId = secondProject.Id,
                        UserId = evaluator2.UserId,
                        AssignedAt = DateTime.UtcNow
                    }
                );
                _context.SaveChanges();
            }

            // ================== EVALUATIONS ==================
            if (!_context.Evaluations.Any())
            {
                var project = _context.Projects.First();
                var evaluator = _context.Users.First(u => u.Username == "FahadSikandar");

                _context.Evaluations.Add(new Evaluation
                {
                    ProjectId = project.Id,
                    UserId = evaluator.UserId,
                    ProblemSignificance = 8,
                    InnovationTechnical = 7,
                    MarketScalability = 9,
                    TractionImpact = 6,
                    BusinessModel = 7,
                    TeamExecution = 8,
                    EthicsEquity = 9,
                    Strengths = "Strong technical innovation and clear vision.",
                    Weaknesses = "Needs more traction and customer validation.",
                    Recommendation = "Promising project, should be shortlisted.",
                    EvaluatedAt = DateTime.UtcNow
                });

                _context.SaveChanges();
            }



        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
