using System.Text.Json.Serialization;
using App.Slack.Model.V1;

namespace App.Slack.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(WeeklyAttendanceMessageV1), typeDiscriminator: 1)]
public abstract class BaseWeeklyAttendanceMessage
{
    public const string MetadataKey = "CygniBotWeeklyAttendanceMessage";
    
    [JsonIgnore]
    public abstract string Version { get; }
    
    protected abstract BaseWeeklyAttendanceMessage? ToNextVersion();

    public WeeklyAttendanceMessageV1 ToLatestVersion()
    {
        var final = this;
        var tempConvert = this.ToNextVersion();
        while (tempConvert is not null)
        {
            final = tempConvert!;
            tempConvert = tempConvert.ToLatestVersion();
        }
        
        return final as WeeklyAttendanceMessageV1 ?? throw new NullReferenceException();
    }
}