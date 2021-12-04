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
        private static Player? Player, PlayerA, PlayerB;
        private static StringBuilder sb = new ();
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
        
        public  static int? RowLength => Player?.Field?.GetUpperBound(0) + 1;
        public  static int? ColLength => Player?.Field?.Length / RowLength;

        private static bool AllBoatsPlaced()
            => Player!.Boats.FirstOrDefault(pair => !pair.Value).Key == 0;

        public void SwapPlayer()
        {
            // Console.WriteLine(Player?.Char);
            Player = Player!.Char == '*' ? PlayerB : PlayerA;
        }
        
        public void DrawField(ref bool setup)
        {
            Player!.SetUp = setup;
            Console.Clear();
            if (Player?.CharAt() != Player!.Char)
            {
                if (setup)
                {
                    var key = Player!.Boats.FirstOrDefault(pair => !pair.Value).Key;
                    if (AllBoatsPlaced())
                    {
                        SwapPlayer();
                        if (AllBoatsPlaced()) setup = false;
                    }
                    for (var i = 0; i < key; i++) Player.SetChar(Player.C + i);
                }
                // Player.SetChar();
            }
            for (var c = 0; c < ColLength; c++)
            {
                sb.Append($"| ");
                for (var r = 0; r < RowLength; r++)
                {
                    var ch = Player.C == c && Player.R == r ? $" {Player.Char}" : $" {Player.CharAt(c, r)} ";
                    sb.Append(ch);
                }
                sb.Append(" |\n");
                Console.Write(sb);
                sb.Clear();
            }
            Console.WriteLine($"{Player.C} : {Player.R}'");

        }

        public void MakeMove(int? c = null, int?r = null)
        {
            var set = false;
            var hitChar = Player?.Char == '+' ? 'x' : 'o';
            Player!.Field![Player.C, Player.R] = Player!.IsHit(c, r) ? hitChar : 'm';
            
            DrawField(ref set);
            SwapPlayer();
        }
        
        private static void UpdateBoat() { Player!.Boats[Player.Boats.FirstOrDefault(pair => !pair.Value).Key] = true; }

        private  bool Move(ref bool enter, ref bool setup)
        {
            if (setup) UpdateBoat();
            enter = true;
            if(!setup) MakeMove();
            return true;
        }
        
        public void Run()
        {
            var setup = true;
            var toMenu = false;
            var enter = false;
            DrawField(ref setup); 
            static bool ReturnToMenu(){ Menu.Run(); return true; } // local function
            
            while (!toMenu)
            {
                _ = Console.ReadKey().Key switch
                {
                    LeftArrow when Player!.R > 0 => Player.Decrement(Pos.R, ref enter, ref setup),
                    RightArrow when Player!.R < ColLength - 1 => Player.Increment(Pos.R, ref enter, ref setup),
                    UpArrow when Player!.C > 0 => Player.Decrement(Pos.C, ref enter, ref setup), 
                    DownArrow when Player!.C < ColLength - 1 => Player.Increment(Pos.C, ref enter, ref setup),
                    Enter => Move(ref enter, ref setup),
                    W => toMenu = ReturnToMenu(), // todo ESC
                    V => Config.Save(new PlayerDto(Player, PlayerA, PlayerB)),
                    _  => true
                };
                DrawField(ref setup); 
            }
        }

        public Player GetCurrent() => Player!;
    }
}