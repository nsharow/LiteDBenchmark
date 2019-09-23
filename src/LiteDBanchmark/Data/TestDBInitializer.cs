using LiteDB;
using LiteDBanchmark.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDBenchmark.Data
{
  internal sealed class TestDBInitializer
  {
    private int totalSize;
    private int currentId;

    public TestDBInitializer(int totalSize)
    {
      this.totalSize = totalSize;
    }

    public void Init()
    {
      Console.WriteLine("{0} Database initialization started", DateTime.Now);
      using (var db = new LiteDatabase($"filename={Constants.TestDbFile}; journal=false"))
      {
        var testCol = db.GetCollection<TestData>();
        testCol.EnsureIndex("Hash", true);
        currentId = testCol.Count() > 0
          ? testCol.Max().AsInt32 : 0;
        while (currentId < totalSize)
        {
          int batchSize = (totalSize - currentId) >= Constants.BatchSize
            ? Constants.BatchSize : (totalSize - currentId);
          var batch = CreateBatch(batchSize).ToArray();
          testCol.Insert(batch);
          Console.Write($"\x000D{currentId} documents from {totalSize} are created");
        }
      }
      Console.WriteLine("\n{0} Database initialization finished. Total documents: {1}", DateTime.Now, currentId);
    }

    private IEnumerable<TestData> CreateBatch(int size)
    {
      return Enumerable.Range(1, size)
        .Select(_ => TestData.Create(++currentId));
    }
  }
}
