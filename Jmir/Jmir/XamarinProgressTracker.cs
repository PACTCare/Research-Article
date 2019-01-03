namespace Jmir
{
  using System;

  using Jmir.Shared;

  public class XamarinProgressTracker : IProgressTracker
  {
    public Action<string> Callback;

    public XamarinProgressTracker(int maxRounds, Action<string> callback)
    {
      this.Callback = callback;
      this.MaxRounds = maxRounds;
    }

    public int MaxRounds { get; }

    /// <inheritdoc />
    public void Update(int count)
    {
      this.Callback(((double)count / this.MaxRounds).ToString("P"));
    }
  }
}