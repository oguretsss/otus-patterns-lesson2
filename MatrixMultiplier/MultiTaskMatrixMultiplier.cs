using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiplier
{
  public class MultiTaskMatrixMultiplier : IMatrixMultiplier
  {
    private Matrix result;
    private List<Task> TaskList;
    private Mutex mut;
    private Dictionary<int, int> tasks;

    public MultiTaskMatrixMultiplier(int numThreads)
    {
      ThreadPool.SetMinThreads(numThreads, numThreads);
      ThreadPool.SetMaxThreads(numThreads, numThreads);
      TaskList = new List<Task>();
      mut = new Mutex();
      tasks = new Dictionary<int, int>();
    }

    public Matrix MultiplyMatrices(Matrix a, Matrix b)
    {
      if (a.Rows != b.Cols || a.Cols != b.Rows)
      {
        throw new ArgumentException("Matrices can not be multiplied");
      }
      Console.WriteLine("Multiplying...");
      result = new Matrix(a.Rows, b.Cols);
      for (var i = 0; i < result.Rows; i++)
      {
        for (var j = 0; j < result.Cols; j++)
        {
          var c = i;
          var d = j;
          TaskList.Add(Task.Run(() => CalcResultCell(c, d, a, b)));
        }
      }
      Task.WaitAll(TaskList.ToArray());
      Console.WriteLine();
      Console.WriteLine($"===========Thread usage stats:===========\n" +
        $"{string.Join("\n", tasks.Select(x => $"{x.Key} : {x.Value}").ToArray())}" +
        $"\n=========================================");
      return result;
    }

    private void CalcResultCell(int i, int j, Matrix a, Matrix b)
    {
      // Create local copy of both matrices to ensure that we work with same data during the test
      mut.WaitOne();
      Matrix aLocalCopy = new Matrix(a.Rows, a.Cols);
      aLocalCopy.ProcessFunctionOverData((i, j) => aLocalCopy[i, j] = a[i, j]);
      Matrix bLocalCopy = new Matrix(b.Rows, b.Cols);
      bLocalCopy.ProcessFunctionOverData((i, j) => bLocalCopy[i, j] = b[i, j]);
      mut.ReleaseMutex();

      // Perform calculations
      double res = 0;
      for (var k = 0; k < aLocalCopy.Cols; k++)
      {
        res += aLocalCopy[i, k] * bLocalCopy[k, j];
        Thread.Sleep(200);
        Console.Write(".");
      }

      // Apply thread stats and write result
      mut.WaitOne();
      int threadId = Thread.CurrentThread.ManagedThreadId;
      if (tasks.ContainsKey(threadId))
      {
        tasks[threadId]++;
      }
      else
      {
        tasks[threadId] = 1;
      }
      result[i, j] = res;
      mut.ReleaseMutex();
    }
  }
}