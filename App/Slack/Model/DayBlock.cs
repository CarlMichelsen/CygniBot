namespace App.Slack.Model;

public class DayBlock
{
    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }
    
    public required List<string> Attending { get; set; }
}