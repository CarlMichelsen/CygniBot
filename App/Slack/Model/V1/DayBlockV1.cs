namespace App.Slack.Model.V1;

public class DayBlockV1 : BaseDayBlock
{
    public override string Version { get; } = "1";
    
    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }
    
    public required List<string> Attending { get; set; }
    
    protected override BaseDayBlock? ToNextVersion() => null;
}