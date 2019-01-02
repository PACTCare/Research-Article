namespace JMIR
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Jmir.Console;
  using Jmir.Shared;

  public class Program
  {
    private const int NumberOfRuns = 10;

    public static void Main(string[] args)
    {
      Console.WriteLine("Start Program");
      Task.Run(async () => await new SendingRoutine(new ConsoleLogger()).RunAsync(NumberOfRuns)).GetAwaiter().GetResult();

      Console.ReadKey();
    }
  }
}