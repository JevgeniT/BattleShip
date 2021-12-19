using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        public static bool IsOk(string[][] array)
        {
            var len = array.GetUpperBound(0) + 1;
            var arr = new List<int>();
            int row = 0, col = 0;
            
            for (var c = 0; c < len; c++)
            {
                
                for (var r = 0; r < len; r++)
                {
                    if (array[c][r] == "x") row++;  else
                    {
                        arr.Add(row);
                        row = 0;
                    };
                    if (array[r][c] == "x") col++; else
                    {
                        arr.Add(col);
                        col = 0;
                    };
                }
                arr.Add(row);
                arr.Add(col);
                row = col = 0;
            }

            var set = arr.ToHashSet();
            Console.WriteLine(string.Join(",",set));
            return true;
        }

        public static bool IsValid(string[][] array)
        {
            var boats = new List<int>();
            var cols = new List<int>();

            int rowSize = 0, colSize = 0, global = 0;
            
            var rows = array.Select(strings => strings.Count(s => s == "x")).ToList();
            for (var i = 0; i < array.Length; i++)
            {
                for (var x = 0; x < array.GetUpperBound(0) + 1; x++)
                {
                    if (array[x][i] == "x") colSize++;
                }
                cols.Add(colSize);
                colSize = 0;
            }

            var s = rows.Concat(cols).ToHashSet().Sum() -1 ;

            var b = cols.Skip(1);
            var a = rows.Skip(1).Concat(b).ToHashSet().Sum();
            Console.WriteLine($"{a} {string.Join(", ", rows.Concat(b).ToHashSet())}");

            Console.WriteLine("row "+string.Join(",", rows));
            Console.WriteLine("col "+ string.Join(",", cols));
            // for (var i = 0; i < array.Length; i++)
            // {
            //     for (var x = 0; x < array.GetUpperBound(0)+1; x++)
            //     {
            //         if (array[i][x] == "x") rowSize++;
            //         if (array[i][x] != "x") rowSize--;
            //         if (array[x][i] == "x") colSize++;
            //     }
            //     var max = Math.Max(rowSize, colSize);
            //     if (rowSize < colSize && rowSize == 1) max = colSize;
            //     if (!boats.Any(n => n == max))
            //     {
            //         boats.Add(max); 
            //     }
            //
            //     rowSize = colSize = 0;
            // }
            //
            // Console.WriteLine(string.Join(",", boats));
            return true;
        }
    }
}