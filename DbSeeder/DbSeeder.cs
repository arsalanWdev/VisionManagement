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
        
            // Seed users if they don't exist
            if (!_context.Users.Any(u => u.Username == "SuperAdmin"))
            {
                _context.Users.Add(new User
                {
                    Username = "SuperAdmin",
                    Email = "superadmin@vision.com",
                    PasswordHash = HashPassword("SuperAdmin@123"),
                    RoleId = 2 // TND role
                });
            }

            if (!_context.Users.Any(u => u.Username == "FSO"))
            {
                _context.Users.Add(new User
                {
                    Username = "FSO",
                    Email = "fso@vision.com",
                    PasswordHash = HashPassword("FSO@123"),
                    RoleId = 3 // CAH role
                });
            }

            // Save changes
            _context.SaveChanges();
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
