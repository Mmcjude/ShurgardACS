using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShurgardACS.Data;
using ShurgardACS.Models;
using System.Text;

namespace ShurgardACS.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var today = DateTime.Today;
        var vm = new DashboardViewModel
        {
            TotalUsers   = _db.Users.Count(u => u.IsActive),
            OnlinePoints = _db.AccessPoints.Count(a => a.IsOnline),
            TodayEntries = _db.AccessLogs.Count(l => l.EventTime >= today && l.Granted),
            DeniedToday  = _db.AccessLogs.Count(l => l.EventTime >= today && !l.Granted),
            RecentLogs   = _db.AccessLogs.OrderByDescending(l => l.EventTime).Take(8).ToList()
        };
        return View(vm);
    }

    public IActionResult AccessPoints() =>
        View(_db.AccessPoints.OrderBy(a => a.Name).ToList());

    public IActionResult Logs(string? filter)
    {
        var query = _db.AccessLogs.AsQueryable();
        if (filter == "granted") query = query.Where(l => l.Granted);
        if (filter == "denied")  query = query.Where(l => !l.Granted);
        ViewBag.Filter = filter ?? "all";
        return View(query.OrderByDescending(l => l.EventTime).Take(100).ToList());
    }

    [Authorize(Policy = "StaffAndUp")]
    public IActionResult ExportLogs()
    {
        var logs = _db.AccessLogs.OrderByDescending(l => l.EventTime).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("Time,AccessPoint,CustomerRef,Granted,Note");
        foreach (var l in logs)
            sb.AppendLine($"{l.EventTime:yyyy-MM-dd HH:mm:ss},{l.AccessPointName},{l.CustomerRef},{l.Granted},{l.Note}");
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "AccessLog_Export.csv");
    }

    [HttpGet]
    public IActionResult Simulate()
    {
        var vm = new SimulateViewModel
        {
            Users        = _db.Users.Where(u => u.IsActive).ToList(),
            AccessPoints = _db.AccessPoints.ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Simulate(SimulateViewModel model)
    {
        model.Users        = _db.Users.Where(u => u.IsActive).ToList();
        model.AccessPoints = _db.AccessPoints.ToList();

        if (model.SelectedUser == null || model.SelectedPoint == null)
            return View(model);

        var point = _db.AccessPoints.FirstOrDefault(a => a.Id.ToString() == model.SelectedPoint);
        var user  = _db.Users.FirstOrDefault(u => u.Id.ToString() == model.SelectedUser);

        if (point == null || user == null) return View(model);

        if (!point.IsOnline)
        {
            model.Result        = false;
            model.ResultMessage = $"Access denied — {point.Name} is currently offline.";
        }
        else if (point.Name == "Server Room" && user.Role != "SystemAdmin")
        {
            model.Result        = false;
            model.ResultMessage = $"Access denied — Server Room is restricted to System Administrators only.";
        }
        else if (point.Name == "Staff Office" && user.Role == "Viewer")
        {
            model.Result        = false;
            model.ResultMessage = $"Access denied — Staff Office requires Staff role or above.";
        }
        else
        {
            model.Result        = true;
            model.ResultMessage = $"Access granted — {user.FullName} can access {point.Name}.";
        }

        // Log the simulation
        _db.AccessLogs.Add(new AccessLog
        {
            AccessPointName = point.Name,
            CustomerRef     = user.Username + " (sim)",
            Granted         = model.Result.Value,
            Note            = "Simulated: " + model.ResultMessage
        });
        _db.SaveChanges();

        return View(model);
    }

    public IActionResult Error() => View();
}
