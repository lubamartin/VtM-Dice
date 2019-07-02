using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VtM_Dice.Services;

namespace VtM_Dice
{
   public class Startup
   {
      public static async Task RunAsync(string[] args)
      {
         var startup = new Startup();
         await startup.RunAsync();
      }

      public async Task RunAsync()
      {
         var services = new ServiceCollection();
         ConfigurationServices(services);

         var provider = services.BuildServiceProvider();
         provider.GetRequiredService<LoggingService>();
         provider.GetRequiredService<CommandHandler>();

         await provider.GetRequiredService<StartupService>().StartAsync();

         await Task.Delay(-1);
      }

      private void ConfigurationServices(IServiceCollection services)
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
