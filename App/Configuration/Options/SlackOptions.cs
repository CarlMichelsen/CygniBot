using System.ComponentModel.DataAnnotations;

namespace App.Configuration.Options;

public class SlackOptions : IConfigurationOptions
{
    public static string SectionName { get; } = "Slack";
    
    [Required]
    [MinLength(1)]
    public required string BotToken { get; init; }
    
    /// <summary>
    /// This is a secret that is used to verify that slack requests are actually coming from slack.
    /// This value can be found on the 'Basic information' page in the *Your-Application* page.
    /// </summary>
    [Required]
    [MinLength(1)]
    public required string SigningSecret { get; init; }
    
    /// <summary>
    /// Channel identifier for the channel that will receive the office-attendance messages. 
    /// </summary>
    [Required]
    [MinLength(1)]
    public required string Channel { get; init; }
}