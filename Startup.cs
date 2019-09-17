using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VtMDiceMVC.Services;

namespace VtMDiceMVC
{
    public class Startup
    {
        public static async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigurationServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<CommandHandler>();

            await provider.GetRequiredService<StartupService>().StartAsync();

            await Task.Delay(-1);
        }

        private static void ConfigurationServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<StartupService>()
                .AddSingleton<LoggingService>();
        }
    }
}