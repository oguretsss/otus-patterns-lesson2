using MatrixMultiplier;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
  internal class TestMultitaskMatrixMultiplier
  {
    /// <summary>
    /// Create matrices a and b, check they were created correctly.
    /// Multiply a by b using MultiTaskMatrixMultiplier, check result
    /// Multiply b by a using MultiTaskMatrixMultiplier, check result, it should be different
    /// </summary>
    [Test]
    public void TestMultiplyMatrices()
    {
      Matrix a = new Matrix(2, 2);
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      Matrix b = new Matrix(2, 2);
      b[0, 0] = 2;
      b[0, 1] = 0;
      b[1, 0] = 5;
      b[1, 1] = -1;

      Assert.AreEqual("1\t2\n3\t4\n", a.ToString());
      Assert.AreEqual("2\t0\n5\t-1\n", b.ToString());

      Matrix res = new MultiTaskMatrixMultiplier(4).MultiplyMatrices(a, b);
      Assert.AreEqual("12\t-2\n26\t-4\n", res.ToString());

      res = new MultiTaskMatrixMultiplier(4).MultiplyMatrices(b, a);
      Assert.AreEqual("2\t4\n2\t6\n", res.ToString());
    }

    /// <summary>
    /// Create matrices a and b that cannot be multiplied (b.Cols != a.Rows), check they were created correctly.
    /// Try to multiply a by b, expect to catch ArgumentException
    /// Try to multiply b by a, expect to catch ArgumentException
    /// </summary>
    [Test]
    public void TestMultiplyMatricesError()
    {
      Matrix a = new Matrix(2, 2);
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      Matrix b = new Matrix(2, 3);
      b[0, 0] = 2;
      b[0, 1] = 0;
      b[1, 0] = 5;
      b[1, 1] = -1;

      Assert.AreEqual("1\t2\n3\t4\n", a.ToString());
      Assert.AreEqual("2\t0\t0\n5\t-1\t0\n", b.ToString());

      Assert.Throws<ArgumentException>(() => new MultiTaskMatrixMultiplier(4).MultiplyMatrices(a, b));
      Assert.Throws<ArgumentException>(() => new MultiTaskMatrixMultiplier(4).MultiplyMatrices(b, a));
    }
  }
}