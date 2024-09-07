namespace School.Project.Systems.Services.Identity.Models.DTOs;

public record SessionDTO(Guid UserId, string Username, string AccessToken);