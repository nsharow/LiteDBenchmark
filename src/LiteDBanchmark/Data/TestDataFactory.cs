using System;

namespace LiteDBenchmark.Data
{
  internal class TestDataFactory : ITestDataFactory
  {
    private readonly int hashSize;
    private readonly string format;
    public TestDataFactory(int hashSize)
    {
      this.hashSize = 
        hashSize > 0
        ? hashSize
        : throw new ArgumentException("Hash size should be a positive number");
      format = $"{{0:d{hashSize}}}";
    }

    public TestData Create(int id)
    {
      /*      byte[] hash = new byte[hashSize];
            byte[] source = BitConverter.GetBytes(id);
            Array.Copy(source, hash, source.Length);*/
      return new TestData()
      {
        Id = id,
        Hash = string.Format(format, id)
      };
    }
  }
}
