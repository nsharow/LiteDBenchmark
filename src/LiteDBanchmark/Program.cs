﻿using LiteDBenchmark.Data;
using System;
using System.Collections.Generic;
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
          new KVDBInitializer(cp.InitDbSize, new TestDataFactory(cp.HashSize)).Init();
          using (var bench = new Benchmarks.KVDBenchmark(new TestDataFactory(cp.HashSize)))
          {
            bench.Run();
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
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
      Console.WriteLine(sb.ToString());
    }
  }
}
