using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Player
    {
        public bool SetUp { get; set; } = true;
        public int R { get; set; } = 5;//row
        public int C { get; set; } = 5;// col
        public char[,]? Field { get; set; } // field with boats
        public char[,]? OpField { get; set; } // opponents field
        public char[,]? MainField() => SetUp ? Field : OpField;

        public int Hits;
        public char Char { get; set; } 
        public char OpChar { get; set; }
        
        /// <summary>
        ///  Key = ship size 1xN
        ///  Value = true -> on field, false -> not yet 
        /// </summary>
        public Dictionary<int, bool> Boats { get; set; }
            =  new() { 
                {1, false},
                {2, false},
                // {3, false},
                // {4, false},
                // {5, false}
            };

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

        public void SetChar(int? c = null, int? r = null, char? ch = null)
        {
            MainField()?.SetValue(ch != null ? default : Char, c ?? C, r ?? R);
        }

        public bool Increment(Pos pos, Axis axis, ref bool enterPressed)
        {
            var longest = Boats.FirstOrDefault(b => !b.Value).Key;
            
            if (pos is Pos.R)
            {
                var prevChar = CharAt(C, R + longest);
                if (axis is Axis.Col && CharAt(C + longest, R+1) != default) return false;
                if (prevChar == Char || R + longest > Field!.GetUpperBound(0)) return false;
                if (prevChar == default) ClearIndex(axis, ref enterPressed);
                R++;
            }

            if (pos is Pos.C)
            {
                if (CharAt(C + longest, R) != default) return false;
                var prevChar = CharAt(C + longest, R);
                if(prevChar == Char || C + longest > Field!.GetUpperBound(0) && axis is Axis.Col) return false;
                if(prevChar == default) ClearIndex(axis, ref enterPressed);
                C++;
            }

            return true;
        }
        
        public bool Decrement(Pos pos, Axis axis, ref bool enterPressed)
        {
            if (pos is Pos.R && CharAt(R - 1, C) == Char) return false; // == Char
            if (pos is Pos.C && CharAt(R, C - 1) == Char) return false; // == Char
            ClearIndex(axis, ref enterPressed);
            if (pos is Pos.R && CharAt(R - 1, C) == default || CharAt(R - 1, C) == OpChar) R--;
            if (pos is Pos.C && CharAt(R, C - 1) == default || CharAt(R, C - 1) == OpChar) C--;
            
            return true;
        }
        
        public void ClearIndex(Axis axis, ref bool enterPressed)
        {
            if (!enterPressed)
            {
                SetChar(ch: '.');
                
                if (SetUp)
                {
                    var key = Boats.FirstOrDefault(pair => !pair.Value).Key;
                    
                    for (var i = 0; i < key; i++)
                    {
                        if (axis is Axis.Col) SetChar(c: C + i, ch: 'v');
                        else SetChar(r: R + i, ch:'v');
                    }
                }
            }
            enterPressed = false;
        }

        public Player(char[,] field, char ch)
        {
            Field = field;
            Char = ch;
        }

        public Player() { }
    }
    
    public enum Pos { R, C }
    public enum Axis { Col, Row }

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