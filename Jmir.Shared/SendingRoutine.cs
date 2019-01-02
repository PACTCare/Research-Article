namespace Jmir.Shared
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public class SendingRoutine
  {
    public SendingRoutine(ILogger logger)
    {
      this.Logger = logger;
    }

    public ILogger Logger { get; }

    public async Task RunAsync(int rounds)
    {
      var timeList = new List<TimeObj>();
      var mam = new MAM(this.Logger);
      var n = 0;
      while (n < rounds)
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
    }
  }
}