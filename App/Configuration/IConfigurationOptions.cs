namespace App.Configuration;

public interface IConfigurationOptions
{
    static abstract string SectionName { get; }
}