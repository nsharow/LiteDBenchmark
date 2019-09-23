using BenchmarkDotNet.Attributes;
using LiteDB;
using LiteDBanchmark.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDBenchmark.Benchmarks
{
  public class DBenchmark : IDisposable
  {
    private int currentId;
    private LiteDatabase db;

    public DBenchmark()
    {
      db = new LiteDatabase($"filename={Constants.TestDbFile}");
    }

    [Benchmark]
    public void Insert1()
    {
      db.GetCollection<TestData>()
        .Insert(TestData.Create(++currentId));
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
      var testData = TestData.Create(1);
      return db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #1 is not found");
    }

    [Benchmark]
    public TestData SelectByHashFromMiddle()
    {
      int testId = currentId / 2;
      var testData = TestData.Create(testId);
      return db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #{testId} is not found");
    }

    [Benchmark]
    public TestData SelectByHashFromEnd()
    {
      var testData = TestData.Create(currentId);
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
      return Enumerable.Range(1, size)
        .Select(_ => TestData.Create(++currentId));
    }
  }
}
