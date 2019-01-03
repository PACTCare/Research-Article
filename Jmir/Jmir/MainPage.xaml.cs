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
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
      ((MainPageViewModel)this.BindingContext).Run(int.Parse(this.Rounds.Text));
    }
  }
}
