using System.Threading.Channels;

namespace App.Extensions;

public static class ChannelExtensions
{
    public static IServiceCollection AddSingleReaderChannel<TMessage>(this IServiceCollection services)
        where TMessage : class
    {
        return services
            .AddSingleton(
                _ => Channel.CreateUnbounded<TMessage>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    AllowSynchronousContinuations = false,
                }));
    }
}