using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetToggleButtonState();

        }

        private void SetToggleButtonState()
        {
            buttonInjectorToggle.Content = "Click to INJECT";

            if (QModInstaller.QModInjector.IsPatcherInjected())
            {
                buttonInjectorToggle.IsChecked = true;
                buttonInjectorToggle.Content = "Click to REMOVE";
            }
        }

        private void buttonInjectorToggle_Checked(object sender, RoutedEventArgs e)
        {
            QModInstaller.QModInjector.Inject();
            buttonInjectorToggle.Content = "Click to REMOVE";
        }

        private void buttonInjectorToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            QModInstaller.QModInjector.Remove();
            buttonInjectorToggle.Content = "Click to INJECT";
        }
    }
}
