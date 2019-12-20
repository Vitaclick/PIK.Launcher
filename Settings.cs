using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Lib.Extensions;
using Server.Lib.Utils;
using static System.Console;

namespace PIK.Launcher
{
  public class Settings
  {
    public string configFile;
    private readonly string userAppdataFolderPath;
    public List<Config> sessionConfigs { get; set; } = new List<Config>();
    public readonly Dictionary<string, Action<List<dynamic>>> operations;

    public Settings()
    {
      configFile = Environment.GetEnvironmentVariable("appdata") + @"\launcherSettings.json";

      userAppdataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      operations = new Dictionary<string, Action<List<dynamic>>>
      {
        {"Установить плагины ПИК", InstallPik},
        {"Отменить звонки Вильчинской", RemoveFileCop },
        {"Удалить прочие приблуды", RemoveOtherAddins },
        {"Удалить плагины для Автокада", RemoveACadPlugins },
        {"Установить ПИК-Координацию", InstallAssembler },
        {"Установить Weandrevit", InstallWeandrevit },
      };
    }

    public void Install(List<Config> configs)
    {
      foreach (var config in configs)
      {
        if (!operations.ContainsKey(config.name))
        {
          throw new ArgumentException("Настройка не найдена");
        }
        operations[config.name].DynamicInvoke(config.args);
      }
      WriteLine("Установка завершена. Нажмите что-нибудь чтобы выйти...");
      ReadKey();
      Environment.Exit(0);
    }

    public void Install(List<string> configs)
    {
      foreach (var config in configs)
      {
        if (!operations.ContainsKey(config))
        {
          throw new ArgumentException("Настройка не найдена");
        }
        operations[config](null);
      }
    }

    public void SaveSettings()
    {
      Clear();
      //WriteLine("Введите имя профиля:");
      //var profileName = ReadLine();
      var sessionConfigurations = new Configurations()
      {
        profile = "default",
        configurations = sessionConfigs
      };
      string json = JsonConvert.SerializeObject(sessionConfigurations);

      File.WriteAllText(configFile, json);
    }

    private void InstallPik(List<dynamic> args)
    {
      var config = new Config()
      {
        name = "InstallPik",
        args = new List<dynamic>()
      };
      var excludedPlugins = new List<string>() { };
      if (args == null)
      {
        WriteLine("Выберите (пробел) для отдельного удаления плагинов ПИК:");
        var removablePlugins = new[]
        {
          "OkCommand",
          "BimInspector.Revit",
          "FamilyManager",
          "FamilyExplorer",
          "Revit_PlanDimensions",
          "Revit_CutLinesPlacer",
          "Revit_Rooms"
        };

        excludedPlugins = new Menu(removablePlugins).Prompt(2)
          as List<string>;


        // Adding arguments
        config.args = new List<dynamic>()
        {
          excludedPlugins
        };
      }
      else
      {
        foreach (var a in args)
        {
          var t = a.GetType();
          switch (a)
          {
            case JArray lstStr:
              var stri = lstStr.ToObject<List<string>>();
              excludedPlugins = stri;
              config.args.Add(stri);
              break;
            case bool b:
              config.args.Add(b);
              break;
          }
        }
      }

      sessionConfigs.Add(config);

      var pluginsConfigPaths = new List<string>() {
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\PIK_PluginConfig.xml"),
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\PIK_PluginConfig.xml")
      };
      if (Debugger.IsAttached)
        pluginsConfigPaths = new List<string>() { "PIK_PluginConfig.xml" };

      void removePlugin(XDocument doc, string keyValue)
      {
        doc.Descendants("PluginInfo")
          .Where(x => x.Element("AssemblyName")?.Value == keyValue)
          .Remove();
      }

      foreach (var pluginsConfigPath in pluginsConfigPaths)
      {
        if (File.Exists(pluginsConfigPath))
        {
          XDocument xmlDoc = XDocument.Load(pluginsConfigPath);
          if (excludedPlugins != null)
            foreach (string plugin in excludedPlugins)
            {
              removePlugin(xmlDoc, plugin);
            }
          xmlDoc.Save(pluginsConfigPath);
        }
        else
        {
          // TODO: при работе в одной версии например 2017 может не быть 2019 .xml файла
          throw new Exception("Конфигурационных файлов ПИК плагинов не существует");
        }
      }
    }

    private void InstallAssembler(List<object> args)
    {
      Directories.CopyWholeDirectory(
        @"\\picompany.ru\pikp\Dep\IT\_SR_Public\01_BIM\10_Development\Revit.Assembler\2017",
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017"));

      Directories.CopyWholeDirectory(
        @"\\picompany.ru\pikp\Dep\IT\_SR_Public\01_BIM\10_Development\Revit.Assembler\2019",
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019"));
    }

    private void InstallWeandrevit(List<object> args)
    {
      Directories.CopyWholeDirectory(
        @"\\picompany.ru\pikp\lib\09_Программы\WeandrevitGates\WeandrevitGates",
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017"));

      Directories.CopyWholeDirectory(
        @"\\picompany.ru\pikp\lib\09_Программы\WeandrevitGates\WeandrevitGates_2019",
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019"));
    }

    private async void RemoveFileCop(List<object> args)
    {
      await Directories.RemoveDirectoriesAsync("C:\\ProgramData\\Autodesk\\ApplicationPlugins\\FileCop.bundle" );
    }

    private async void RemoveACadPlugins(List<object> args)
    {
      await Directories.RemoveDirectoriesAsync("C:\\Autodesk");
    }

    private async void RemoveOtherAddins(List<object> args)
    {
      var otherListAddins = new[] { "AlignTag", "Ostec" };
      var removeDirs = new []
      {
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\OtherAddins"),
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\OtherAddins"),
        @"C:\ProgramData\Autodesk\ApplicationPlugins\VitroPlugin.bundle",
        @"C:\Autodesk\AutoCAD\Pik\Settings\Dll"
      };

      var addinsPath = Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins");
      var otherPluginsAddinsFiles = Directory.GetFiles(addinsPath, "*.addin", SearchOption.AllDirectories);
      foreach (var addinFile in otherPluginsAddinsFiles)
      {
        if (addinFile.ContainsAny(otherListAddins))
        {
          File.Delete(addinFile);
        }
      }

      await Directories.RemoveDirectoriesAsync(removeDirs);
    }
  }
}
