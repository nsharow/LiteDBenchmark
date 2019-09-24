using LightningDB;
using LiteDBenchmark.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiteDBenchmark.Benchmarks
{
  public class KVDBenchmark : IDisposable
  {
    private int currentId;
    private LightningEnvironment env;
    private readonly ITestDataFactory dataFactory;

    public KVDBenchmark(ITestDataFactory testDataFactory)
    {
      string dataCatalog = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test_data");
      Directory.CreateDirectory(dataCatalog);
      env = new LightningEnvironment(dataCatalog, new EnvironmentConfiguration()
      {
        MapSize = 4294967296
      });
      env.MaxDatabases = 4;
      env.Open();
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        currentId = (int)tx.GetEntriesCount(db);
      }
      dataFactory = testDataFactory;
    }

    ~KVDBenchmark()
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
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var batch = CreateBatch(1);
        foreach (var td in batch)
        {
          tx.Put(db, Encoding.UTF8.GetBytes(td.Id), Encoding.UTF8.GetBytes(td.Data));
        }
        tx.Commit();
      }     
    }

    public void Insert10()
    {
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var batch = CreateBatch(10);
        foreach (var td in batch)
        {
          tx.Put(db, Encoding.UTF8.GetBytes(td.Id), Encoding.UTF8.GetBytes(td.Data));
        }
        tx.Commit();
      }
    }

    public void Insert100()
    {
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var batch = CreateBatch(100);
        foreach (var td in batch)
        {
          tx.Put(db, Encoding.UTF8.GetBytes(td.Id), Encoding.UTF8.GetBytes(td.Data));
        }
        tx.Commit();
      }
    }

    public void SelectByHashFromBegin()
    {
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var testData = dataFactory.Create(1);
        var data = Encoding.UTF8.GetString(tx.Get(db, Encoding.UTF8.GetBytes(testData.Id)));
        if (!data.Equals(testData.Data))
            throw new InvalidOperationException($"Document #1 is wrong");
      }     
    }

    public void SelectByHashFromMiddle()
    {
      int testId = currentId / 2;
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var testData = dataFactory.Create(testId);
        var data = Encoding.UTF8.GetString(tx.Get(db, Encoding.UTF8.GetBytes(testData.Id)));
        if (!data.Equals(testData.Data))
          throw new InvalidOperationException($"Document #{testId} is wrong");
      }
    }

    public void SelectByHashFromEnd()
    {
      int testId = currentId;
      using (var tx = env.BeginTransaction())
      using (var db = tx.OpenDatabase("test"))
      {
        var testData = dataFactory.Create(testId);
        var data = Encoding.UTF8.GetString(tx.Get(db, Encoding.UTF8.GetBytes(testData.Id)));
        if (!data.Equals(testData.Data))
          throw new InvalidOperationException($"Document #{testId} is wrong");
      }
    }

    public void Dispose()
    {
      env?.Dispose();
      env = null;
    }

    private IEnumerable<TestData> CreateBatch(int size)
    {
      return Enumerable.Range(1, size)
        .Select(_ => dataFactory.Create(++currentId));
    }
  }
}
