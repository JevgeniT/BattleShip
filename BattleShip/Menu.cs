using System;
using System.Collections.Generic;

namespace BattleShip
{
    public static class Menu
    {
        private static int _index;
        private readonly static  List<string> _mainMenuItems = new() {"New Game", "Resume Game", "Settings", "Exit"};
        private readonly static List<string> _settingsMenu = new() {"Set Length", "Set Width", "Back"};
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
                                case "Set Length":
                                    _settings.SetLength();
                                    break;
                                case "Set Width":
                                    _settings.SetWidth();
                                    break;
                                case "Back":
                                Console.Clear();
                                loop = false;
                                break;
                            } //end settings switch
                        } while (loop) ;//end do
                        break;
                    
                    case "Resume Game":
                        _index = 0;
                        // _savedGames.Add("Back");
                        var resumeLoop = true;
                        // do
                        // { 
                        //     menuItem = DrawListMenu(_savedGames);
                        //     
                        //     if (menuItem == _savedGames[_index] && menuItem != "Back")
                        //     {
                        //         _settings  = new Config().LoadGame(_savedGames[_index]);
                        //         gameEngine = new GameEngine(_settings);
                        //         
                        //         gameEngine.Run();
                        //     }
                        //     else if (menuItem == "Back")
                        //     {
                        //         resumeLoop = false;
                        //         _savedGames.Remove("Back");
                        //     }
                        // } while (resumeLoop);
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

            var ckey = Console.ReadKey();
            switch (ckey.Key.ToString())
            {
                case "S"  when _index < items.Count-1:
                    _index++;
                    break;
                case "DownArrow" when _index < items.Count-1:
                    _index++;
                    break;
                case "W" when _index > 0:
                    _index--;
                    break;
                default:
                {
                    if (ckey.Key == ConsoleKey.Enter)
                    {
                        return items[_index];
                    }
                    break;
                }
            }
            return "";
        }
    }
}