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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace MiniFRCDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string currentPort = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void portRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Refresh Clicked");

            portList.Items.Clear();

            Console.WriteLine(SerialPort.GetPortNames());
            string[] ports = SerialPort.GetPortNames();

            foreach(string port in ports)
            {
                portList.Items.Add(port);
                Console.WriteLine(port);
            }

            if (ports.Length == 0)
            {
                portList.Items.Add("No Devices");
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (portList.SelectedItem != null)
            {
                currentPort = portList.SelectedItem.ToString();
                portNameLabel.Content = portList.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a port first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
        }

        private void consoleButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWindow console = new ConsoleWindow();
            console.Show();
        }
    }
}
