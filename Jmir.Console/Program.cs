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
      var timeList = new List<TimeObj>();
      Task.Run(
        async () =>
          {
            var mam = new MAM(new ConsoleLogger());
            var n = 0;
            while (n < NumberOfRuns)
            {
              n++;
              Console.WriteLine(n);
              timeList.Add(await mam.PublishMessage("Hello World"));
            }

            Console.WriteLine("Create Time");
            foreach (var timeObj in timeList)
            {
              Console.WriteLine(timeObj.CreateTime);
            }

            Console.WriteLine("Attach Time");
            foreach (var timeObj in timeList)
            {
              Console.WriteLine(timeObj.AttachTime);
            }
          }).GetAwaiter().GetResult();


      Console.ReadKey();
    }
  }
}