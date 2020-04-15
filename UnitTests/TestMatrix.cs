using MatrixMultiplier;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
  internal class TestMatrix
  {
    /// <summary>
    /// Create matrix, check that it was created correctly
    /// Using ProcessFunctionOverData() method, multiply every matrix member by 2, check matrix again
    /// </summary>
    [Test]
    public void TestProcessFunctionOverData()
    {
      Matrix a = new Matrix(2, 2);
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      Assert.AreEqual("1\t2\n3\t4\n", a.ToString());
      a.ProcessFunctionOverData((i, j) => a[i, j] *= 2);
      Assert.AreEqual("2\t4\n6\t8\n", a.ToString());
    }
  }
}