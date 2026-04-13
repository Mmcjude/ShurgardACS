using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShurgardACS.Data;
using ShurgardACS.Models;

namespace ShurgardACS.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    public IActionResult Index() =>
        View(_db.Users.OrderBy(u => u.FullName).ToList());

    [HttpGet]
    public IActionResult Add() => View(new AddUserViewModel());

    [HttpPost]
    public IActionResult Add(AddUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (_db.Users.Any(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View(model);
        }

        _db.Users.Add(new AppUser
        {
            FullName     = model.FullName,
            Username     = model.Username,
            PasswordHash = HashPassword(model.Password),
            Role         = model.Role,
            IsActive     = true
        });
        _db.SaveChanges();
        TempData["Success"] = $"User '{model.FullName}' added successfully.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Deactivate(int id)
    {
        var user = _db.Users.Find(id);
        if (user != null && user.Role != "SystemAdmin")
        {
            user.IsActive = false;
            _db.SaveChanges();
            TempData["Success"] = $"User '{user.FullName}' deactivated.";
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Activate(int id)
    {
        var user = _db.Users.Find(id);
        if (user != null)
        {
            user.IsActive = true;
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    private static string HashPassword(string password) =>
        Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password + "shurgard_salt")));
}
