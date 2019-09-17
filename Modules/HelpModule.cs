using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using VtMDiceMVC.Services;

namespace VtMDiceMVC.Modules
{
   [Name("Help")]
   public class HelpModule : ModuleBase<SocketCommandContext>
   {
      private readonly CommandService _service;

      public HelpModule(CommandService service)
      {
         _service = service;
      }

      [Command("help")]
      [Summary("List of all commands")]
      public async Task HelpAsync()
      {
         var builder = new EmbedBuilder()
         {
            Color = new Color(114, 137, 218),
            Description = "These are the commands you can use"
         };

         foreach (var module in _service.Modules)
         {
            string description = null;
            foreach (var cmd in module.Commands)
            {
               var result = await cmd.CheckPreconditionsAsync(Context);
               if (result.IsSuccess)
                  description += $"{CommandHandler.Prefix}{cmd.Aliases.First()} {cmd.Summary} \n";
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
               builder.AddField(x =>
               {
                  x.Name = module.Name;
                  x.Value = description;
                  x.IsInline = false;
               });
            }
         }

         await ReplyAsync("", false, builder.Build());
      }
   }
}
