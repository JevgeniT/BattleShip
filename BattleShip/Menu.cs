using System;
using System.Collections.Generic;

namespace BattleShip
{
    public static class Menu
    {
        private static int _index;
        private static Settings _settings = new ();
        private static readonly List<string> _mainMenuItems = new() {"New Game", "Resume Game", "Settings", "Exit"};
        private static readonly List<string> _settingsMenu = new() {"Set Length", "Set Width", "Back"};
        private static readonly List<string> _savedGames = Config.ListAll();

        public static void Run()
        {
            Console.CursorVisible = false;
            while (true)
            {
                var menuItem = DrawListMenu(_mainMenuItems);

                switch (menuItem)  //main menu switch
                {
                    case "New Game":
                         _settings = new Settings();
                        var gameEngine = new GameEngine(_settings);
                        gameEngine.Run();
                        break;
                    case "Settings":
                        _index = 0;
                        var loop = true;
                        while (loop)
                        {
                            menuItem = DrawListMenu(_settingsMenu);
                            switch (menuItem)
                            {
                                case "Set Length": _settings.SetLength(); break;
                                case "Set Width": _settings.SetWidth(); break;
                                case "Back": Console.Clear(); loop = false; break;
                            }
                        }
                        break;
                    case "Resume Game":
                        _index = 0;
                        _savedGames.Add("Back");
                        var resumeLoop = true;
                        while (resumeLoop)
                        { 
                            menuItem = DrawListMenu(_savedGames);
                            
                            if (menuItem == _savedGames[_index] && menuItem != "Back")
                            {
                                var dto  = Config.LoadGame(_savedGames[_index]);
                                gameEngine = new GameEngine(dto);
                                gameEngine.Run();
                            }
                            else if (menuItem == "Back")
                            {
                                resumeLoop = false;
                                _savedGames.Remove("Back");
                            }
                        }
                        break;
                    case "Exit": Environment.Exit(0); break;
                }
            }
        }

        private static string DrawListMenu(List<string> items)
        {
            Console.Clear();
            for (var i = 0; i < items.Count; i++)
            {
              var pointer =  i == _index ? "* ":"";
              Console.WriteLine("|{0,9}{1,11}{2,-9}|", pointer, items[i], pointer);
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.DownArrow when _index < items.Count-1: _index++; break;
                case ConsoleKey.UpArrow when _index > 0: _index--; break;
                case ConsoleKey.UpArrow when _index > 0: return items[_index];
                default: if (Console.ReadKey().Key == ConsoleKey.Enter) return items[_index]; break;
            }
            return string.Empty;
        }
    }
}