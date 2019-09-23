using BenchmarkDotNet.Running;
using System;

namespace LiteDBenchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      
      new Data.TestDBInitializer(30000000).Init();
      

  //    var summary = BenchmarkRunner.Run<Benchmarks.DBenchmark>();
    }
  }
}
