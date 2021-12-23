using System;
using System.Collections.Generic;
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
        private static readonly StringBuilder sb = new ();
        public const int Boats = 3;
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

        public int? FieldLength => GetCurrent()?.Field?.GetUpperBound(0) + 1;

        public void SwapPlayer() { Player = Player!.Char == '*' ? PlayerB : PlayerA; }
        public Player GetOpposite() => (Player?.Char == '*' ? PlayerB : PlayerA)!;

        public void DrawField()
        {
            Console.Clear();
   
            for (var c = 0; c < FieldLength; c++)
            {
                sb.Append($"| ");
                for (var r = 0; r < FieldLength; r++)
                {
                    var empty = Player!.OpField![c, r];
                    var d = empty == Player.OpChar ? $" {default} " : $" {empty.ToString()} ";
                    var ch = Player!.C == c && Player.R == r && !Player!.SetUp
                        ? $" {Player.Char}" 
                        : $"{d}";
                    
                    sb.Append(ch);
                }
                sb.Append(" | ");
                Console.Write(sb);
                sb.Clear();
                
                // second field 
                sb.Append($" | ");
                for (var r = 0; r < FieldLength; r++)
                {
                    var empty = Player!.Field![c, r] == default ? $" {default} " : $" {Player!.Field![c, r]}";
                    sb.Append(empty);
                }
                sb.Append(" |\n");
                Console.Write(sb);
                sb.Clear();
            }
        }

        public bool MakeMove(int? c = null, int? r = null, bool isWeb = false)
        {
            var hitChar = Player?.Char == '+' ? 'x' : 'o';
            if (!isWeb)
            {
                Player!.OpField![Player.C, Player.R] = Player!.IsHit(c, r) ? hitChar : 'm';
            }
            else
            {
                Player!.Field![Player.C, Player.R] = Player!.IsHit(c, r) ? hitChar : 'm';
            }            
            DrawField();
            SwapPlayer();
            return true;
        }

        private bool GenerateBoats(char[,]? arr, out int sum)
        {
            sum = 0;
            var rnd = new Random();
            var rand = new Stack<int>(new int[]{1, 0, 1,0,1}); // 0 = r, 1 =c 
            for (int i = 0; i < Boats; i++)
            {
                var placed = false;
                var c = rnd.Next(1, 9);
                var r = rnd.Next(1, 9);
                var pos = rand.Pop();
                int xx = 0;
                while (!Valid(c, r, i, arr))
                {
                    r = rnd.Next(1, 9);
                    c = rnd.Next(1, 9);
                    xx++;
                    if (xx > 10) break;
                }
                if (pos == 0)
                {
                    if (r + i > 9 || arr?[r+1, i] != null) r = rnd.Next(0, 9);
                    for (var x = 0; x <= i; x++)
                    {
                        arr![r, x] = Player!.Char;
                        placed = true;
                        sum ++;

                    }
                }
            
                if (pos == 1)
                {   
                    if (c + i > 9 ||  arr?[i, c] != null) c = rnd.Next(0, 9);
                    for (var x = 0; x <= i; x++)
                    {
                        arr![x, c] = Player!.Char;
                        placed = true;
                        sum ++;
                    }
                }

                if (!placed)
                {
                    i--;
                    rand.Push(pos); // can't place, push back, retry
                }
            }
            
            return false;
        }
        static bool Valid(int c, int r, int l,  char[,]? arr)
        {
            if (c + l > 9 || r + l > 9) return false;
            for (int i = 0; i < l; i++)
            {
                if (arr?[c+i, r] != null || arr?[c, r+i] != null ) return false;
            }
        
            return true;
        }
        public void Run()
        {
            bool toMenu = false, isRestored = false;
            GenerateBoats(Player!.Field, out var sum);
            DrawField();
        
            bool Flush()
            {
                Player!.Field = new char[10, 10];
                return false;
            }
            if (Player.Field!.Cast<char>().Any(c => new List<char>(c).Any(ch => ch != default)))
            {
                isRestored = true;
            }
            while (Player!.SetUp && !isRestored)
            {
                
                var placed = Console.ReadKey().Key switch
                {
                    Spacebar => Flush(),
                    Enter => true,
                    _ => false,
                };
                if (placed)
                {
                    GetOpposite().OpField = Player.Field;
                    Player.SetUp = false;
                    Player.BoatsSum = sum;
                    SwapPlayer();
                }
                else
                {
                    GenerateBoats(Player!.Field,  out _);
                }
                DrawField();
            }
            
            
            DrawField(); 
            static bool ReturnToMenu(){ Menu.Run(); return true; }

            while (!toMenu)
            {
                _ = Console.ReadKey().Key switch
                {
                    LeftArrow when Player!.R > 0 => Player.Decrement(Pos.R),
                    RightArrow when Player!.R < FieldLength - 1 => Player.Increment(Pos.R),
                    UpArrow when Player!.C > 0 => Player.Decrement(Pos.C),
                    DownArrow when Player!.C < FieldLength - 1 => Player.Increment(Pos.C),
                    Enter => MakeMove(isWeb:false),
                    W => toMenu = ReturnToMenu(), // todo ESC
                    V => Config.Save(new PlayerDto(Player, PlayerA, PlayerB)),
                    _  => true
                };
                
                if (Player?.Hits == Player!.BoatsSum)
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