using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MatrixMultiplier
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
      string resourcesPath = Path.Combine(projectDir, "Resources");

      Matrix matrixA = LoadMatrixFromFile(Path.Join(resourcesPath, "matrixA.txt"));

      Matrix matrixB = LoadMatrixFromFile(Path.Join(resourcesPath, "matrixB.txt"));
      Console.WriteLine($"Will multiply matrix:\n{matrixA} by matrix:\n{matrixB}");

      int numThreads = 0;
      while (numThreads == 0)
      {
        Console.WriteLine("Please enter number of threads to use:");
        try
        {
          numThreads = Math.Min(24, Math.Max(1, int.Parse(Console.ReadLine())));
          Console.WriteLine($"Will use {numThreads} threads");
        }
        catch (Exception)
        {
          Console.WriteLine("Please use only numeric characters");
          numThreads = 0;
        }
      }
      var watch = System.Diagnostics.Stopwatch.StartNew();
      Matrix res = new MultiTaskMatrixMultiplier(numThreads).MultiplyMatrices(matrixA, matrixB);
      watch.Stop();
      Console.WriteLine("Result:");
      Console.WriteLine(res);
      var elapsedMs = watch.ElapsedMilliseconds;
      Console.WriteLine($"Calculations took {elapsedMs}ms using {numThreads} threads");
      Task writeProto = Task.Run(() => WriteCalculationsResults(matrixA, matrixB, res, elapsedMs, numThreads));
      Task.WaitAll(writeProto);
      Console.WriteLine("Results saved to proto file");
      Console.ReadKey();
    }

    private static Matrix LoadMatrixFromFile(string filePath)
    {
      string[] matrixRows = null;
      try
      {
        matrixRows = File.ReadAllText(filePath).Split("\n");
      }
      catch (IOException e)
      {
        Console.WriteLine($"Couldn't open file. Full exception details:\n{e.Message}\nProgram will close");
        Console.ReadKey();
        Environment.Exit(1);
      }

      int numRows = matrixRows.Length;
      int numCols = matrixRows[0].Trim().Split().Length;
      Matrix result = new Matrix(numRows, numCols); ;
      for (int i = 0; i < numRows; i++)
      {
        string[] line = matrixRows[i].Trim().Split();
        for (int j = 0; j < line.Length; j++)
        {
          if (result != null)
          {
            try
            {
              result[i, j] = double.Parse(line[j]);
            }
            catch (Exception e) when (e is FormatException || e is IndexOutOfRangeException)
            {
              Console.WriteLine($"Couldn't parse matrix from file {filePath}. " +
                $"Error on row {i + 1} column {j + 1}.\n" +
                $"Full exception details:\n{e.Message}\nProgram will close");
              Console.ReadKey();
              Environment.Exit(1);
            }
          }
        }
      }
      return result;
    }

    private static void WriteCalculationsResults(Matrix a, Matrix b, Matrix result, long time, int numThreads)
    {
      string protoPath = "protocol.txt";
      using (StreamWriter sw = File.AppendText(protoPath))
      {
        sw.WriteLine(new String('=', 48));
        sw.WriteLine($"Calculated at: {DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")}");
        sw.WriteLine($"Matrix 1:\n{a}");
        sw.WriteLine($"Matrix 2:\n{b}");
        sw.WriteLine($"Matrix 1 * Matrix 2 will produce the following matrix:\n{result}");
        sw.WriteLine($"Calculations took {time}ms using {numThreads} threads");
      }
    }
  }
}