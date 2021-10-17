using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace BattleShip
{
    public class GameEngine
    {
        private static Player? Player, PlayerA, PlayerB;

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
            Player = PlayerA;
        }
        
        private static int? RowLength => Player?.Field?.GetUpperBound(0) + 1;
        private static int? ColLength => Player?.Field?.Length / RowLength;

        private static bool AllBoatsPlaced()
            => Player!.Boats.FirstOrDefault(pair => !pair.Value).Key == 0;

        private static void SwapPlayer()
        {
            Player = Player!.Char == '*' ? PlayerB : PlayerA;
            Thread.Sleep(3000);
        }
        private static void DrawField(ref bool setup)
        {
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
                Player.SetChar();
            }
            var sb = new StringBuilder();
            for (var c = 0; c < ColLength; c++)
            {
                sb.Append($"| ");
                for (var r = 0; r < RowLength; r++)
                {
                    var ch = Player.CharAt(c, r) != Player!.Char
                        ? $" {Player.CharAt(c, r)} "
                        : $" {Player.CharAt(c, r)}";
                    sb.Append(ch);
                }
                sb.Append(" |\n");
                Console.Write(sb);
                sb.Clear();
            }
        }

        private static void MakeMove()
        {
            var set = false;
            Player!.Field![Player!.C, Player!.R] = 'o';
            DrawField(ref set);
            SwapPlayer();
        }
        
        private static void UpdateBoat() { Player!.Boats[Player.Boats.FirstOrDefault(pair => !pair.Value).Key] = true; }

        private static bool Move(ref bool enter, ref bool setup)
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
            static bool ReturnToMenu(){ Menu.Run(); return true;} // local function
            
            while (!toMenu)
            {
                _ = Console.ReadKey().Key switch
                {
                    ConsoleKey.LeftArrow when Player!.R > 0 => Player.Decrement(Pos.R, ref enter, ref setup),
                    ConsoleKey.RightArrow when Player!.R < ColLength - 1 => Player.Increment(Pos.R, ref enter, ref setup),
                    ConsoleKey.UpArrow when Player!.C > 0 => Player.Decrement(Pos.C, ref enter, ref setup), 
                    ConsoleKey.DownArrow when Player!.C < ColLength - 1 => Player.Increment(Pos.C, ref enter, ref setup),
                    ConsoleKey.Enter => Move(ref enter, ref setup),
                    ConsoleKey.W => toMenu = ReturnToMenu(), // todo ESC
                    ConsoleKey.V => Config.Save(new PlayerDto(Player, PlayerA, PlayerB)),
                    _  => true
                };
                DrawField(ref setup); 
            }
        }
    }
}