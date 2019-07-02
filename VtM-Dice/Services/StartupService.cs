using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace VtM_Dice.Services
{
   public class StartupService
   {
      private const string Config = "Config.cfg";

      private readonly IServiceProvider _provider;
      private readonly DiscordSocketClient _discord;
      private readonly CommandService _commands;

      public StartupService( IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
      {
         _provider = provider;
         _discord = discord;
         _commands = commands;
      }

      public async Task StartAsync()
      {
         string discordToken = GetToken();
         if (string.IsNullOrWhiteSpace(discordToken))
         {
            throw new Exception("Please enter your bot's token.");
         }

         await _discord.LoginAsync(TokenType.Bot, discordToken);
         await _discord.StartAsync();

         await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
      }

      private string GetToken()
      {
         string result;
         try
         {
            var configFile = Path.Combine(AppContext.BaseDirectory, Config);
            using (var reader = new StreamReader(configFile))
            {
               result = reader.ReadLine();
            }
         }
         catch (IOException e)
         {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e);
            throw;
         }

         return result;
      }
   }
}
