namespace AddressBook.Core.DTOs;

public class ContactDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public int JobId { get; set; }
    public string? JobName { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string Mobile { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = null!;
    public string? PhotoPath { get; set; }
    public int Age { get; set; }
}
