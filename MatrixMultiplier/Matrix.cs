using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixMultiplier
{
  public class Matrix
  {
    private double[,] data;

    public int Cols { get; }
    public int Rows { get; }

    public Matrix(int rows, int cols)
    {
      this.Rows = rows;
      this.Cols = cols;
      this.data = new double[rows, cols];
    }

    ///<summary>
    ///Loop through all matrix elements and perform callback <paramref name="func"/> on each of them
    ///</summary>
    public void ProcessFunctionOverData(Action<int, int> func)
    {
      for (var i = 0; i < this.Rows; i++)
      {
        for (var j = 0; j < this.Cols; j++)
        {
          func(i, j);
        }
      }
    }

    public double this[int x, int y]
    {
      get
      {
        return this.data[x, y];
      }
      set
      {
        this.data[x, y] = value;
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      ProcessFunctionOverData((i, j) => sb.Append(data[i, j].ToString() + (j == (Cols - 1) ? "\n" : "\t")));
      return sb.ToString();
    }
  }
}