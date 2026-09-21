using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Validation;

namespace UserManagementAPI.Models;

public class UserRequest
{
    [Required, NotWhiteSpace, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, NotWhiteSpace, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Department { get; set; }

    public User ToUser() => new()
    {
        FirstName = FirstName.Trim(),
        LastName = LastName.Trim(),
        Email = Email.Trim(),
        Department = string.IsNullOrWhiteSpace(Department) ? null : Department.Trim()
    };
}
