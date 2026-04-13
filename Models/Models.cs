using System.ComponentModel.DataAnnotations;

namespace ShurgardACS.Models;

public class AppUser
{
    public int    Id           { get; set; }
    [Required] public string FullName     { get; set; } = "";
    [Required] public string Username     { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required] public string Role         { get; set; } = "Viewer";
    public bool   IsActive     { get; set; } = true;
    public DateTime CreatedAt  { get; set; } = DateTime.Now;
}

public class AccessPoint
{
    public int    Id       { get; set; }
    [Required] public string Name     { get; set; } = "";
    public string Location  { get; set; } = "";
    public string Method    { get; set; } = "";
    public bool   IsOnline  { get; set; } = true;
}

public class AccessLog
{
    public int      Id              { get; set; }
    public string   AccessPointName { get; set; } = "";
    public string   CustomerRef     { get; set; } = "";
    public DateTime EventTime       { get; set; } = DateTime.Now;
    public bool     Granted         { get; set; }
    public string   Note            { get; set; } = "";
}

// View models
public class LoginViewModel
{
    [Required] public string Username { get; set; } = "";
    [Required] public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}

public class AddUserViewModel
{
    [Required] public string FullName { get; set; } = "";
    [Required] public string Username { get; set; } = "";
    [Required, MinLength(6)] public string Password { get; set; } = "";
    [Required] public string Role     { get; set; } = "Viewer";
}

public class SimulateViewModel
{
    public string? SelectedUser  { get; set; }
    public string? SelectedPoint { get; set; }
    public bool?   Result        { get; set; }
    public string? ResultMessage { get; set; }
    public List<AppUser>      Users        { get; set; } = new();
    public List<AccessPoint>  AccessPoints { get; set; } = new();
}

public class DashboardViewModel
{
    public int TotalUsers      { get; set; }
    public int OnlinePoints    { get; set; }
    public int TodayEntries    { get; set; }
    public int DeniedToday     { get; set; }
    public List<AccessLog> RecentLogs { get; set; } = new();
}
