using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
    public Dictionary<string, object> args { get; set; }
  }
}
