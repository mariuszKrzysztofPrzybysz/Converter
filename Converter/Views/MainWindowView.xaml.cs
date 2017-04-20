using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Converter.Controlers;

namespace Converter.Views
{
    /// <summary>
    ///     Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private readonly MainWindowControler _mainWindowControler = new MainWindowControler();

        public MainWindowView()
        {
            InitializeComponent();
            DataContext = _mainWindowControler;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}