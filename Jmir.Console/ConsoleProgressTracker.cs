using System;
using System.Collections.Generic;
using System.Text;

namespace Jmir.Console
{
  using Jmir.Shared;

  using Console = System.Console;

  public class ConsoleProgressTracker : IProgressTracker
  {
    public int MaxRounds { get; }

    public ConsoleProgressTracker(int maxRounds)
    {
      this.MaxRounds = maxRounds;
    }

    /// <inheritdoc />
    public void Update(int count)
    {
      Console.WriteLine(((double)count / this.MaxRounds).ToString("P"));
    }
  }
}
