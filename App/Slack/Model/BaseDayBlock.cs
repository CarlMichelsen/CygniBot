using System.Text.Json.Serialization;
using App.Slack.Model.V1;

namespace App.Slack.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DayBlockV1), typeDiscriminator: 1)]
public abstract class BaseDayBlock
{
    [JsonIgnore]
    public abstract string Version { get; }

    protected abstract BaseDayBlock? ToNextVersion();
    
    public DayBlockV1 ToLatestVersion()
    {
        var final = this;
        var tempConvert = this.ToNextVersion();
        while (tempConvert is not null)
        {
            final = tempConvert!;
            tempConvert = tempConvert.ToLatestVersion();
        }
        
        return final as DayBlockV1 ?? throw new NullReferenceException();
    }
}