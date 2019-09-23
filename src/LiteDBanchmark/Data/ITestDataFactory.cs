using LiteDBanchmark.Data;

namespace LiteDBenchmark.Data
{
  internal interface ITestDataFactory
  {
    TestData Create(int id);
  }
}
