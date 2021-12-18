using System;

namespace BattleShip
{
    public class Settings
    {
        public char[,] Field { get; set; } = new char[10, 10];
        private   int Length { get; set; } = 10;
        private   int Width { get; set; } = 10;

        public Settings() { }
        public Settings(int height, int width)
        {
            Length = height;
            Width = width;
            Field = new char[height, width];
        }
        public void SetLength()
        {
            Console.Write("Enter field length: ");
            if (int.TryParse(Console.ReadLine(), out var length) )
            {
                if (length is >= 10 and <= 30) Length = length;
                Console.WriteLine("Length has been changed to {0}", Length);
            }
            else
            {
                Console.WriteLine("Invalid input! Must be integer between 10-20");
            }
            Console.ReadKey();
        }

        public  void SetWidth()
        { 
            Console.Write("Enter field width: ");
            if (int.TryParse(Console.ReadLine(), out var width) )
            {
                if (width is >= 10 and <= 20) Width = width; 
                Console.WriteLine("Width has been changed to {0}", Width);
            }
            else
            {
                Console.WriteLine("Invalid input! Must be integer between 10-20");
            }
            Console.ReadKey();
        }
    }
}