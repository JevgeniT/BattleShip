using System;
using System.Linq;
using System.Text;
using System.Threading;
using Models;
using static System.ConsoleKey;

namespace BattleShip
{
    public class GameEngine
    {
        private static Axis Axis = Axis.Col;
        private static Player? Player, PlayerA, PlayerB;
        private static readonly StringBuilder sb = new ();
        public static bool SetUp = Player?.SetUp ?? true;
        public Player GetCurrent() => Player!;
        public GameEngine(PlayerDto dto)
        {
            Player = dto.Player;
            PlayerA = dto.PlayerA;
            PlayerB = dto.PlayerB;
        }
        public GameEngine(Settings settings)
        {
            PlayerA = new(settings.Field, '*');
            PlayerB = new((settings.Field.Clone() as char[,])!, '+');
            PlayerA.OpField = PlayerB.Field;
            PlayerB.OpField = PlayerA.Field;
            PlayerA.OpChar = PlayerB.Char;
            PlayerB.OpChar = PlayerA.Char;
            Player = PlayerA;
        }

        public int? RowLength => GetCurrent()?.Field?.GetUpperBound(0) + 1;
        public int? ColLength => RowLength;

        private static bool AllBoatsPlaced()
            => Player!.Boats.FirstOrDefault(pair => !pair.Value).Key == 0;

        public void SwapPlayer() { Player = Player!.Char == '*' ? PlayerB : PlayerA; }

        public void DrawField()
        {
            SetUp = Player!.SetUp;
            Console.Clear();
            if (Player?.CharAt() != Player!.Char)
            {
                if (SetUp)
                {
                    var key = Player!.Boats.FirstOrDefault(pair => !pair.Value).Key;
                    if (AllBoatsPlaced())
                    {
                        Player.SetUp = SetUp = false;
                        SwapPlayer();
                        if (AllBoatsPlaced()) Player.SetUp = SetUp = false;
                    }

                    for (var i = 0; i < key; i++)
                    {
                        if (Axis is Axis.Col && Player.C   + i < ColLength ) Player.SetChar(Player.C + i);
                        else if(Axis is Axis.Row && Player.R + i < RowLength ) Player.SetChar(r: Player.R + i);
                    }
                }
            }
            for (var c = 0; c < ColLength; c++)
            {
                sb.Append($"| ");
                for (var r = 0; r < RowLength; r++)
                {
                   
                    var empty = Player.CharAt(c, r) == default ? $" {default}" : $"{Player.CharAt(c, r)}";
                    var ch = Player.C == c && Player.R == r 
                        ? $" {Player.Char}" 
                        : $" {empty}";
                    sb.Append(ch);
                }
                sb.Append(" |\n");
                Console.Write(sb);
                sb.Clear();
            }

            Console.WriteLine($"{Player.C} { Player.R} {Player.SetUp}");
        }

        public void MakeMove(int? c = null, int? r = null)
        {
            var hitChar = Player?.Char == '+' ? 'x' : 'o';
            Player!.Field![Player.C, Player.R] = Player!.IsHit(c, r) ? hitChar : 'm';
            
            DrawField();
            SwapPlayer();
        }

        private static void UpdateBoat()
        {
            Player!.Boats[Player.Boats.FirstOrDefault(pair => !pair.Value).Key] = true;
        }

        public bool Move(ref bool enter)
        {
            if (SetUp) UpdateBoat(); else MakeMove();
            enter = true;
            return true;
        }

        public void Run()
        {
            bool toMenu = false, enter = false;
            
            DrawField(); 
            static bool ReturnToMenu(){ Menu.Run(); return true; }

            static bool SwapAxis()
            {
                if (!SetUp || Player!.C == 9) return false;
                var ent = false;
                Player!.ClearIndex(Axis, ref ent);
                Axis = Axis is Axis.Col ? Axis.Row : Axis.Col;
                return true;
            }

            while (!toMenu)
            {
                _ = Console.ReadKey().Key switch
                {
                    LeftArrow when Player!.R > 0 => Player.Decrement(Pos.R, Axis, ref enter),
                    RightArrow when Player!.R < ColLength - 1 => Player.Increment(Pos.R, Axis, ref enter),
                    UpArrow when Player!.C > 0 => Player.Decrement(Pos.C, Axis, ref enter), 
                    DownArrow when Player!.C < ColLength - 1 => Player.Increment(Pos.C, Axis, ref enter),
                    Enter => Move(ref enter),
                    Spacebar => SwapAxis(),
                    W => toMenu = ReturnToMenu(), // todo ESC
                    V => Config.Save(new PlayerDto(Player, PlayerA, PlayerB)),
                    _  => true
                };
                
                if (Player?.Hits == 2)
                {
                    Console.Clear();
                    Console.WriteLine($"Player {(GetCurrent().Char == '*' ? "One" : "Two" )} won");
                    Thread.Sleep(3000);
                    toMenu = true;
                }
                DrawField(); 
            }
        }
    }
}