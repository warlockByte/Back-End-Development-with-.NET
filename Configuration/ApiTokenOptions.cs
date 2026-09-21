using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Configuration;

public sealed class ApiTokenOptions
{
    public const string SectionName = "Authentication";

    [Required, MinLength(16)]
    public string ApiToken { get; init; } = string.Empty;
}
