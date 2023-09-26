using Microsoft.UI.Xaml.Controls;

using NumericUpDownLib.WinUI.Demo.ViewModels;

namespace NumericUpDownLib.WinUI.Demo.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = App.GetService<BlankViewModel>();
        InitializeComponent();
    }
}
