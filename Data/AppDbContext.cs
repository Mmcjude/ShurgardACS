using Microsoft.EntityFrameworkCore;
using ShurgardACS.Models;

namespace ShurgardACS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser>     Users        { get; set; }
    public DbSet<AccessPoint> AccessPoints { get; set; }
    public DbSet<AccessLog>   AccessLogs   { get; set; }
}

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        db.Users.AddRange(
            new AppUser { FullName = "Alice Lindqvist", Username = "admin",   PasswordHash = BCryptHash("Admin@123"),  Role = "SystemAdmin",      IsActive = true },
            new AppUser { FullName = "John Smith",      Username = "jsmith",  PasswordHash = BCryptHash("Staff@123"),  Role = "Staff",            IsActive = true },
            new AppUser { FullName = "Maria Ek",        Username = "viewer1", PasswordHash = BCryptHash("View@123"),   Role = "Viewer",           IsActive = true },
            new AppUser { FullName = "Erik Guard",      Username = "guard1",  PasswordHash = BCryptHash("Guard@123"),  Role = "SecurityOfficer",  IsActive = true }
        );

        db.AccessPoints.AddRange(
            new AccessPoint { Name = "Main Entrance",  Location = "Ground floor",      Method = "PIN + RFID", IsOnline = true  },
            new AccessPoint { Name = "Floor A Gate",   Location = "Floor 1",           Method = "PIN",        IsOnline = true  },
            new AccessPoint { Name = "Floor B Gate",   Location = "Floor 2",           Method = "PIN",        IsOnline = true  },
            new AccessPoint { Name = "Loading Bay",    Location = "Ground floor, rear", Method = "RFID",      IsOnline = true  },
            new AccessPoint { Name = "Staff Office",   Location = "Ground floor",      Method = "Card",       IsOnline = true  },
            new AccessPoint { Name = "Server Room",    Location = "Basement",          Method = "Card + PIN", IsOnline = false }
        );

        db.AccessLogs.AddRange(
            new AccessLog { AccessPointName = "Main Entrance", CustomerRef = "#2041", EventTime = DateTime.Now.AddMinutes(-9),  Granted = true,  Note = "Valid PIN" },
            new AccessLog { AccessPointName = "Floor B Gate",  CustomerRef = "Unknown", EventTime = DateTime.Now.AddMinutes(-21), Granted = false, Note = "Unknown PIN attempt" },
            new AccessLog { AccessPointName = "Loading Bay",   CustomerRef = "#1887", EventTime = DateTime.Now.AddMinutes(-36),  Granted = true,  Note = "Valid RFID" },
            new AccessLog { AccessPointName = "Main Entrance", CustomerRef = "#0012", EventTime = DateTime.Now.AddMinutes(-78),  Granted = false, Note = "Expired credential" },
            new AccessLog { AccessPointName = "Floor A Gate",  CustomerRef = "#1103", EventTime = DateTime.Now.AddMinutes(-99),  Granted = true,  Note = "Valid PIN" },
            new AccessLog { AccessPointName = "Staff Office",  CustomerRef = "jsmith", EventTime = DateTime.Now.AddMinutes(-120), Granted = false, Note = "Wrong PIN x3" },
            new AccessLog { AccessPointName = "Main Entrance", CustomerRef = "#0998", EventTime = DateTime.Now.AddMinutes(-150), Granted = true,  Note = "Valid RFID" }
        );

        db.SaveChanges();
    }

    private static string BCryptHash(string password)
    {
        // Simple hash for demo — replace with BCrypt in production
        return Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password + "shurgard_salt")));
    }
}
