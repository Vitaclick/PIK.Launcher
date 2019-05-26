using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace PIK.Launcher.Test
{
  public class UnitTest1
  {
    [Fact]
    public void WriteDummyDataToJson()
    {
      var data = new List<Config>()
      {
        new Config()
        {
          name = "Pik",
          args = new List<dynamic>()
          {
            "Hello", "World"
          }
        },
        new Config()
        {
          name = "Puk",
          args = new List<dynamic>()
          {
            "Hello2", "World2", false
          }
        }
      };
      var serialized = JsonConvert.SerializeObject(data);
      File.WriteAllText("kek.json", serialized);
    }
    [Fact]
    public void ReadDummyDataFromJson()
    {
      var data = JsonConvert.DeserializeObject<List<Config>>(File.ReadAllText("kek.json"));
      foreach (var config in data)
      {
        var resu = config.args[2].GetType();
      }

    }

    [Fact]
    public void InstallationTest()
    {
      var configs = new List<Config>
      {
        new Config()
        {
          name = "InstallPik",
          args = new List<dynamic>
          {
            "lol",
            "kek"
          }
        },
        new Config()
        {
          name = "DestroyPlanet",
          args = new List<dynamic>
          {
            "lol",
            "kek"
          }
        }
      };

      var set = new Settings();
      set.Install(configs);
    }
  }
}
