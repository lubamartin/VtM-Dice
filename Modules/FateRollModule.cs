using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using VtMDiceMVC.Dice;

namespace VtMDiceMVC.Modules
{
   [Name("Fate-Roll")]
   [Summary("Roll dices for you (Fate system)")]
   public class FateRollModule : ModuleBase<SocketCommandContext>
   {
      private const int DefaultModifier = 0;
      private const int DefaultDamage = 0;
      private const int DefaultNumberOfDice = 4;
      private const int SuccessFrom = 5;
      private const int FailureFrom = 2;


      [Command("f")]
      [Summary(" - Roll dices with default modifier")]
      public async Task Roll()
      {
         var message = RollDice(DefaultModifier, DefaultDamage);
         await ReplyAsync("", false, message);
      }

      [Command("f")]
      [Summary("[modifier] - Roll dices with modifier")]
      public async Task Roll(int modifier)
      {
         var message = RollDice(modifier, DefaultDamage);
         await ReplyAsync("", false, message);
      }

      [Command("f")]
      [Summary("d[damage] - Roll dices with default modifier and damage to roll")]
      public async Task Roll(string damage)
      {
         var message = RollDice(DefaultModifier, ParseDamage(damage));
         await ReplyAsync("", false, message);
      }

      [Command("f")]
      [Summary("[modifier] d[damage] - Roll dices with modifier and damage to roll")]
      public async Task Roll(int modifier, string damage)
      {
         var message = RollDice(modifier, ParseDamage(damage));
         await ReplyAsync("", false, message);
      }

      private Embed RollDice(int modifier, int damage)
      {
         int numberOfDices = DefaultNumberOfDice - damage;
         var dice = Dice6.Instance;
         List<int> rolls = new List<int>();
         for (int i = 0; i < numberOfDices; i++)
         {
            rolls.Add(dice.Roll());
         }

         rolls.Sort();

         return ComposeMessage(ConvertRollNumberToCharacters(rolls), CountSuccesses(rolls, modifier), modifier, damage);
      }

      private int CountSuccesses(IEnumerable<int> rolls, int modifier)
      {
         int numberOfSuccesses = modifier;

         foreach (var roll in rolls)
         {
            if (roll >= SuccessFrom)
            {
               numberOfSuccesses++;
            }
            else if (roll <= FailureFrom)
            {
               numberOfSuccesses--;
            }
         }

         return numberOfSuccesses;
      }

      private Embed ComposeMessage(IEnumerable<string> rolls, int numberOfSuccesses, int modifier, int damage)
      {
         var builder = new EmbedBuilder()
         {
            Color = new Color(114, 137, 218)
         };

         var title = numberOfSuccesses < 0 
            ? $"Roll is failure. Number of failures: {Math.Abs(numberOfSuccesses)}" 
            : $"Number of successes : {numberOfSuccesses}";

         builder.Title = title;
         builder.Description = $"Rolls [{string.Join(", ", rolls)}]";
         if (modifier != DefaultModifier)
         {
            builder.Description += $" + modifier: {modifier} |";
         }

         if (damage > DefaultDamage)
         {
            builder.Description += $" damage: {damage}";
         }

         return builder.Build();
      }

      private List<string> ConvertRollNumberToCharacters(IEnumerable<int> rolls)
      {
          int highestNegativeValue = 2;
          int lowestPositiveValue = 5;
          List<string> result = new  List<string>();

          foreach (int roll in rolls)
          {
              if (roll <= highestNegativeValue)
              {
                  result.Add(" - ");
              }
              else if (roll >= lowestPositiveValue)
              {
                  result.Add(" + ");
              }
              else
              {
                  result.Add(" 0 ");
              }
          }

          return result;
      }

      private int ParseDamage(string damage)
      {
         int parsedDamage = DefaultDamage;
         string prefix = damage.Substring(0, 1);
         string damageModifier = damage.Substring(1);
         if (prefix == "d")
         {
            int.TryParse(damageModifier, out parsedDamage);
         }

         return parsedDamage;
      }
   }
}
