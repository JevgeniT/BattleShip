using System.Collections.Generic;

namespace BattleShip
{
    public class Player
    {
        public char Id { get; set; }
        public int R { get; set; } = 5;//row
        public int C { get; set; } = 5;// col
        public char[,]? Field { get; set; }
        public bool HisTurn { get; set; }
        public char Char { get; set; }
        public Dictionary<int, bool> Boats { get; set; } =  new() {{1, false}, {2, false}, {3, false}, {4, false}, {5, false}};

        public Player(int r, int c, char[,]? field, bool hisTurn)
        {
            R = r;
            C = c;
            Field = field;
            HisTurn = hisTurn;
        }

        public Player()
        {
            
        }
    }
}