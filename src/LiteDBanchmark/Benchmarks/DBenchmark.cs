using LiteDB;
using LiteDBenchmark.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiteDBenchmark.Benchmarks
{
  public class DBenchmark : IDisposable
  {
    private int currentId;
    private LiteDatabase db;
    private readonly ITestDataFactory dataFactory;

    public DBenchmark(ITestDataFactory testDataFactory)
    {
      db = new LiteDatabase($"filename={BenchConfig.TestDbFile}");
      var testCol = db.GetCollection<TestData>();
      var max = testCol.Max()?.AsInt32 ?? 0;
      currentId = max;
      dataFactory = testDataFactory;
    }

    ~DBenchmark()
    {
      Dispose();
    }

    public void Run()
    {
      RunInternal(nameof(Insert1), () => Insert1());
      RunInternal(nameof(Insert10), () => Insert10());
      RunInternal(nameof(Insert100), () => Insert100());
      RunInternal(nameof(SelectByHashFromBegin), () => SelectByHashFromBegin());
      RunInternal(nameof(SelectByHashFromMiddle), () => SelectByHashFromMiddle());
      RunInternal(nameof(SelectByHashFromEnd), () => SelectByHashFromEnd());
    }

    public void RunInternal(string methodName, Action method)
    {
      try
      {
        var sw = Stopwatch.StartNew();
        method();
        sw.Stop();
        Console.WriteLine($"{methodName}: {sw.Elapsed.TotalMilliseconds} ms");
      }
      catch(Exception ex)
      {
        Console.WriteLine($"{methodName} error: {ex}");
      }
      
    }

    public void Insert1()
    {
      db.GetCollection<TestData>()
        .Insert(
        CreateBatch(1));
    }

    public void Insert10()
    {
      db.GetCollection<TestData>()
        .Insert(CreateBatch(10));
    }

    public void Insert100()
    {
      db.GetCollection<TestData>()
        .Insert(CreateBatch(100));
    }

    public void SelectByHashFromBegin()
    {
      var testData = dataFactory.Create(1);
      var resultData = db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #1 is not found");
      if (resultData.Id != 1)
        throw new InvalidOperationException($"Document #1 is wrong");
    }

    public void SelectByHashFromMiddle()
    {
      int testId = currentId / 2;
      var testData = dataFactory.Create(testId);
      var resultData = db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #{testId} is not found");
      if (resultData.Id != testId)
        throw new InvalidOperationException($"Document #{testId} is wrong");
    }

    public void SelectByHashFromEnd()
    {
      var testData = dataFactory.Create(currentId);
      var resultData = db.GetCollection<TestData>()
        .FindOne(td => td.Hash == testData.Hash)
        ?? throw new InvalidOperationException($"Document #{currentId} is not found");
      if (resultData.Id != currentId)
        throw new InvalidOperationException($"Document #{currentId} is wrong");
    }

    public void Dispose()
    {
      db?.Dispose();
      db = null;
    }

    private IEnumerable<TestData> CreateBatch(int size)
    {
      return Enumerable.Range(1, size)
        .Select(_ => dataFactory.Create(++currentId));
    }
  }
}
