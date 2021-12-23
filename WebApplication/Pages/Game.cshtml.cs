using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace WebApplication.Pages
{
    public class Game : PageModel
    {
        #region init
        [BindProperty] public List<string> AreChecked { get; set; }
        [BindProperty] public bool ToDb { get; set; }
        [BindProperty] public int Height { get; set; }
        [BindProperty] public int Width { get; set; }
        [BindProperty] public static bool FieldChanged { get; set; }
        public GameEngine Engine;
        public string Message { get; set; }
        private static List<GameEngine> list = new ();
        private Player Current => Engine.GetCurrent();
        #endregion

        public Game()
        {
            if (list.Count == 0) list.Add(new (new Settings()));
            Engine = list?.FirstOrDefault();
        }

        public ActionResult OnGet([FromQuery] string gameName)
        {
            if (!string.IsNullOrEmpty(gameName))
            {
                var engine = new GameEngine(Config.LoadGame(gameName));
                list.Add(engine);

                return Page();
            }

            list = new() { new (new Settings()) };
            return Page();
        }

        public ActionResult OnPost()
        {
            Message = AreChecked.Count switch
            {
                0 => "Invalid move",
                not 1 when !Current.SetUp => "Invalid move",
                _ => null
            };
            
            if (Message is not null) return Page();
            
            foreach (var str in AreChecked)
            {
                var xy = str.Replace("|", "").Split(",");
                var r = int.Parse(xy[0]);
                var c = int.Parse(xy[1]);

                if (Current.SetUp)
                {
                    Current!.Field![c, r] = Current.Char;
                    Current.BoatsSum++;
                }
                
                else Engine.MakeMove(Current.C = c, Current.R = r, true);
            }
            
            if (Current.SetUp)
            {
                if(!IsOk(Current.MainField()))
                {
                    Current.BoatsSum = 0;
                    Message = "not ok";
                    Current.Field = new char[Engine.FieldLength.Value, Engine.FieldLength.Value];
                    return Page();
                }
                Current.SetUp = false;
                Engine.SwapPlayer();
            }

            var isWin = Current.BoatsSum != 0 && Current.Hits >= Current.BoatsSum;
            if (isWin && !Current.SetUp)
            {
                return RedirectToPage(
                    "./WinnerPage",
                    new {winner= Current.Char.ToString()}
                    );
            }
           
            return Page();
        }

        public IActionResult OnPostSave()
        {
            var current = Current;
            var dto = new PlayerDto
            {
                Player = current,
            };
            if (current.Char == '*')
            {
                dto.PlayerA = current;
                Engine.SwapPlayer();
                dto.PlayerB = Current;
            }
            else
            {
                dto.PlayerB = current;
                Engine.SwapPlayer();
                dto.PlayerA = Current;
            }
            Engine.SwapPlayer();
            
            var fileName = $"Web-{DateTime.Now.Hour}:{DateTime.Now.Minute}.{(ToDb ? "db" : "json")}";
            Config.Save(dto, fileName);
            Message = $"Game saved as {fileName}";

            return Page();
        }

        public IActionResult OnPostSaveSettings()
        {
            var validSize = Height is <= 10 or >= 20 && Width is <= 10 or >= 20;
            if (validSize) return Page();
            list.Clear();
            list.Add(new GameEngine(new Settings(Height, Width)));
            FieldChanged = true;
            
            return new PageResult();
        }
        
        private bool IsOk(char[,] array)
        {
            var len = array.GetUpperBound(0) + 1;
            var arr = new List<int>();
            int row = 0, col = 0;
            
            for (var c = 0; c < len; c++)
            {
                for (var r = 0; r < len; r++)
                {
                    if (array[c,r] == Current.Char) row++; else
                    {
                        arr.Add(row);
                        row = 0;
                    };
                    if (array[r,c] == Current.Char) col++; else
                    {
                        arr.Add(col);
                        col = 0;
                    };
                }
                arr.Add(row);
                arr.Add(col);
                row = col = 0;
            }

            var sum = arr.ToHashSet().Sum();
            if (Current.BoatsSum > 0)
            {
                return sum-1  == Current.BoatsSum;
            }
            
            return  arr.ToHashSet().Sum() == 4;
        }
    }
}