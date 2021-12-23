
namespace Models
{
    public class Player
    {
        public bool SetUp { get; set; } = true;
        public int R { get; set; } //= 5;//row
        public int C { get; set; } //= 5;// col
        public char[,]? Field { get; set; } // field with boats
        public char[,]? OpField { get; set; } // opponents field
        public char[,]? MainField() => SetUp ? Field : OpField;
        public int BoatsSum;
        public int Hits;
        public char Char { get; set; } 
        public char OpChar { get; set; }
    
        public bool IsHit(int? c = null, int? r = null)
        {
            var isHit = OpField![c ?? C, r ?? R] == OpChar;
            if (isHit) Hits++;
            return isHit;
        }

        public char CharAt(int? c = null, int? r = null)
        {
            if ((c ?? C) is < 0 or > 9 || (r ?? R) is < 0 or > 9 ) return default;
            if(!SetUp && MainField()![c ?? C, r ?? R] == OpChar) return default;
            return MainField()![c ?? C, r ?? R];
        }


        public bool Increment(Pos pos)
        {
            var longest = 0;
            
            if (pos is Pos.R)
            {
                var prevChar = CharAt(C, R + longest);
                if (prevChar == Char || R + longest > Field!.GetUpperBound(0)) return false;
                R++;
            }

            if (pos is Pos.C)
            {
                if (CharAt(C + longest, R) != default) return false;
                var prevChar = CharAt(C + longest, R);
                C++;
            }

            return true;
        }
        
        public bool Decrement(Pos pos)
        {
            if (pos is Pos.R && CharAt(R - 1, C) == Char) return false; // == Char
            if (pos is Pos.C && CharAt(R, C - 1) == Char) return false; // == Char
            if (pos is Pos.R && CharAt(R - 1, C) == default || CharAt(R - 1, C) == OpChar) R--;
            if (pos is Pos.C && CharAt(R, C - 1) == default || CharAt(R, C - 1) == OpChar) C--;
            
            return true;
        }

        public Player(char[,] field, char ch)
        {
            Field = field;
            Char = ch;
        }

        public Player() { }
    }
    
    public enum Pos { R, C }

    public class PlayerDto
    {
        public Player? Player { get; set; }
        public Player? PlayerA { get; set; }
        public Player? PlayerB { get; set; }

        public PlayerDto(Player? player, Player? playerA, Player? playerB)
        {
            Player = player;
            PlayerA = playerA;
            PlayerB = playerB;
        }

        public PlayerDto()
        {
        }
    }
}