using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using PIK.Launcher;
using static System.Console;

namespace PikCorrector
{
  class Program
  {
    static ICollection<string> Promt(Menu menu, int verticalOffset)
    {
      var menuPainter = new MenuPainter(menu);

      bool done = false;
      do
      {
        menuPainter.Paint(1, verticalOffset);
        var keyInfo = Console.ReadKey();

        switch (keyInfo.Key)
        {
          case ConsoleKey.UpArrow: menu.MoveUp(); break;
          case ConsoleKey.DownArrow: menu.MoveDown(); break;
          case ConsoleKey.Spacebar: menu.Select(); break;
          case ConsoleKey.Enter: done = true; break;
        }
      }
      while (!done);

      Debug.WriteLine("Выбранная(ые) опция: " + (string.Join(",", menu.SelectedItems) ?? "(nothing)"));

      return menu.SelectedItems;
    }
    static void Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;

      var configFile = Environment.GetEnvironmentVariable("appdata")
        + @"\PIK.Launcher\launcherSettings.json";
      if (System.IO.File.Exists(configFile))
      {
        System.Console.WriteLine("Запустить с последними настройками?");
        IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile(configFile, true, true)
          .Build();
        var c = config["name"];
        System.Console.WriteLine(c);

      }
      else
      {
      }

      var process = new Process();
      process.StartInfo.FileName = "cmd.exe";
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardInput = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.Start();

      // main menu
      System.Console.WriteLine("Выберите опции (пробел). По завершению нажмите Enter");
      var mainOptions = new Dictionary<string, Action> {
        {"Плагины PIK", Configurations.InstallPik},
        {"Assembler", Configurations.InstallAssembler},
        {"WeAndRevit", Configurations.InstallWeAndRevit},
        {"Очистить от всех плагинов", Configurations.ClearAllPlugins}
      };

      var menu = new Menu(mainOptions);
      Promt(menu, 2);

      var opts = menu.SelectedItems.Select(x => mainOptions[x]).ToList();
      foreach (Action opt in opts)
      {
        if (opt == Configurations.InstallPik)
        {
          System.Console.WriteLine("\nОтключить плагины PIK:");
          var pikList = new string[] {
            "Other addins (Tesla, Ostec, ...)",
            "RevitNameValidator",
            "OkCommand",
            "BimInspector.Revit",
            "InspectorConfig",
            "ChangeManager",
            "FamilyManager",
            "FamilyExplorer"
          };

          var pikMenu = new Menu(pikList);
          var selection = Promt(pikMenu, mainOptions.Count + 4);
          Configurations.InstallPik(selection.ToList(), selection.Contains(pikList.First()));
        }
        opt.DynamicInvoke();
      }

      process.StandardInput.WriteLine("ipconfig");
      process.StandardInput.Flush();
      process.StandardInput.Close();
      System.Console.WriteLine(process.StandardOutput.ReadToEnd());
      Console.ReadKey();

      process.WaitForExit();
    }
  }
}

