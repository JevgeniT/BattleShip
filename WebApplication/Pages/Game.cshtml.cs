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
        public static int P1, P2;
        public int P11, P22;
        public GameEngine Engine;
        public string Message { get; set; }
        private static List<GameEngine> list = new ();
        public Player BasePlayer { get; set; }

        private Player Current => Engine.GetCurrent();
        #endregion

        public Game()
        {
            if (list.Count == 0) list.Add(new (new BattleShip.Settings()));
            Engine = list?.FirstOrDefault();
            BasePlayer = Engine?.GetCurrent();
            P11 = P1;
            P22 = P2;
        }

        public ActionResult OnGet([FromQuery] string gameName)
        {
            if (!string.IsNullOrEmpty(gameName))
            {
                var engine = new GameEngine(Config.LoadGame(gameName));
                list.Add(engine);
                return Page();
            }
            
            list?.Clear();
            P1 = P2 = 0;
            return Page();
        }

        public ActionResult OnPost()
        {
            foreach (var str in AreChecked)
            {
                var xy = str.Replace("|", "").Split(",");
                var r = int.Parse(xy[0]);
                var c = int.Parse(xy[1]);

                if (Current.SetUp)
                {
                    
                    Current.Field[c, r] = Current.Char;
                }
                else
                {
                    Current.C = c;
                    Current.R = r;
                    Engine.MakeMove(c, r);
                }
            }

            AreChecked.Clear();

            if (Current.SetUp)
            {
                if(!IsOk(Current.MainField()))
                {
                    Message = "not ok";
                    return Page();
                }
                Current.SetUp = false;
                Engine.SwapPlayer();
            }

            if (Current.Hits >= 2)
            {
                
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
            if (Height <= 11) return Page();
            list.Clear();
            list.Add(new GameEngine(new BattleShip.Settings(Height, Width)));
            FieldChanged = true;
            return new PageResult();
        }
        
        public bool IsOk(char[,] array)
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
            Console.WriteLine(string.Join(",", arr.ToHashSet()));
            return arr.ToHashSet().Sum()==4;
        }
    }
}