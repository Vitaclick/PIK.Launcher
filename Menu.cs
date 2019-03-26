using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PikCorrector
{
  //  logic for selecting specific option
  public class Menu
  {
    public IReadOnlyList<string> Items { get; }
    public int SelectedIndex { get; private set; } = -1; // nothing selected
    public string SelectedOption => SelectedIndex != -1 ? Items[SelectedIndex] : null;
    public void MoveUp() => SelectedIndex = Math.Max(SelectedIndex - 1, 0);
    public void MoveDown() => SelectedIndex = Math.Min(SelectedIndex + 1, Items.Count - 1);
    public Menu(IEnumerable<string> items)
    {
      Items = items.ToArray();
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
        System.Console.WriteLine(menu.Items[i]);
      }
    }
  }
}
