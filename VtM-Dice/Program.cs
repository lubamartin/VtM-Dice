using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace VtM_Dice
{
   class Program
   {
      public static Task Main(string[] args)
         => Startup.RunAsync(args);


      private DiscordSocketClient _client;

      public async Task MainAsync()
      {
         var config = new DiscordSocketConfig { MessageCacheSize = 100 };

         _client = new DiscordSocketClient(config);
         _client.Log += Log;

         // Remember to keep token private or to read it from an 
         // external source! In this case, we are reading the token 
         // from an environment variable. If you do not know how to set-up
         // environment variables, you may find more information on the 
         // Internet or by using other methods such as reading from 
         // a configuration.
         string token = getToken();

         await _client.LoginAsync(TokenType.Bot, token);
         await _client.StartAsync();


         _client.MessageUpdated += MessageUpdated;

         //CommandHandler commandHandler = new CommandHandler(_client, new CommandService());
         // Block this task until the program is closed.
         await Task.Delay(-1);
      }

      private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
      {
         // If the message was not in the cache, downloading it will result in getting a copy of `after`.
         var message = await before.GetOrDownloadAsync();
         Console.WriteLine($"{message} -> {after}");
      }


      private Task Log(LogMessage msg)
      {
         Console.WriteLine(msg.ToString());
         return Task.CompletedTask;
      }
      private string getToken()
      {
         string result;

         try
         {
            using (StreamReader sr = new StreamReader("Config.cfg"))
            {
               result = sr.ReadLine();
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
