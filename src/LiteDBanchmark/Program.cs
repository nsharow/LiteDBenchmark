using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;

namespace LiteDBenchmark
{
  class Program
  {
    static void Main(string[] args)
    {      
      new Data.TestDBInitializer(30000000).Init();
#if DEBUG
      var summary = BenchmarkRunner.Run<Benchmarks.DBenchmark>(new DebugInProcessConfig());
#else
      var summary = BenchmarkRunner.Run<Benchmarks.DBenchmark>();
#endif
      Console.ReadKey();
    }
  }
}
