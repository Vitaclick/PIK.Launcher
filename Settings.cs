using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using PIK.Launcher.Utils;
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

      if (Debugger.IsAttached)
      {
        configFile = "kek.json";
      }
      userAppdataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      operations = new Dictionary<string, Action<List<dynamic>>>
      {
        {"InstallPik", InstallPik},
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
    }

    private void InstallPik(List<dynamic> args)
    {
      var config = new Config() {name = "InstallPik"};
      var excludedPlugins = new List<string>(){};
      bool removeOtherAddins = false;
      if (args == null)
      {
        var removablePlugins = new string[]
        {
          "RevitNameValidator",
          "OkCommand",
          "BimInspector.Revit",
          "InspectorConfig",
          "ChangeManager",
          "FamilyManager",
          "FamilyExplorer"
        };

        excludedPlugins = new Menu(removablePlugins).Promt(2)
          as List<string>;
        WriteLine("\nУдалить другие плагины?");
        removeOtherAddins = new Menu(new[] {"Да", "Нет"})
                              .Promt(removablePlugins.Length + 5)
                              .FirstOrDefault() == "Да";
        // Adding arguments
        config.args = new List<dynamic>()
        {
          excludedPlugins, removeOtherAddins
        };
      }
      else
      {
        foreach (var a in args)
        {
          switch (a)
          {
            case string str:
              excludedPlugins.Add(str);
              break;
            case List<string> lstStr:
              excludedPlugins = lstStr;
              break;
            case bool b:
              removeOtherAddins = b;
              break;
          }
        }
      }

      sessionConfigs.Add(config);

      // TODO: can throw an exception if file not found
      var pluginsConfigPaths = new List<string>() {
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\PIK_PluginConfig.xml"),
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\PIK_PluginConfig.xml")
      };

      void removePlugin(XDocument doc, string keyValue)
      {
        doc.Descendants("PluginInfo")
          .Where(x => x.Element("AssemblyName")?.Value == keyValue)
          .Remove();
      }


      foreach (var pluginsConfigPath in pluginsConfigPaths)
      {
        XDocument xmlDoc = XDocument.Load(pluginsConfigPath);
        if (excludedPlugins != null)
          foreach (string plugin in excludedPlugins)
          {
            removePlugin(xmlDoc, plugin);
          }
        xmlDoc.Save(pluginsConfigPath);
      }

      if (removeOtherAddins)
      {
        Directories.RemoveDirectories(
            new List<string>{
            Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\OtherAddins"),
            Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\OtherAddins"),
            @"C:\ProgramData\Autodesk\ApplicationPlugins\VitroPlugin.bundle",
            @"C:\Autodesk\AutoCAD\Pik\Settings\Dll"
          });
      }
    }
  }
}
