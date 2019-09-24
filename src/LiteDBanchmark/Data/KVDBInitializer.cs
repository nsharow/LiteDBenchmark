using LightningDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiteDBenchmark.Data
{
  internal sealed class KVDBInitializer
  {
    private readonly int totalSize;
    private readonly ITestDataFactory testFactory;
    private int currentId;
    
    public KVDBInitializer(int totalSize, ITestDataFactory testDataFactory)
    {
      this.totalSize = totalSize;
      testFactory = testDataFactory;
    }

    public void Init()
    {
      Console.WriteLine("{0} KV database initialization started", DateTime.Now);
      string dataCatalog = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test_data");
      Directory.CreateDirectory(dataCatalog);
      using (var env = new LightningEnvironment(dataCatalog, new EnvironmentConfiguration()
      {
        MapSize = 4294967296
      }))
      {
        env.MaxDatabases = 4;
        env.Open();
        while (currentId < totalSize)
        {
          using (var tx = env.BeginTransaction())
          using (var db = tx.OpenDatabase("test", new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create }))
          {
            currentId = (int)tx.GetEntriesCount(db);
            if (currentId >= totalSize) break;
            int batchSize = (totalSize - currentId) >= BenchConfig.BatchSize
                ? BenchConfig.BatchSize : (totalSize - currentId);

            var batch = CreateBatch(batchSize);
            foreach (var d in batch)
            {
              tx.Put(db, Encoding.UTF8.GetBytes(d.Id), Encoding.UTF8.GetBytes(d.Data));
            }
            tx.Commit();
            Console.Write($"\x000D{currentId} documents from {totalSize} has been created");
          }
          env.Flush(false);
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
