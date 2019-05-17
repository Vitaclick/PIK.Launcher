using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PIK.Launcher
{
  public class Settings
  {
    public string configFile;
    private readonly string userAppdataFolderPath;
    public List<string> sessionConfigs { get; set; } = new List<string>();
    public readonly Dictionary<string, ConfigHandler> operations;
    private List<string> log;
    public delegate int ConfigHandler(Dictionary<string, object> args);
    public Settings()
    {
      configFile = Environment.GetEnvironmentVariable("appdata") + @"\launcherSettings.json";
      userAppdataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      operations = new Dictionary<string, ConfigHandler>
      {
        {"RemoveDirectories", RemoveDirectories},
        {"ClearAllPlugins", ClearAllPlugins},
        {"InstallPik", InstallPik},
        {"InstallAssembler", InstallAssembler},
        {"InstallWeAndRevit", InstallWeAndRevit}
      };
    }
    public int Install(string name, Dictionary<string, object> args)
    {
      if (!operations.ContainsKey(name))
      {
        System.Console.WriteLine($"command: {name} not found");
        return -1;
      }
      return operations[name](args);
    }
    private int RemoveDirectories(Dictionary<string, object> args)
    {
      dynamic d = args["prop1"];
      var directories = d.ToObject<List<string>>();

      foreach (var directory in directories)
      {
        if (Directory.Exists(directory))
        {
          try
          {
            Directory.Delete(directory, true);
            log.Add($"Папка {directory} удалена");
          }
          catch (Exception ex)
          {
            log.Add($"Ошибка при удалении папки {directory}:\n{ex.Message}");
            Debug.Write(ex.Message);
            continue;
          }
        }
      }
      return 100;
    }
    private int ClearAllPlugins(Dictionary<string, object> args)
    {
      return 100;
    }
    private int InstallPik(Dictionary<string, dynamic> args)
    {
      List<string> excluded = args["prop1"].ToObject<List<string>>();
      bool removeOtherAddins = args["prop2"];

      // TODO: can thror an exception if no such file
      var pluginsConfigPaths = new List<string>() {
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\PIK_PluginConfig.xml"),
        Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\PIK_PluginConfig.xml")
      };
      // logic for excluding PIK plugins
      void removePlugin(XDocument doc, string keyValue)
      {
        doc.Descendants("PluginInfo")
          .Where(x => x.Element("AssemblyName").Value == keyValue)
          .Remove();
      }

      if (excluded.Count > 0)
      {
        foreach (var pluginsConfigPath in pluginsConfigPaths)
        {
          XDocument xmlDoc = XDocument.Load(pluginsConfigPath);
          foreach (var plugin in excluded)
          {
            removePlugin(xmlDoc, plugin);
          }
          xmlDoc.Save(pluginsConfigPath);

        }
      }
      if (removeOtherAddins)
      {
        RemoveDirectories(new Dictionary<string, object>(){
          {
            "prop1",
            new List<string>{
            Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2017\PIK\OtherAddins"),
            Path.Combine(userAppdataFolderPath, @"Autodesk\Revit\Addins\2019\PIK\OtherAddins"),
            @"C:\ProgramData\Autodesk\ApplicationPlugins\VitroPlugin.bundle",
            @"C:\Autodesk\AutoCAD\Pik\Settings\Dll"
            }
          }
        });
      }
      return 100;
    }
    private int InstallAssembler(Dictionary<string, object> args)
    {
      return 100;
    }
    private int InstallWeAndRevit(Dictionary<string, object> args)
    {
      return 100;
    }
  }

}
