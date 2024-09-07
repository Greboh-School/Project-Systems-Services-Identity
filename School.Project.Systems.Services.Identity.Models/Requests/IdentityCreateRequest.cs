using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace School.Project.Systems.Services.Identity.Models.Requests;

public sealed record IdentityCreateRequest
{
    public required string UserName { get; set; } = default!;
    
    public required string Password { get; set; } = default!;
    
    [DefaultValue("SYSTEM"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CreatedBy { get; set; } = "SYSTEM";
}