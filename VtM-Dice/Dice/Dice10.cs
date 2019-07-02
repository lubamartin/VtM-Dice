using System;
using System.Collections.Generic;
using System.Text;

namespace VtM_Dice.Dice
{
   public class Dice10
   {
      private static Dice10 _instance;
      private readonly Random _random;
      private const int Zero = 0;
      private const int MaxTenValue = 10;

      public static Dice10 Instance
      {
         get
         {
            if (_instance == null)
               _instance = new Dice10();
            return _instance;
         }
      }

      Dice10()
      {
         _random = new Random();
      }

      public int Roll()
      {
         return _random.Next(Zero, MaxTenValue);
      }





   }
}
