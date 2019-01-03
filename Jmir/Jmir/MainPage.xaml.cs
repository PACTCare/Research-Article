using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Jmir
{
  using Jmir.ViewModels;

  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      this.BindingContext = new MainPageViewModel();
      InitializeComponent();

      this.StartButton.IsEnabled = true;
      this.StartButton.Text = "Start Routine";
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.Rounds.Text))
      {
        return;
      }

      this.StartButton.IsEnabled = false;
      ((MainPageViewModel)this.BindingContext).Run(int.Parse(this.Rounds.Text));
      this.StartButton.IsEnabled = true;
    }
  }
}
