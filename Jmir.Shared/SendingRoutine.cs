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
        this.Logger.Log(n.ToString());
        timeList.Add(await mam.PublishMessage("Hello World"));
      }

      this.Logger.Log("Create Time");
      long summedCreateTime = 0;
      foreach (var timeObj in timeList)
      {
        this.Logger.Log($"{timeObj.CreateTime:0000} Milliseconds");
        summedCreateTime += long.Parse(timeObj.CreateTime.ToString());
      }

      this.Logger.Log("Attach Time");
      long summedAttachTime = 0;
      foreach (var timeObj in timeList)
      {
        this.Logger.Log($"{timeObj.AttachTime:0000} Milliseconds");
        summedAttachTime += long.Parse(timeObj.AttachTime.ToString());
      }

      this.Logger.Log($"Average Create Time: {summedCreateTime / rounds} milliseconds");
      this.Logger.Log($"Average Create Time: {summedAttachTime / rounds} milliseconds");
    }
  }
}