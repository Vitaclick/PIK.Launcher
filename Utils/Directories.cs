using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PIK.Launcher.Utils
{
  public static class Directories
  {
    public static void RemoveDirectories(List<string> args)
    {
      foreach (string directory in args)
      {
        if (Directory.Exists(directory))
        {
          try
          {
            Directory.Delete(directory, true);
            //log.Add($"Папка {directory} удалена");
          }
          catch (Exception ex)
          {
            //log.Add($"Ошибка при удалении папки {directory}:\n{ex.Message}");
            Debug.Write(ex.Message);
          }
        }
      }
    }
  }
}
