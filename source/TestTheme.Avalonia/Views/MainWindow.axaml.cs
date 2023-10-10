using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace TestTheme.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        bool t = true;
        public void ChangeTheme(object sender, RoutedEventArgs args)
        {
            t = !t;
            ThemeVariant theme = t ? ThemeVariant.Light : ThemeVariant.Dark;

            App.Current.RequestedThemeVariant = theme;
        }
    }
}