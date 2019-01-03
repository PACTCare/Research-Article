namespace Jmir.ViewModels
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Runtime.CompilerServices;
  using System.Threading.Tasks;

  using Jmir.Models;
  using Jmir.Shared;

  public class MainPageViewModel : INotifyPropertyChanged
  {
    private List<Log> logs;

    public List<Log> Logs
    {
      get => this.logs;
      set
      {
        this.logs = value;
        this.OnPropertyChanged(nameof(this.Logs));
      }
    }

    public void Run(int rounds)
    {
      Task.Run(async () => await new SendingRoutine(new XamarinLogger(() => this.Logs = XamarinLogger.Logs)).RunAsync(rounds)).GetAwaiter()
        .GetResult();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
