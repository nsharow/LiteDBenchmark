using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LiteDBenchmark.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiteDBenchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      var cp = ParseCommandArgs(args);
      if (cp.ShowHelp)
        ShowHelp();
      else
      {
        try
        {
          new TestDBInitializer(cp.InitDbSize, new TestDataFactory(cp.HashSize)).Init();
          if (Debugger.IsAttached)
            BenchmarkRunner.Run<Benchmarks.DBenchmark>(new DebugInProcessConfig());
          else 
            BenchmarkRunner.Run<Benchmarks.DBenchmark>(new Benchmarks.DBenchmark.Config());
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
      Console.WriteLine("Press any key for exit");
      Console.ReadKey();
    }

    static ( int InitDbSize, int HashSize, bool ShowHelp ) ParseCommandArgs(string[] args)
    {
      var result = (InitDbSize: 30000000, HashSize: 42, ShowHelp: false);
      var it = (args as IEnumerable<string>).GetEnumerator();
      while (it.MoveNext())
      {
        string par = it.Current.ToLower().Trim();
        switch (par)
        {
          case "--dbsize":
            {
              if (it.MoveNext())
                int.TryParse(it.Current, out result.InitDbSize);
              break;
            }
          case "--hashsize":
            {
              if (it.MoveNext())
                int.TryParse(it.Current, out result.HashSize);
              break;
            }
          case string arg when arg == "--help"
          || arg == "/?"
          || arg == "?":
            {
              result.ShowHelp = true;
              break;
            }
        }
      }
      return result;
    }

    static void ShowHelp()
    {
      StringBuilder sb = new StringBuilder()
        .AppendLine("Command: dotnet LiteDBenchmark.dll [paremeters]")
        .AppendLine("Parameters:")
        .AppendLine("\t--dbsize <size>")
        .AppendLine("\t--hashsize <size>")
        .Append("\t--help | /? | ?");
    }
  }
}
