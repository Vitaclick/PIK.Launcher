using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PIK.Launcher;
using static System.Console;

namespace PikCorrector
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;

      var settings = new Settings();

      if (System.IO.File.Exists(settings.configFile))
      {
        System.Console.WriteLine("Найдены профили с настройками. Выберите один либо нажмите ESC для продолжения:");
        var jsonSettings = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(settings.configFile));
        Console.Clear();
        foreach (var config in jsonSettings.configurations)
        {
          var result = settings.Install(config.name, config.args);
        }
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

      var mainOptions = settings.operations;
      var menu = new Menu(mainOptions);
      menu.Promt(2);

      var opts = menu.SelectedItems.Select(x => mainOptions[x]).ToList();
      settings.sessionConfigs.AddRange(menu.SelectedItems);

      List<Config> configList = new List<Config>();
      foreach (var item in menu.SelectedItems)
      {
        var config = new Config();
        config.name = item;
        configList.Add(config);
      }
      var sessionConfigurations = new Configurations()
      {
        profile = "1",
        configurations = configList
      };
      string json = JsonConvert.SerializeObject(sessionConfigurations);

      System.IO.File.WriteAllText(settings.configFile, json);

      // foreach (var opt in opts)
      // {
      //   if (opt == Configurations.InstallPik)
      //   {
      //     System.Console.WriteLine("\nОтключить плагины PIK:");
      //     var pikList = new string[] {
      //       "Other addins (Tesla, Ostec, ...)",
      //       "RevitNameValidator",
      //       "OkCommand",
      //       "BimInspector.Revit",
      //       "InspectorConfig",
      //       "ChangeManager",
      //       "FamilyManager",
      //       "FamilyExplorer"
      //     };

      //     var pikMenu = new Menu(pikList);
      //     var selection = Promt(pikMenu, mainOptions.Count + 4);
      //     Configurations.InstallPik(selection.ToList(), selection.Contains(pikList.First()));
      //   }
      //   opt.DynamicInvoke();
      // }

      process.StandardInput.WriteLine("ipconfig");
      process.StandardInput.Flush();
      process.StandardInput.Close();
      System.Console.WriteLine(process.StandardOutput.ReadToEnd());
      Console.ReadKey();

      process.WaitForExit();
    }
  }
}

