using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DAL;
using Models;
using Newtonsoft.Json;

namespace BattleShip
{
    public static class Config
    {
        private static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/";
        private static readonly AppDbContext DbContext = new();
        private static bool ValidInput(string fileName)
        {
            if (!(!fileName!.Contains(".json") ^ !fileName!.Contains(".db")) || fileName.Length <= 3)
            {
                Console.WriteLine("Invalid input");

                return false;
            }
            return true;
        }

        public static bool Save(PlayerDto dto)
        {
            Console.Write("Enter file name(.db for database, .json otherwise): ");
            var fileName = Console.ReadLine()?.ToLower() ?? string.Empty;

            if (!ValidInput(fileName!)) return false;

            return fileName.Contains(".json")
                switch 
                {
                    true  => SaveToJson(fileName, dto), 
                    false => SaveToDb(fileName, dto), 
                };
        }

        private static bool SaveToDb(string fileName, PlayerDto dto)
        {
            DbContext.Records.Add(
                new DbRecord
                {
                    Id = Guid.NewGuid(),
                    FileName = fileName,
                    Game = JsonConvert.SerializeObject(dto)
                });
            DbContext.SaveChanges();
            return true;
        }
        private static bool SaveToJson(string fileName, PlayerDto dto)
        {
            File.WriteAllText($"{Path}{fileName}.json", JsonConvert.SerializeObject(dto));
            return true;
        }

        public static PlayerDto LoadGame(string name)
        {
           var game = name.Contains(".json") 
                ? File.ReadAllText($"{Path}{name}")
                : DbContext.Records?.FirstOrDefault(r => r.FileName == name)?.Game!;
            
            return JsonConvert.DeserializeObject<PlayerDto>(game)!;
        }

        public static List<string> ListAll()
        {
            var db = DbContext.Records.Select(r => r.FileName).ToList();
            var json =  new DirectoryInfo(Path).GetFiles("*.json").Select(o => o.Name).ToList();

            return json.Concat(db).ToList();
        }
    }
}