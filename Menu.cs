using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static PIK.Launcher.Settings;

namespace PikCorrector
{

  // logic for selecting specific option
  public class Menu
  {
    public IReadOnlyList<string> Items { get; }
    public List<string> SelectedItems { get; set; } = new List<string>();
    public int SelectedIndex { get; private set; } = -1; // nothing selected
    public string SelectedOption => SelectedIndex != -1 ? Items[SelectedIndex] : null;
    public void MoveUp() => SelectedIndex = Math.Max(SelectedIndex - 1, 0);
    public void MoveDown() => SelectedIndex = Math.Min(SelectedIndex + 1, Items.Count - 1);
    public void Select()
    {
      var selection = Items[SelectedIndex];
      if (!SelectedItems.Contains(selection))
        SelectedItems.Add(selection);
      else
        SelectedItems.Remove(selection);
    }
    public Menu(Dictionary<string, ConfigHandler> items)
    {
      Items = items.Keys.ToArray();
    }
    public Menu(string[] items)
    {
      Items = items.ToArray();
    }
    public ICollection<string> Promt(int verticalOffset)
    {
      var menuPainter = new MenuPainter(this);

      bool done = false;
      do
      {
        menuPainter.Paint(1, verticalOffset);
        var keyInfo = Console.ReadKey();

        switch (keyInfo.Key)
        {
          case ConsoleKey.UpArrow: MoveUp(); break;
          case ConsoleKey.DownArrow: MoveDown(); break;
          case ConsoleKey.Spacebar: Select(); break;
          case ConsoleKey.Enter: done = true; break;
        }
      }
      while (!done);

      Debug.WriteLine("Выбранная(ые) опция: " + (string.Join(",", SelectedItems) ?? "(nothing)"));

      return SelectedItems;
    }
  }
  // drawing menu list
  public class MenuPainter
  {
    readonly Menu menu;
    public MenuPainter(Menu menu)
    {
      this.menu = menu;
    }
    public void Paint(int x, int y)
    {
      for (int i = 0; i < menu.Items.Count; i++)
      {
        Console.SetCursorPosition(x, y + i);
        var color = menu.SelectedIndex == i ? ConsoleColor.Yellow : ConsoleColor.Gray;

        Console.ForegroundColor = color;
        var item = menu.Items[i];
        System.Console.WriteLine(menu.SelectedItems.Contains(item) ? "[x] " : "[ ] " + menu.Items[i]);
      }
    }
  }
}
