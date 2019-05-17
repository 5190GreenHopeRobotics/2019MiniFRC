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
        InputIdentifier inId = new InputIdentifier();

        Boolean teleopLoop = false;

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
                try
                {
                    serialPort.PortName = portList.SelectedItem.ToString();
                    serialPort.BaudRate = 9600;
                    portNameLabel.Content = portList.SelectedItem.ToString();
                }
                catch
                {
                    MessageBox.Show("You cannot change the port while it is in use.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var taskAuto = Task.Run( () => { autonomous(); });
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

        private void teleopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                teleopLoop = true;
                serialPort.Open();
            }
            catch
            {
                MessageBox.Show("Please select a port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void autonomous()
        {
            try
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
                            Dispatcher.Invoke(() => { consoleBox.Text = consoleBox.Text + message + "\n"; });

                            Thread.Sleep(Convert.ToInt32(instParts[instParts.Length - 1]) + 2);

                            Dispatcher.Invoke(() => { consoleBox.Text = consoleBox.Text + serialPort.ReadExisting() + "\n"; });
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Formatting was incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        serialPort.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Please load auto in the settings menu first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                serialPort.Close();
            }
            catch
            {
                MessageBox.Show("Please select a port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                serialPort.Close();
            }
        }

        public void teleop()
        {
            try
            {
                serialPort.Open();
                string[] settingLines = settings.settingLines;
                if (settingLines != null)
                {
                    while (teleopLoop)
                    {
                        try
                        {
                            string message = "z1";

                            foreach (string instruction in settingLines)
                            {
                                string[] instParts = Regex.Split(instruction, " ");
                                Key specifiedKey = (Key)Convert.ToChar(instParts[0]);
                                Dispatcher.Invoke(() =>
                                {
                                    if (Keyboard.IsKeyDown(specifiedKey))
                                    {
                                        message = message + " 1";
                                    }
                                    else
                                    {
                                        message = message + " 0";
                                    }
                                });
                            }
                            serialPort.WriteLine(message);
                            Dispatcher.Invoke(() => { consoleBox.Text = consoleBox.Text + message + "\n"; });
                        }
                        catch
                        {
                            MessageBox.Show("Formatting was incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                serialPort.Close();
            }
            catch
            {
                MessageBox.Show("Please select a port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                serialPort.Close();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            teleopLoop = false;
            serialPort.Close();
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keypress");
            if (teleopLoop)
            {
                string[] settingLines = settings.settingLines;
                if (settingLines != null)
                {
                    try
                    {
                        string message = "z1";

                        foreach (string instruction in settingLines)
                        {
                            Key specifiedKey = new Key();
                            string[] instParts = Regex.Split(instruction, " ");
                            if (instParts[0] == "axis")
                            {
                                string text = "0";
                                specifiedKey = inId.identifier(instParts[1]);
                                Key specifiedKey2 = inId.identifier(instParts[2]);
                                if (Keyboard.IsKeyDown(specifiedKey) && Keyboard.IsKeyDown(specifiedKey2)) { text = "0"; }
                                if (Keyboard.IsKeyDown(specifiedKey) && !Keyboard.IsKeyDown(specifiedKey2)) { text = "1"; }
                                if (Keyboard.IsKeyDown(specifiedKey2) && !Keyboard.IsKeyDown(specifiedKey)) { text = "-1"; }
                                message = message + " " + text;
                            }
                            if (instParts[0] == "button")
                            {
                                specifiedKey = inId.identifier(instParts[1]);
                                Console.WriteLine(specifiedKey);
                                if (Keyboard.IsKeyDown(specifiedKey))
                                {
                                    message = message + " 1";
                                }
                                else
                                {
                                    message = message + " 0";
                                }
                            }
                        }
                        serialPort.WriteLine(message);
                        consoleBox.Text = consoleBox.Text + message + "\n";
                    }
                    catch
                    {
                        MessageBox.Show("Formatting was incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void mainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (teleopLoop)
            {
                serialPort.WriteLine("z1 0 0");
            }
        }
    }
}
