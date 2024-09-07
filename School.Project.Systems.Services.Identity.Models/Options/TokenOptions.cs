namespace School.Project.Systems.Services.Identity.Models.Options;

public class TokenOptions
{
    public static string Section => "Token";
    public float LifeTimeInMinutes { get; set; }
}