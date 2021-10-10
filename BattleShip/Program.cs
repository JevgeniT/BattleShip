using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BattleShip
{
    class Program
    {
        private static int _index = 0;

        static void Main(string[] args)
        {


            Menu.Run();
        }
        
        private static string DrawListMenu(List<string> items)
        {
            Console.Clear();
            for (var i = 0; i < items.Count; i++)
            {
                var pointer =  i == _index ? "**":"";
                Console.WriteLine("|{0,9}{1,11}{2,-9}|", pointer, items[i], pointer);
            } 

            
            var ckey = Console.ReadKey();
            switch (ckey.Key.ToString())
            {
                case "S" when _index < items.Count-1:
                    _index++;
                    break;
                case "W" when _index > 0:
                    _index--;
                    break;
                default:
                {
                    if (ckey.Key == ConsoleKey.Enter)
                    {
                        return items[_index];
                    }
                    break;
                }
            }
            return "";
        }
    }


    public enum Currency : int
    {
        EUR = 1,
        USD = 2
    }
}