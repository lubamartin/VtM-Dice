using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using VtM_Dice.Dice;

namespace VtM_Dice.Modules
{
   [Name("Roll")]
   [Summary("Roll dices for you")]
   public class RollModule : ModuleBase<SocketCommandContext>
   {
      private const int DefaultDifficulty = 6;
      private const string DoNotCountCriticalSufix = "n";
      private const string WillpowerSufix = "w";

      [Command("r")]
      [Summary("[number Of Dices] - Roll dices with default difficulty and count critical roll")]
      public async Task Roll(int numberOfDices)
      {
         var message = RollDice(numberOfDices, DefaultDifficulty, true, false);
         await ReplyAsync("", false, message);
      }

      [Command("r")]
      [Summary("[number Of Dices] n or w - Roll dices with default difficulty and do not count critical | use willpower")]
      public async Task Roll(int numberOfDices, string param)
      {
         ParseParameter(param, out var countCritical, out var willpowerUsed);
         var message = RollDice(numberOfDices, DefaultDifficulty, countCritical, willpowerUsed);
         await ReplyAsync("", false, message);
      }

      [Command("r")]
      [Summary("[number Of Dices] [difficulty] - Roll dices with set difficulty and count critical roll")]
      public async Task Roll(int numberOfDices, int difficulty)
      {
         var message = RollDice(numberOfDices, difficulty, true, false);
         await ReplyAsync("", false, message);
      }

      [Command("r")]
      [Summary("[number Of Dices] [difficulty] n or w - Roll dices with set difficulty and do not count critical | use willpower")]
      public async Task Roll(int numberOfDices, int difficulty, string param)
      {
         ParseParameter(param, out var countCritical, out var willpowerUsed);
         var message = RollDice(numberOfDices, difficulty, countCritical, willpowerUsed);
         await ReplyAsync("", false, message);
      }

      [Command("r")]
      [Summary("[number Of Dices] n or w - Roll dices with set difficulty and do not count critical | use willpower")]
      public async Task Roll(int numberOfDices, string firstParam, string secondParam)
      {
         ParseParameter(firstParam, secondParam, out var countCritical, out var willpowerUsed);
         var message = RollDice(numberOfDices, DefaultDifficulty, countCritical, willpowerUsed);
         await ReplyAsync("", false, message);
      }


      [Command("r")]
      [Summary("[number Of Dices] [difficulty] n or w - Roll dices with set difficulty and do not count critical | use willpower")]
      public async Task Roll(int numberOfDices, int difficulty, string firstParam , string secondParam)
      {
         ParseParameter(firstParam, secondParam, out var countCritical, out var willpowerUsed);
         var message = RollDice(numberOfDices, difficulty, countCritical, willpowerUsed);
         await ReplyAsync("", false, message);
      }

      private Embed RollDice(int numberOfDices, int difficulty, bool countCritical, bool willpowerUsed)
      {
         var dice = Dice10.Instance;
         List<int> rolls = new List<int>();
         for (int i = 0; i < numberOfDices; i++)
         {
            rolls.Add(dice.Roll());
         }

         rolls.Sort();

         if (countCritical)
         {
            var reRolls = GetReRolls(rolls);
            rolls.AddRange(reRolls);
            rolls.Sort();
         }

         rolls = rolls.OrderBy(x => x == 0 ? int.MaxValue : x).ToList();

         return ComposeMessage(rolls, CountSuccesses(rolls, difficulty, willpowerUsed), willpowerUsed);
      }

      private void ParseParameter(string param, out bool countCritical, out bool willpowerUsed)
      {
         ParseParameter(param, param, out countCritical, out willpowerUsed);
      }

      private void ParseParameter(string paramFirst, string paramSecond, out bool countCritical, out bool willpowerUsed)
      {
         countCritical = !(paramFirst == DoNotCountCriticalSufix || paramSecond == DoNotCountCriticalSufix);
         willpowerUsed = paramFirst == WillpowerSufix || paramSecond == WillpowerSufix;
      }

      private Embed ComposeMessage(List<int> rolls, int numberOfSuccesses, bool willpowerUsed)
      {
         var builder = new EmbedBuilder()
         {
            Color = new Color(114, 137, 218)
         };
         var title = string.Empty;
         if (numberOfSuccesses < 0)
         {
            title = $"Roll is critical failure. Number of failures: {Math.Abs(numberOfSuccesses)}";
         }
         else
         {
            title = $"Number of successes : {numberOfSuccesses}";
         }

         builder.Title = title;
         builder.Description = $"Rolls [{string.Join(", ", rolls)}]";
         if (willpowerUsed)
         {
            builder.Description += " - willpower used";
         }

         return builder.Build();
      }

      private List<int> GetReRolls(List<int> rolls)
      {
         var dice = Dice10.Instance;
         List<int> reRolls = new List<int>();
         foreach (var roll in rolls)
         {
            if (roll == 0)
            {
               var number = dice.Roll();
               reRolls.Add(number);
               while (number == 0)
               {
                  number = dice.Roll();
                  reRolls.Add(number);
               }
            }
            else
            {
               break;
            }
         }
         return reRolls;
      }

      private int CountSuccesses(List<int> rolls, int difficulty, bool willpowerUsed)
      { 
         int numberOfSuccesses = willpowerUsed ? 1 : 0;

         foreach (var roll in rolls)
         {
            if (roll == 0 || roll >= difficulty)
            {
               numberOfSuccesses++;
            }
            else if(roll == 1)
            {
               numberOfSuccesses--;
            }
         }

         if (numberOfSuccesses < 0 && willpowerUsed)
         {
            numberOfSuccesses = 0;
         }

         return numberOfSuccesses;
      }
   }
}
