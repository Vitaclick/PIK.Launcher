using System.Collections.Generic;

namespace PIK.Launcher
{
  public class Configurations
  {
    public List<Config> configurations { get; set; }
    public string profile { get; set; }
  }
  public class Config
  {
    public string name { get; set; }
    public List<dynamic> args { get; set; }
  }
}
