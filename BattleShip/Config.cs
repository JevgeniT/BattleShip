using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace BattleShip
{
    public static class Config
    {
        private static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/";
        private static bool ValidInput(string fileName)
        {
            if (fileName.Length >= 3) return true;
            Console.WriteLine("Size has to be at least 3");
            return false;
        }

        public static bool Save(PlayerDto dto)
        {
            Console.Write("ENTER FILE NAME: ");
            var fileName = Console.ReadLine()?.ToLower();

            if (File.Exists($"{Path}{fileName}.json") && ValidInput(fileName))
            {
                Console.WriteLine("The file already exists. Do you want to overwrite it? [y/n]");
                if (Console.ReadLine() == "y") return InnerSave(fileName, dto);
            }
            else if (!File.Exists($"{Path}{fileName}.json") && ValidInput(fileName)) return InnerSave(fileName, dto);

            return false;
        }

        private static bool InnerSave(string fileName, PlayerDto dto)
        {
            File.WriteAllText($"{Path}{fileName}.json", JsonConvert.SerializeObject(dto));
            Console.WriteLine($"{Path}{fileName}.json");
            Thread.Sleep(5000);
            Console.WriteLine("GAME SAVED");
            return true;
        }

        public static PlayerDto LoadGame(string name) 
            => JsonConvert.DeserializeObject<PlayerDto>(File.ReadAllText($"{Path}{name}"))!;

        public static List<string> ListAll() 
            => new DirectoryInfo(Path).GetFiles("*.json").Select(o => o.Name).ToList();
    }
}