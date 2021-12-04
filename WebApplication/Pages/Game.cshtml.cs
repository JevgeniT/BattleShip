using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace WebApplication.Pages
{
    public class Game : PageModel
    {
        public readonly char[,] Board;//= new char[10, 10];
        [BindProperty] public List<string> AreChecked { get; set; }
        public static int P1 = 0;
        public static int P2 = 0;
        public   int P11 = 0;
        public   int P22 = 0;
        public GameEngine Engine;

        private static List<GameEngine> list = new ();
        public Player BasePlayer { get; set; }
        
        public Game()
        {
            if (list.Count == 0) list.Add(new (new BattleShip.Settings()));
            Engine = list?.FirstOrDefault();
            BasePlayer = Engine?.GetCurrent();
            Board = BasePlayer?.MainField();
            P11 = P1;
            P22 = P2;
        }
        
        public void OnGet()
        {
            list?.Clear();
            P1 = P2 = 0;
        }
        
        public ActionResult OnPostAsync()
        {
            var r = Current.SetUp = P1 == 2 && P2 == 2;
            foreach (var str in AreChecked)
            {
                var xy = str.Replace("|", "").Split(",");
                var x = int.Parse(xy[0]);
                var y = int.Parse(xy[1]);
               
                if (r)
                {
                    Current.C = y;
                    Current.R = x;
                    // Console.WriteLine($"{Current.IsHit(y, x)} hit {Current.CharAt(y,x )}");
                    // Console.WriteLine($"{Current.C} {Current.R} {y} {x}");
                    Engine.MakeMove(y, x);
                }
                else
                {
                    Board[y, x] = Current.Char; _ = Current.Char == '*' ? P1++ : P2++;
                }
            }
           
            // Engine.DrawField(ref r);
            AreChecked.Clear();
            if (!r)
            {
                Engine?.SwapPlayer();
            }
            return Page();
        }

        private Player Current => Engine.GetCurrent();
    }
}