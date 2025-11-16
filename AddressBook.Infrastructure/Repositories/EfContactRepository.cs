using AddressBook.Core.Entities;
using AddressBook.Core.Interfaces;
using AddressBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Infrastructure.Repositories;

public class EfContactRepository : IContactRepository
{
    private readonly AppDbContext _db;
    public EfContactRepository(AppDbContext db) { _db = db; }

    public async Task AddAsync(Contact contact) { await _db.Contacts.AddAsync(contact); }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await _db.Contacts.Include(c => c.Job).Include(c => c.Department).ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _db.Contacts.Include(c => c.Job).Include(c => c.Department).FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Remove(Contact contact) { _db.Contacts.Remove(contact); }

    public void Update(Contact contact) { _db.Contacts.Update(contact); }

    public async Task SaveChangesAsync() { await _db.SaveChangesAsync(); }

    public async Task<(IEnumerable<Contact> Items, int Total)> SearchAsync(string? q, DateTime? FromDate, DateTime? ToDate, int page, int pageSize)
    {
        var query = _db.Contacts.Include(c => c.Job).Include(c => c.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(c =>
                c.FullName.Contains(q) ||
                c.Email.Contains(q) ||
                c.Mobile.Contains(q) ||
                c.Address.Contains(q) ||
                (c.Job != null && c.Job.Name.Contains(q)) ||
                (c.Department != null && c.Department.Name.Contains(q))
            );
        }

        if (FromDate.HasValue) query = query.Where(c => c.BirthDate >= FromDate.Value);
        if (ToDate.HasValue) query = query.Where(c => c.BirthDate <= ToDate.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
