using LiteDB;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LiteDBanchmark.Data
{
  public sealed class TestData
  {
    public int Id { get; set; }
    public string Hash { get; set; }

    public static TestData Create(int id)
    {
//      using (var alg = SHA256.Create())
      {
        var result = new TestData()
        {
          Id = id,
          //          Hash = alg.ComputeHash(BitConverter.GetBytes(id))
          Hash = string.Format("{0:d64}", id)
        };
        return result;
      }        
    }

  }
}
