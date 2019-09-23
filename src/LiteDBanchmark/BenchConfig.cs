using LiteDBenchmark.Data;

namespace LiteDBenchmark
{
  internal class BenchConfig
  {
    public readonly int BatchSize;
    public readonly string TestDbFile;

    public ITestDataFactory TestDataFactory { get; }
    public static BenchConfig Instance { get; private set; }
    
    public static void Init(ITestDataFactory testDataFactory)
    {
      Instance = new BenchConfig(testDataFactory);
    }

    private BenchConfig(ITestDataFactory testDataFactory)
    {
      TestDataFactory = testDataFactory;
      BatchSize = 100000;
      TestDbFile = "test_data.db";
    }
  }
}
