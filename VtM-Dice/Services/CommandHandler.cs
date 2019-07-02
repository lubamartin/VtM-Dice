using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace VtM_Dice.Services
{
   public class CommandHandler
   {
      public const string Prefix = "!";
      private const string Roll = "roll";

      private readonly DiscordSocketClient _discord;
      private readonly CommandService _command;
      private readonly IServiceProvider _provider;

      public CommandHandler(DiscordSocketClient discord, CommandService command, IServiceProvider provider)
      {
         _discord = discord;
         _command = command;
         _provider = provider;

         _discord.MessageReceived += OnMessageReceivedAsync;
      }

      private async Task OnMessageReceivedAsync(SocketMessage arg)
      {
         if (arg is SocketUserMessage message && message.Author.Id != _discord.CurrentUser.Id)
         {
            var context = new SocketCommandContext(_discord, message);

            int argumentPosition = 0;
            if(message.HasStringPrefix(Prefix, ref argumentPosition) || message.HasMentionPrefix(_discord.CurrentUser, ref argumentPosition))
            {
               var result = await _command.ExecuteAsync(context, argumentPosition, _provider);

               if (!result.IsSuccess)
               {
                  await context.Channel.SendFileAsync(result.ToString()); //TODO test and add error handling
               }
            }
         }
      }
   }
}
