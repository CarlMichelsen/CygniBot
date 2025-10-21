using System.ComponentModel.DataAnnotations;

namespace App.Configuration.Options;

public class SlackOptions : IConfigurationOptions
{
    public static string SectionName { get; } = "Slack";
    
    [Required]
    [MinLength(1)]
    public required string BotToken { get; init; }
}