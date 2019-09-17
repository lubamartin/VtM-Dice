using System;

namespace VtMDiceMVC.Dice
{
   public class Dice6
   {
      private static Dice6 _instance;
      private readonly Random _random;
      private const int Zero = 1;
      private const int MaxTenValue = 7;

      public static Dice6 Instance => _instance ?? (_instance = new Dice6());

      private Dice6() => _random = new Random();

      public int Roll()
      {
         return _random.Next(Zero, MaxTenValue);
      }
   }
}
