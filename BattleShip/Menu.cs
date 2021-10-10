using System;
using System.Collections.Generic;

namespace BattleShip
{
    public static class Menu
    {
        private static int _index;
        private static readonly List<string> _mainMenuItems = new() {"New Game", "Resume Game", "Settings", "Exit"};
        private static readonly List<string> _settingsMenu = new() {"Set Length", "Set Width", "Back"};
        // private readonly List<string> _savedGames = new Config().ListAll();
        private  static Settings _settings = new ();
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
                        do
                        {
                            menuItem = DrawListMenu(_settingsMenu);
                            switch (menuItem) // settings switch 
                            {
                                case "Set Length": _settings.SetLength();
                                    break;
                                case "Set Width": _settings.SetWidth();
                                    break;
                                case "Back": Console.Clear();
                                loop = false;
                                break;
                            } //end settings switch
                        } while (loop) ;//end do
                        break;
                    
                    case "Resume Game":
                        _index = 0;
                        // _savedGames.Add("Back");
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                } //main switch end
            } // end while 
        }

        private static string DrawListMenu(List<string> items)
        {
            Console.Clear();
            for (var i = 0; i < items.Count; i++)
            {
              var pointer =  i == _index ? "**":"";
              Console.WriteLine("|{0,9}{1,11}{2,-9}|", pointer, items[i], pointer);
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.DownArrow when _index < items.Count-1:
                    _index++;
                    break;
                case ConsoleKey.UpArrow when _index > 0:
                    _index--;
                    break;
                default:
                {
                    if (Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        return items[_index];
                    }
                    break;
                }
            }

            return string.Empty;
        }
    }
}