using System.Reflection;

namespace App;

public static class ApplicationConstants
{
    public const string Name = "CygniBot";

    public static string Version { get; } = Assembly
        .GetEntryAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? throw new ArgumentNullException(nameof(Version));
}