using Microsoft.AspNetCore.Mvc;
using AddressBook.Core.Interfaces;
using AddressBook.Core.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AddressBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _service;
    private readonly IWebHostEnvironment _env;

    public ContactsController(IContactService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? q, [FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (items, total) = await _service.SearchAsync(q, FromDate, ToDate, page, pageSize);
        return Ok(new { items, total });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var Result = await _service.GetByIdAsync(id);
        if (Result == null) return NotFound();
        return Ok(Result);
    }

    [HttpPost]
    [Authorize]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Create([FromForm] ContactCreateUpdateDto dto, [FromForm] IFormFile? photo)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
            return BadRequest(new { errors });
        }

        var Created = await _service.CreateAsync(dto);

        if (photo != null && photo.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(imagesPath)) 
                Directory.CreateDirectory(imagesPath);

            var path = Path.Combine(imagesPath, fileName);
            using (var fs = System.IO.File.Create(path))
            {
                await photo.CopyToAsync(fs);
            }
            await _service.SetPhotoPathAsync(Created.Id, $"/images/{fileName}");
            // reload created dto with photo path
            Created = await _service.GetByIdAsync(Created.Id) ?? Created;
        }

        return CreatedAtAction(nameof(Get), new { id = Created.Id }, Created);
    }

    [HttpPut("{id}")]
    [Authorize]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Update(int id, [FromForm] ContactCreateUpdateDto dto, [FromForm] IFormFile? photo)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
            return BadRequest(new { errors });
        }
        var existingEntity = await _service.GetEntityByIdAsync(id); // we need an internal method returning Contact entity
        if (existingEntity == null) 
            return NotFound();
        var Updated = await _service.UpdateAsync(id, dto);
        if (Updated == null) 
            return NotFound();

        if (photo != null && photo.Length > 0)
        {
            // Delete old file if exists
            if (!string.IsNullOrWhiteSpace(existingEntity.PhotoPath))
            {
                var oldPath = existingEntity.PhotoPath.TrimStart('/');
                var fullOldPath = Path.Combine(_env.WebRootPath ?? "wwwroot", oldPath.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullOldPath))
                {
                    try { System.IO.File.Delete(fullOldPath); } catch { /* swallow or log */ }
                }
            }
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
            var path = Path.Combine(imagesPath, fileName);
            using (var fs = System.IO.File.Create(path))
            {
                await photo.CopyToAsync(fs);
            }
            await _service.SetPhotoPathAsync(id, $"/images/{fileName}");
            Updated = await _service.GetByIdAsync(id) ?? Updated;
        }

        return Ok(Updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpGet("export")]
    [Authorize]
    public async Task<IActionResult> Export([FromQuery] string? q, [FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
    {
        var ms = await _service.ExportToExcelAsync(q, FromDate, ToDate);
        ms.Position = 0;
        return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Contacts.xlsx");
    }
}
