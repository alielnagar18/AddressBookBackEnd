using System.ComponentModel.DataAnnotations;

namespace AddressBook.Core.DTOs;

public class ContactCreateUpdateDto
{
    [Required]
    public string FullName { get; set; } = null!;
    [Required] 
    public int JobId { get; set; }
    [Required] 
    public int DepartmentId { get; set; }
    [Required] 
    [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "Invalid phone number")]
    public string Mobile { get; set; } = null!;
    [Required] 
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    [Required] 
    [EmailAddress] 
    public string Email { get; set; } = null!;
}
