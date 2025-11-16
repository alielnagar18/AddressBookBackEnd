using AddressBook.Core.DTOs;
using AddressBook.Core.Entities;
using AddressBook.Core.Interfaces;
using AutoMapper;
using ClosedXML.Excel;

namespace AddressBook.Services.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _repo;
    private readonly IMapper _mapper;

    public ContactService(IContactRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ContactDto> CreateAsync(ContactCreateUpdateDto dto)
    {
        var entity = new Contact
        {
            FullName = dto.FullName,
            JobId = dto.JobId,
            DepartmentId = dto.DepartmentId,
            Mobile = dto.Mobile,
            BirthDate = dto.BirthDate,
            Address = dto.Address,
            Email = dto.Email
        };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();

        // reload to get navigation fields
        var created = await _repo.GetByIdAsync(entity.Id);
        return MapToDto(created!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        _repo.Remove(existing);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<ContactDto?> GetByIdAsync(int id)
    {
        var Result = await _repo.GetByIdAsync(id);
        if (Result == null)
            return null;
        return MapToDto(Result);
    }
    public async Task<Contact?> GetEntityByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<(IEnumerable<ContactDto> Items, int Total)> SearchAsync(string? q, DateTime? FromDate, DateTime? ToDate, int page, int pageSize)
    {
        var (items, total) = await _repo.SearchAsync(q, FromDate, ToDate, page, pageSize);
        var dtos = items.Select(MapToDto);
        return (dtos, total);
    }

    public async Task<ContactDto?> UpdateAsync(int id, ContactCreateUpdateDto dto)
    {
        var Existing = await _repo.GetByIdAsync(id);
        if (Existing == null) 
            return null;

        Existing.FullName = dto.FullName;
        Existing.JobId = dto.JobId;
        Existing.DepartmentId = dto.DepartmentId;
        Existing.Mobile = dto.Mobile;
        Existing.BirthDate = dto.BirthDate;
        Existing.Address = dto.Address;
        Existing.Email = dto.Email;

        _repo.Update(Existing);
        await _repo.SaveChangesAsync();

        var Updated = await _repo.GetByIdAsync(id);
        return MapToDto(Updated!);
    }

    public async Task SetPhotoPathAsync(int id, string relativePath)
    {
        var Existing = await _repo.GetByIdAsync(id);
        if (Existing == null) 
            return;
        Existing.PhotoPath = relativePath;
        _repo.Update(Existing);
        await _repo.SaveChangesAsync();
    }

    public async Task<MemoryStream> ExportToExcelAsync(string? q, DateTime? FromDate, DateTime? ToDate)
    {
        var (items, _) = await _repo.SearchAsync(q, FromDate, ToDate, 1, int.MaxValue);
        var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Contacts");
        ws.Cell(1, 1).Value = "Id";
        ws.Cell(1, 2).Value = "FullName";
        ws.Cell(1, 3).Value = "Email";
        ws.Cell(1, 4).Value = "Mobile";
        ws.Cell(1, 5).Value = "Job";
        ws.Cell(1, 6).Value = "Department";
        ws.Cell(1, 7).Value = "BirthDate";
        ws.Cell(1, 8).Value = "Address";
        ws.Cell(1, 9).Value = "PhotoPath";
        var row = 2;
        foreach (var c in items)
        {
            ws.Cell(row, 1).Value = c.Id;
            ws.Cell(row, 2).Value = c.FullName;
            ws.Cell(row, 3).Value = c.Email;
            ws.Cell(row, 4).Value = c.Mobile;
            ws.Cell(row, 5).Value = c.Job?.Name;
            ws.Cell(row, 6).Value = c.Department?.Name;
            ws.Cell(row, 7).Value = c.BirthDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 8).Value = c.Address;
            ws.Cell(row, 9).Value = c.PhotoPath;
            row++;
        }

        var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private ContactDto MapToDto(Contact e)
    {
        return new ContactDto
        {
            Id = e.Id,
            FullName = e.FullName,
            JobId = e.JobId,
            JobName = e.Job?.Name,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            Mobile = e.Mobile,
            BirthDate = e.BirthDate,
            Address = e.Address,
            Email = e.Email,
            PhotoPath = e.PhotoPath,
            Age = e.Age
        };
    }
}
