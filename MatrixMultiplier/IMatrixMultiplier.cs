using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixMultiplier
{
  public interface IMatrixMultiplier
  {
    Matrix MultiplyMatrices(Matrix a, Matrix b);
  }
}