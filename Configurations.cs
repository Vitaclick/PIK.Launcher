using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PIK.Launcher
{
  public static class Configurations
  {
    private static readonly string userPath;
    public static List<string> log;
    static Configurations()
    {
      userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
    public static void RemoveDirectories(List<string> directories)
    {
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
    }

    public static void ClearAllPlugins()
    {

    }
    public static void InstallPik()
    {

    }
    public static void InstallPik(List<string> excluded, bool removeOtherAddins)
    {
      // TODO: can thror an exception if no such file
      var pluginsConfigPaths = new List<string>() {
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2017\PIK\PIK_PluginConfig.xml"),
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2019\PIK\PIK_PluginConfig.xml")
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
        RemoveDirectories(new List<string>{
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2017\PIK\OtherAddins"),
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2019\PIK\OtherAddins"),
        @"C:\ProgramData\Autodesk\ApplicationPlugins\VitroPlugin.bundle",
        @"C:\Autodesk\AutoCAD\Pik\Settings\Dll"
        });
      }
    }
    public static void InstallAssembler()
    {

    }
    public static void InstallWeAndRevit()
    {

    }
  }
}
