# Access Control System

ASP.NET Core 8 MVC web application with role-based access control,
cookie authentication, SQLite database via Entity Framework Core.




## Respective Demo accounts

| Role               | Username  | Password   |
|--------------------|-----------|------------|
| System Admin       | admin     | Admin@123  |
| Staff              | jsmith    | Staff@123  |
| Viewer             | viewer1   | View@123   |
| Security Officer   | guard1    | Guard@123  |


## Role permissions

| Feature              | Admin | Staff | Security | Viewer |
|----------------------|-------|-------|----------|--------|
| View dashboard & log | ✓     | ✓     | ✓        | ✓      |
| Simulate access      | ✓     | ✓     | ✓        | ✓      |
| Reset customer PIN   | ✓     | ✓     | ✓        | –      |
| Lock / unlock gate   | ✓     | ✓     | ✓        | –      |
| Export audit CSV     | ✓     | ✓     | ✓        | –      |
| Manage users         | ✓     | –     | –        | –      |
| Edit permissions     | ✓     | –     | –        | –      |

---

# Notes

- Authentication uses ASP.NET Core cookie middleware
- Passwords are SHA-256 hashed (swap for BCrypt in production)
- Database is SQLite — zero config, file-based (`shurgard.db`)
- All access events (including simulations and logins) are written to the audit log
