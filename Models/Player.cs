using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Player
    {
        public bool SetUp { get; set; }
        public int R { get; set; } = 5;//row
        public int C { get; set; } = 5;// col
        public char[,]? Field { get; set; } // field with boats
        public char[,]? OpField { get; set; } // opponents field
        public char[,]? MainField()
        {
            return SetUp ? Field : OpField;
        }

        public char Char { get; set; } // player's key pointer and boat on field
        
        /// <summary>
        ///  Key = ship size 1xN
        ///  Value = true -> on field, false -> not yet 
        /// </summary>
        public Dictionary<int, bool> Boats { get; set; }
            =  new() {{1, false}, {2, false}, 
                // {3, false}, {4, false}, {5, false}
            };

        public char CharAt(int? c = null, int? r = null) => MainField()![c ?? C, r ?? R];
        public void SetChar(int? c = null, int? r = null, char? ch = null)
        {
            c ??= C;
            r ??= R;
            ch = ch != null ? default : Char;
            
            MainField()?.SetValue(ch, c.Value, r.Value);
        }
       
        public bool Increment(Pos pos, ref bool enterPressed, ref bool setup)
        {
            ClearIndex(ref enterPressed, ref setup);
            if (pos is Pos.R && MainField()![R + 1, C] != Char) R++;
            if (pos is Pos.C && MainField()![R, C + 1] != Char) C++;
            return true;
        }
        
        public bool Decrement(Pos pos, ref bool enterPressed, ref bool setup)
        {
            ClearIndex(ref enterPressed, ref setup);
            if (pos is Pos.R && MainField()![R - 1, C] != Char) R--;
            if (pos is Pos.C && MainField()![R, C - 1] != Char) C--;
            R = Math.Abs(R);
            C = Math.Abs(C);
            return true;
        }
        
        private void ClearIndex(ref bool enterPressed, ref bool setup)
        {
            if (!enterPressed)
            {
                SetChar(ch: ' ');
                if (setup)
                {
                    var key = Boats.FirstOrDefault(pair => !pair.Value).Key;
                    for (var i = 0; i < key; i++) SetChar(c: C + i, ch: 'v');
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
    public enum Pos{ R, C }

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