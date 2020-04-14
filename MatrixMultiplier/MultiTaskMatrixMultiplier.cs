using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiplier
{
  internal class MultiTaskMatrixMultiplier : IMatrixMultiplier
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
      if (a.Rows != b.Cols)
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
      int threadId = Thread.CurrentThread.ManagedThreadId;
      double res = 0;
      for (var k = 0; k < a.Cols; k++)
      {
        res += a[i, k] * b[k, j];
        Thread.Sleep(200);
        Console.Write(".");
      }
      mut.WaitOne();
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