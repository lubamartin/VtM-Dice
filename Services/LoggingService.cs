using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace VtMDiceMVC.Services
{
   public class LoggingService
   {
      private string LogDirectory { get; }
      private string LogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");

      public LoggingService(DiscordSocketClient discord, CommandService commands){
         LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

         discord.Log += OnLogAsync;
         commands.Log += OnLogAsync;

      }

      private Task OnLogAsync(LogMessage message)
      {
         if (!Directory.Exists(LogDirectory))
         {
            Directory.CreateDirectory(LogDirectory);
         }

         if (!File.Exists(LogFile))
         {
            File.Create(LogFile);
         }

         string logText = $"{DateTime.UtcNow:hh:mm:ss} [{message.Severity}] {message.Source}: {message.Exception?.ToString() ?? message.Message}";
         //using (StreamWriter sw = File.AppendText(LogFile))
         //{
         //   sw.WriteLine(logText);
         //}

         return Console.Out.WriteLineAsync(logText);
      }
   }   
}
