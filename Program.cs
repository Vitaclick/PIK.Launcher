using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PikCorrector
{
  class Program
  {

    static void Main(string[] args)
    {
      var userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var pluginsConfigPaths = new List<string>() {
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2017\PIK\PIK_PluginConfig.xml"),
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2019\PIK\PIK_PluginConfig.xml")
      };

      var pluginsToRemove = new List<string>
      {
        //"ChangeManager",
        "RevitNameValidator",
        "OkCommand"
      };

      void removePlugin(XDocument doc, string keyValue)
      {
        doc.Descendants("PluginInfo")
          .Where(x => x.Element("AssemblyName").Value == keyValue)
          .Remove();
      }

      if (args.Length > 0) {

        if (args[0] == "1") {
          pluginsToRemove.AddRange(new List<string> {
          //"FamilyManager",
          //"FamilyExplorer",
          "BimInspector.Revit",
          "InspectorConfig"
        });
        }
      }

      foreach (var pluginsConfigPath in pluginsConfigPaths) {
        XDocument xmlDoc = XDocument.Load(pluginsConfigPath);

        foreach (var r in pluginsToRemove) {
          removePlugin(xmlDoc, r);
        }

        xmlDoc.Save(pluginsConfigPath);

      }

      // delete unnesesery folders
      var deleters = new List<string> {
        @"C:\Autodesk\AutoCAD\Pik\Settings\Dll",
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2017\PIK\OtherAddins"),
        Path.Combine(userPath, @"Autodesk\Revit\Addins\2019\PIK\OtherAddins"),
        @"C:\ProgramData\Autodesk\ApplicationPlugins\VitroPlugin.bundle"
      };

      // add vitrocad folder
      // if (userPath.Contains("malozyomovvv")){
      //   deleters.Add("vitro path...");
      // }

      foreach (var path in deleters) {
        if (Directory.Exists(path))
          try {
            Directory.Delete(path, true);
          }
          catch (Exception ex) {
            Debug.Write(ex.Message);
          }
      }
    }
  }
}
