using Microsoft.AspNetCore.Mvc;
using AddressBook.Infrastructure.Data;
using AddressBook.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DepartmentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var list = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        return Ok(list);
    }
}
