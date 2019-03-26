using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static System.Console;

namespace PikCorrector
{
  class Program
  {
    static int index = 0;
    static void Main(string[] args)
    {
      var process = new Process();

      process.StartInfo.FileName = "cmd.exe";
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardInput = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.Start();
      Console.OutputEncoding = Encoding.UTF8;
      if (Debugger.IsAttached || args[0] == "-debug")
        System.Console.WriteLine("Launcher is starting...");

      var menu = new Menu(new string[] {
        "Да",
        "Нет",
        "Отмена"
      });

      var menuPainter = new MenuPainter(menu);

      bool done = false;

      // Console.CursorVisible = false;
      do
      {
        System.Console.WriteLine("Настроить Ревит?");
        menuPainter.Paint(1, 3);
        var keyInfo = Console.ReadKey();

        switch (keyInfo.Key)
        {
          case ConsoleKey.UpArrow: menu.MoveUp(); break;
          case ConsoleKey.DownArrow: menu.MoveDown(); break;
          case ConsoleKey.Enter: done = true; break;
        }
      }
      while (!done);

      Console.ForegroundColor = ConsoleColor.Cyan;
      System.Console.WriteLine("Выбранная опция: " + (menu.SelectedOption ?? "(nothing)"));

      process.StandardInput.WriteLine("ipconfig");
      process.StandardInput.Flush();
      process.StandardInput.Close();
      System.Console.WriteLine(process.StandardOutput.ReadToEnd());
      Console.ReadKey();




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

      if (args.Length > 0)
      {

        if (args[0] == "1")
        {
          pluginsToRemove.AddRange(new List<string> {
          //"FamilyManager",
          //"FamilyExplorer",
          "BimInspector.Revit",
          "InspectorConfig"
        });
        }
      }

      foreach (var pluginsConfigPath in pluginsConfigPaths)
      {
        XDocument xmlDoc = XDocument.Load(pluginsConfigPath);

        foreach (var r in pluginsToRemove)
        {
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

      foreach (var path in deleters)
      {
        if (Directory.Exists(path))
          try
          {
            Directory.Delete(path, true);
          }
          catch (Exception ex)
          {
            Debug.Write(ex.Message);
          }
      }

      process.WaitForExit();

    }
  }
}

