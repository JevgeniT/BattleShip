using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip
{
    public class GameEngine
    {
        private static Player? PlayerB;
        private static bool swap;
        private static Player? PlayerA;

        private static Player? Player;
        // private static char[,]? Field { get; set; } = Player?.Field;

        private enum Pos{ R, C } // ROW / COL


        public GameEngine(Settings settings)
        {
            // settings.Field;
            PlayerA = new()
            {
                
                HisTurn = true,
                Field = settings.Field,
                Char = '*'
            };
            PlayerB = new()
            {
                Field = new char[10, 10],
                Char = '+'
            };
            
            
            Player = PlayerA;
        }
        
        private static int? RowLength => Player?.Field?.GetUpperBound(0) + 1;
        private static int? ColLength => Player?.Field?.Length / RowLength;

        private static void SwapPlayer()
        {
            Player = Player!.Char == '*' ? PlayerB : PlayerA;
            swap = true;
        }
        private static void DrawField(ref bool setup)
        {
            Console.Clear();
            if (Player?.Field![Player!.C, Player.R] != Player!.Char)
            {
                if (setup)
                {
                    var key = Player!.Boats.FirstOrDefault(pair => !pair.Value).Key;
                    if (key == null  || key == 0)
                    {
                        SwapPlayer();
                    }
                    for (var i = 0; i < key; i++)
                    {
                        Player.Field![Player.C + i, Player.R] = Player!.Char;
                    }
                }
                Player!.Field![Player.C, Player.R] = Player!.Char;
            }

            for (var c = 0; c < ColLength; c++)
            {
                var sb = new StringBuilder($"| ");
                for (var r = 0; r < RowLength; r++)
                {
                    var ch = Player?.Field![c, r] != Player!.Char
                        ? $" {Player!.Field![c, r]} "
                        : $" {Player!.Field![c, r]}";
                    sb.Append(ch);
                }
                sb.Append(" |\n");
                Console.Write(sb);
            }

            Console.WriteLine(string.Join(", ", Player.Boats));
            Console.WriteLine($"{swap}");

        }

        private static void MakeMove()
        {
            bool set = false;
            Player!.Field![Player!.C, Player!.R] = 'o';
            DrawField(ref set);
            SwapPlayer();
        }

        private static void ClearIndex(ref bool enterPressed, ref bool setup)
        {
            if (!enterPressed)
            {
                Player!.Field![Player!.C, Player.R] = default;
                if (setup)
                {
                    var key = Player.Boats.FirstOrDefault(pair => !pair.Value).Key;
                    for (var i = 0; i < key; i++)
                    {
                        Player.Field![Player.C + i, Player.R] = default;
                    }
                }
            }

            enterPressed = false;
        }

        private static void Increment(Pos pos)
        {
            if (pos is Pos.R && Player?.Field![Player!.R + 1, Player.C] != Player!.Char) Player!.R++;
            if (pos is Pos.C && Player?.Field![Player!.R, Player.C + 1] != Player!.Char) Player!.C++;
        }

        private static void Decrement(Pos pos)
        {
            if (pos is Pos.R && Player?.Field![Player!.R - 1, Player.C] != Player!.Char) Player!.R--;
            if (pos is Pos.C && Player?.Field![Player!.R, Player.C - 1] != Player!.Char) Player!.C--;
        }
       
        private static void UpdateBoat() { Player!.Boats[Player.Boats.FirstOrDefault(pair => !pair.Value).Key] = true; }
        public void Run()
        {
            var setup = true;

            DrawField(ref setup);

            var enter = false;
            do
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow when Player!.R > 0:
                        ClearIndex(ref enter, ref setup);
                        Decrement(Pos.R);
                        break;
                    case ConsoleKey.RightArrow when Player!.R < ColLength - 1:
                        ClearIndex(ref enter, ref setup);
                        Increment(Pos.R);
                        break;
                    case ConsoleKey.UpArrow when Player!.C > 0:
                        ClearIndex(ref enter, ref setup);
                        Decrement(Pos.C);
                        Player.C = Math.Abs(Player.C);
                        break;
                    case ConsoleKey.DownArrow when Player!.C < ColLength - 1:
                        ClearIndex(ref enter, ref setup);
                        Increment(Pos.C);
                        Player.C = Math.Abs(Player.C);
                        break;
                    case ConsoleKey.Enter:
                        if (setup)
                        {
                            UpdateBoat();
                        }
                        enter = true;
                        if(!setup) MakeMove();
                        break;
                    case ConsoleKey.Escape:
                    {
                        Menu.Run();
                        break;
                    }
                }
                DrawField(ref setup);
            } while (true);
        } //end Run
    }
}
