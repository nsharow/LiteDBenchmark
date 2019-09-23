using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using LiteDB;
using LiteDBanchmark.Data;
using LiteDBenchmark.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDBenchmark.Benchmarks
{
  public class DBenchmark : IDisposable
  {
    public class Config : ManualConfig
    {
      public Config()
      {
        Add(Job.Dry);
        Add(ConsoleLogger.Default);
        Add(TargetMethodColumn.Method, StatisticColumn.Min, StatisticColumn.Median, StatisticColumn.Max);
        Add(RPlotExporter.Default, CsvExporter.Default);
        Add(EnvironmentAnalyser.Default);
        UnionRule = ConfigUnionRule.AlwaysUseLocal;        
      }
    }

    private int currentId;
    private LiteDatabase db;

    public DBenchmark()
    {
      db = new LiteDatabase($"filename={BenchConfig.TestDbFile}");
      var testCol = db.GetCollection<TestData>();
      var max = testCol.Max()?.AsInt32 ?? 0;
      currentId = max;
    }

    [Benchmark]
    public void Insert1()
    {
      db.GetCollection<TestData>()
        .Insert(
        CreateBatch(1));
    }

    [Benchmark]
    public void Insert10()
    {
      db.GetCollection<TestData>()
        .Insert(CreateBatch(10));
    }

    [Benchmark]
    public void Insert100()
    {
      db.GetCollection<TestData>()
        .Insert(CreateBatch(100));
    }

    [Benchmark]
    public TestData SelectByHashFromBegin()
    {
      var testData = new TestDataFactory(32).Create(1);
      return db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #1 is not found");
    }

    [Benchmark]
    public TestData SelectByHashFromMiddle()
    {
      int testId = currentId / 2;
      var testData = new TestDataFactory(32).Create(testId);
      return db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #{testId} is not found");
    }

    [Benchmark]
    public TestData SelectByHashFromEnd()
    {
      var testData = new TestDataFactory(32).Create(currentId);
      return db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #{currentId} is not found");
    }

    public void Dispose()
    {
      db?.Dispose();
      db = null;
    }

    private IEnumerable<TestData> CreateBatch(int size)
    {
      var dataFactory = new TestDataFactory(32);
      return Enumerable.Range(1, size)
        .Select(_ => dataFactory.Create(++currentId));
    }
  }
}
