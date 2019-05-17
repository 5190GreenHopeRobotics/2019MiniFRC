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
using System.Text.RegularExpressions;
using System.Threading;

namespace MiniFRCDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort = new SerialPort();

        SettingsWindow settings = new SettingsWindow();
        ConsoleWindow console = new ConsoleWindow();

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
                serialPort.PortName = portList.SelectedItem.ToString();
                serialPort.BaudRate = 9600;
                portNameLabel.Content = portList.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a port first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.Show();
            }
            catch
            {
                settings.Visibility = Visibility.Visible;
            }
        }

        private void consoleButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWindow console = new ConsoleWindow();
            console.Show();
        }

        private void autoButton_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Open();
            string[] autoLines = settings.autoLines;
            if (autoLines != null)
            {
                try
                {
                    foreach (string instruction in autoLines)
                    {
                        string[] instParts = Regex.Split(instruction, " ");
                        string message = "z2 " + instruction;
                        serialPort.WriteLine(message);
                        consoleBox.Text = consoleBox.Text + message + "\n";

                        Thread.Sleep(Convert.ToInt32(instParts[instParts.Length - 1]) + 2);

                        consoleBox.Text = consoleBox.Text + serialPort.ReadExisting() + "\n";
                    }
                }
                catch
                {
                    MessageBox.Show("Formatting was incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please load auto in the settings menu first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            serialPort.Close();
        }

        private void consoleBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (consoleBox.IsVisible)
            {
                consoleBox.ScrollToEnd();
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            settings.settingLines = Regex.Split(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.txt"), "\r\n|\r|\n");
            consoleBox.Text = consoleBox.Text + "Loaded Settings\n";
            settings.autoLines = Regex.Split(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "autonomous.txt"), "\r\n|\r|\n");
            consoleBox.Text = consoleBox.Text + "Loaded Autonomous\n";
        }
    }
}
