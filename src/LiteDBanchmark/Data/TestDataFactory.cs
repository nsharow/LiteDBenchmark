using System;
using System.Text;

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
    /*
        public TestData Create(int id)
        {
          return new TestData()
          {
            Id = id,
            Hash = string.Format(format, id)
          };
        }
    */
    public TestData Create(int id)
    {
      var hash = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format(format, id)));

      // step 2, convert byte array to hex string
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
      {
        sb.Append(hash[i].ToString("X2"));
      }
      return new TestData()
      {
        Id = id,
        Hash = sb.ToString()
      };
    }
}
}
