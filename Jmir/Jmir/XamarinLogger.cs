namespace Jmir
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  using Jmir.Models;
  using Jmir.Shared;

  public class XamarinLogger : ILogger
  {
    public Action Callback { get; }

    public XamarinLogger(Action callback)
    {
      this.Callback = callback;
      Logs = new List<Log>();
    }

    public static List<Log> Logs { get; set; }

    /// <inheritdoc />
    public void Log(string message)
    {
      Debug.Print(message);
      Logs.Add(new Log { Message = message });
      this.Callback();
    }
  }
}