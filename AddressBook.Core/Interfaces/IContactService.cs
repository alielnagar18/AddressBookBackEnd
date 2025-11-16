using AddressBook.Core.DTOs;
using AddressBook.Core.Entities;

namespace AddressBook.Core.Interfaces;

public interface IContactService
{
    Task<ContactDto?> GetByIdAsync(int id);
    Task<Contact?> GetEntityByIdAsync(int id);
    Task<(IEnumerable<ContactDto> Items, int Total)> SearchAsync(string? q, DateTime? FromDate, DateTime? ToDate, int page, int pageSize);
    Task<ContactDto> CreateAsync(ContactCreateUpdateDto dto);
    Task<ContactDto?> UpdateAsync(int id, ContactCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task SetPhotoPathAsync(int id, string relativePath);
    Task<MemoryStream> ExportToExcelAsync(string? q, DateTime? FromDate, DateTime? ToDate);
}
