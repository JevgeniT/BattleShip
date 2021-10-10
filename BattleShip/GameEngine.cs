using System;
using System.Text;

namespace BattleShip
{
    public struct Player
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class GameEngine
    {
        private Player Player;
        private static char[,] Field { get; set; }

        public GameEngine(Settings settings)
        {
            Field = settings.Field;
            Player = new();
        }
        
        private static int RowLength() => Field.GetUpperBound(0) + 1;

        private static int ColLength() => Field.Length / RowLength();

        private void DrawField()
        {
            Console.Clear();

            if (Field[Player.Y, Player.X] == default)
            {
                Field[Player.Y, Player.X] = '*';
            }
    
            for (int i = 0; i < RowLength(); i++)
            {
                var sb = new StringBuilder("| ");
                for (int j = 0; j < ColLength(); j++)
                {
                    var ch = Field[i, j] == default
                        ? $" {Field[i, j]} "
                        : $" {Field[i, j]}";
                    sb.Append(ch);
                }
                sb.Append(" |\n");
                Console.Write(sb);
            }
        }

        private void MakeMove()
        {
            Field[Player.Y, Player.X] = 'o';
            DrawField();
        }


        private void ClearIndex(ref bool enterPressed)
        {
            if (!enterPressed)
            {
                Field[Player.Y, Player.X] = default;
            }
            enterPressed = false;
        }

        public enum Pos
        {
            X, Y
        }
        public void Increment(Pos pos)
        {
            if (pos is Pos.X && Field[Player.X+1, Player.Y] == default
                || Field[Player.X+1, Player.Y+1] == default) Player.X++;
            
            if (pos is Pos.Y && Field[Player.X, Player.Y+1] == default) Player.Y++;
        }
        public void Decrement(Pos pos)
        {
            if (pos is Pos.X && Field[Player.X-1, Player.Y] == default) Player.X--;
            if (pos is Pos.Y && Field[Player.X, Player.Y-1] == default) Player.Y--;
        }
        
        public void Run()
        {
            DrawField();
            var enter = false;
            do
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow when Player.X > 0:
                        ClearIndex(ref enter);
                        Decrement(Pos.X);
                        break;
                    case ConsoleKey.RightArrow when Player.X < ColLength() - 1:
                        ClearIndex(ref enter);
                        Increment(Pos.X);
                        break;
                    case ConsoleKey.UpArrow: //when Player.Y > 0:
                        ClearIndex(ref enter);
                        Decrement(Pos.Y);
                        Player.Y = Math.Abs(Player.Y);
                        break;
                    case ConsoleKey.DownArrow when Player.Y  < ColLength() - 1:
                        ClearIndex(ref enter);
                        Increment(Pos.Y);
                        Player.Y = Math.Abs(Player.Y);
                        break;
                    case ConsoleKey.Enter:
                        enter = true;
                        MakeMove();
                        break;
                    case ConsoleKey.Escape:
                    {
                        Menu.Run();
                        break;
                    }
                    default:
                        DrawField();
                        break;
                }
                DrawField();
            } while (true);
        } //end Run
    }
}