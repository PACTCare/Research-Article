namespace Jmir.Shared
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public class SendingRoutine
  {
    public SendingRoutine(ILogger logger, IProgressTracker tracker)
    {
      this.Logger = logger;
      this.Tracker = tracker;
    }

    public ILogger Logger { get; }

    public IProgressTracker Tracker { get; }

    public async Task RunAsync(int rounds)
    {
      var timeList = new List<TimeObj>();
      var mam = new MAM(this.Logger);
      var n = 0;
      while (n < rounds)
      {
        n++;
        this.Tracker.Update(n);
        timeList.Add(await mam.PublishMessage("Hello World"));
      }

      foreach (var timeObj in timeList)
      {
        this.Logger.Log($"Create: {timeObj.CreateTime:0000} | Attach: {timeObj.AttachTime:0000}");
      }
    }
  }
}