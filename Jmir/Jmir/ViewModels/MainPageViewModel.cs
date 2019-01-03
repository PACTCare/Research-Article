namespace Jmir.ViewModels
{
  using System.ComponentModel;
  using System.Runtime.CompilerServices;
  using Jmir.Shared;

  public class MainPageViewModel : INotifyPropertyChanged
  {
    private string logs = string.Empty;

    public string Logs
    {
      get => this.logs;
      set
      {
        this.logs = value;
        this.OnPropertyChanged(nameof(this.Logs));
      }
    }

    public async void Run(int rounds)
    {
      await new SendingRoutine(
        new XamarinLogger(
          () =>
            {
              var internalLogs = string.Empty;
              foreach (var log in XamarinLogger.Logs)
              {
                internalLogs += $"\n {log.Message}";
                this.Logs = internalLogs;
              }
            }),
        new XamarinProgressTracker(rounds, progress => { this.Logs = progress; })).RunAsync(rounds);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
