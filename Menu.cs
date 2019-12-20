using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace PIK.Launcher
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
      // skip empty selection
      if (SelectedIndex != -1)
      {
        var selection = Items[SelectedIndex];
        if (!SelectedItems.Contains(selection))
          SelectedItems.Add(selection);
        else
          SelectedItems.Remove(selection);
      }
    }
    //public Menu(Dictionary<string, Settings.ConfigHandler> items)
    //{
    //  Items = items.Keys.ToArray();
    //}
    public Menu(string[] items)
    {
      Items = items.ToArray();
    }
    public ICollection<string> Prompt(int verticalOffset)
    {
      var menuPainter = new MenuPainter(this);

      bool done = false;
      do
      {
        menuPainter.Paint(1, verticalOffset);
        var keyInfo = ReadKey();

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

    public static bool PromptYesNo(string message)
    {
      Write($"{message} (y/n):");
      do
      {
        var answer = ReadKey(true);
        switch (answer.Key)
        {
          case ConsoleKey.Y:
            return true;
          case ConsoleKey.N:
            return false;
          case ConsoleKey.Escape:
            return false;
        }
      } while (true);
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
        SetCursorPosition(x, y + i);
        var color = menu.SelectedIndex == i ? ConsoleColor.Yellow : ConsoleColor.Gray;

        ForegroundColor = color;
        var item = menu.Items[i];
        WriteLine(menu.SelectedItems.Contains(item) ? "[x] " : "[ ] " + menu.Items[i]);
      }
    }
  }
}
