using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDBenchmark.Data
{
  internal sealed class TestDBInitializer
  {
    private readonly int totalSize;
    private readonly ITestDataFactory testFactory;
    private int currentId;

    public TestDBInitializer(int totalSize, ITestDataFactory testDataFactory)
    {
      this.totalSize = totalSize;
      testFactory = testDataFactory;
    }

    public void Init()
    {
      Console.WriteLine("{0} Database initialization started", DateTime.Now);
      using (var db = new LiteDatabase($"filename={BenchConfig.TestDbFile}; journal=false"))
      {
        var testCol = db.GetCollection<TestData>();
//        testCol.EnsureIndex("Hash", true);
        var max = testCol.Count();
        currentId = max;
        while (currentId < totalSize)
        {
          int batchSize = (totalSize - currentId) >= BenchConfig.BatchSize
            ? BenchConfig.BatchSize : (totalSize - currentId);
          var batch = CreateBatch(batchSize).ToArray();
          testCol.Insert(batch);
          Console.Write($"\x000D{currentId} documents from {totalSize} has been created");
        }
      }
      Console.WriteLine("\n{0} Database initialization finished. Total documents: {1}", DateTime.Now, currentId);
    }

    private IEnumerable<TestData> CreateBatch(int size)
    {
      return Enumerable.Range(1, size)
        .Select(_ => testFactory.Create(++currentId)).OrderBy(td => td.Id);
    }
  }
}
