using System;
using LiteDBanchmark.Data;

namespace LiteDBenchmark.Data
{
  internal class TestDataFactory : ITestDataFactory
  {
    private readonly int hashSize;
    public TestDataFactory(int hashSize)
    {
      this.hashSize = 
        (hashSize == 16 || hashSize == 32)
        ? hashSize
        : throw new ArgumentException("Hash size should be 16 or 32");
    }

    public TestData Create(int id)
    {
      byte[] hash = new byte[hashSize];
      byte[] source = BitConverter.GetBytes(id);
      Array.Copy(source, hash, source.Length);
      return new TestData()
      {
        Id = id,
        Hash = hash
      };
    }
  }
}
