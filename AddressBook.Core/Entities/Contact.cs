using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBook.Core.Entities;

public class Contact
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public int JobId { get; set; }
    public Job? Job { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string Mobile { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = null!;
    public string? PhotoPath { get; set; } 

    [NotMapped]
    public int Age => CalculateAge();

    private int CalculateAge()
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
