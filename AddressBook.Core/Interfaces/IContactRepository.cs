using AddressBook.Core.Entities;

namespace AddressBook.Core.Interfaces;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(int id);
    Task<IEnumerable<Contact>> GetAllAsync();
    Task AddAsync(Contact contact);
    void Update(Contact contact);
    void Remove(Contact contact);
    Task SaveChangesAsync();

    Task<(IEnumerable<Contact> Items, int Total)> SearchAsync(string? q, DateTime? FromDate, DateTime? ToDate, int page, int pageSize);
}
