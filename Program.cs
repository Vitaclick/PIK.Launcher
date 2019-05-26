using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Newtonsoft.Json;
using static System.Console;

namespace PIK.Launcher
{
  class Program
  {
    static void Main(string[] args)
    {
      OutputEncoding = Encoding.UTF8;

      var settings = new Settings();

      if (File.Exists(settings.configFile))
      { 
        if (Menu.PromptYesNo("Найдены предыдущие настройки. Продложить с ними либо нажмите ESC для отмены."))
        {
          var jsonSettings = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(settings.configFile));
          if (jsonSettings != null)
          {
            settings.Install(jsonSettings.configurations);
          }
        }

          
        //foreach (var config in jsonSettings.configurations)
        //{
        //  var result = settings.Install(config.name, config.args);
        //  WriteLine($"Установка {config.name} ... " + (result == 100 ? "OK" : "ERROR"));
        //}

      }
      else
      {
        File.WriteAllText(settings.configFile, string.Empty);
      }

      //var process = new Process
      //{
      //  StartInfo = {
      //    FileName = "cmd.exe",
      //    CreateNoWindow = true,
      //    RedirectStandardInput = true,
      //    RedirectStandardOutput = true,
      //    RedirectStandardError = true
      //  }
      //};
      //process.Start();

      // main menu
      Clear();
      WriteLine("Выберите опции (пробел). По завершению нажмите Enter");

      var mainMenu = new Menu(settings.operations.Keys.ToArray());
      var selectedConfigs = mainMenu.Prompt(2).ToList();

      //var opts = selectedConfigs.Select(x => settings.operations[x]).ToList();
      Clear();
      settings.Install(selectedConfigs);
      //foreach (var opt in opts)
      //{
      //  opt(null);
      //}

      if (Menu.PromptYesNo("Сохранить настройки?"))
      {
        settings.SaveSettings();
      }

      //process.WaitForExit();

    }
  }
}

