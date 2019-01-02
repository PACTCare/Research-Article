namespace Jmir.Console
{
  using System;

  using Jmir.Shared;

  public class ConsoleLogger : ILogger
  {
    /// <inheritdoc />
    public void Log(string message)
    {
      Console.WriteLine(message);
    }
  }
}