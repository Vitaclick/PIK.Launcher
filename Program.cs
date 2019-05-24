using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        WriteLine("Найдены профили с настройками. Выберите один либо нажмите ESC для продолжения:");
        var jsonSettings = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(settings.configFile));


        //foreach (var config in jsonSettings.configurations)
        //{
        //  var result = settings.Install(config.name, config.args);
        //  WriteLine($"Установка {config.name} ... " + (result == 100 ? "OK" : "ERROR"));
        //}
        settings.Install(jsonSettings.configurations);
        ReadKey();
      }

      var process = new Process
      {
        StartInfo = {
          FileName = "cmd.exe",
          CreateNoWindow = true,
          RedirectStandardInput = true,
          RedirectStandardOutput = true,
          RedirectStandardError = true
        }
      };
      process.Start();

      // main menu
      Clear();
      WriteLine("Выберите опции (пробел). По завершению нажмите Enter");

      var mainOptions = settings.operations.Keys.ToArray();
      var mainMenu = new Menu(mainOptions);
      var selectedConfigs = mainMenu.Promt(2);

      var opts = selectedConfigs.Select(x => settings.operations[x]).ToList();
      foreach (var opt in opts)
      {
        //opt.DynamicInvoke(null);
        opt(null);
      }

      var sessionConfigurations = new Configurations()
      {
        profile = "1",
        configurations = settings.sessionConfigs
      };
      string json = JsonConvert.SerializeObject(sessionConfigurations);

      File.WriteAllText(settings.configFile, json);




      process.StandardInput.WriteLine("ipconfig");
      process.StandardInput.Flush();
      process.StandardInput.Close();
      WriteLine(process.StandardOutput.ReadToEnd());
      ReadKey();

      process.WaitForExit();
    }
  }
}

