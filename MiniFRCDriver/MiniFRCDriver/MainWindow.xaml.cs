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
        public SerialPort serialPort = new SerialPort();

        SettingsWindow settings = new SettingsWindow();
        ConsoleWindow console = new ConsoleWindow();
        InputIdentifier inId = new InputIdentifier();
        Autonomous Auto = new Autonomous();
        Teleop teleop = new Teleop();

        public Boolean teleopLoop = false;
        public Boolean debugMode = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        //refreshes the list of ports
        private void portRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            portList.Items.Clear();

            Console.WriteLine(SerialPort.GetPortNames());
            string[] ports = SerialPort.GetPortNames();

            foreach(string port in ports)
            {
                portList.Items.Add(port);
            }

            if (ports.Length == 0)
            {
                portList.Items.Add("No Devices");
            }
        }

        //loads selected port into memory
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (portList.SelectedItem != null)
            {
                try
                {   serialPort.PortName = portList.SelectedItem.ToString();
                    serialPort.BaudRate = 9600;
                    portNameLabel.Content = portList.SelectedItem.ToString();
                }catch{ errorMsg("You cannot change the port while it is in use."); }
            }
            else{ errorMsg("Please select a port."); }
        }

        //opens settings window
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        { try{ settings.Show(); }catch{ settings.Visibility = Visibility.Visible; } }

        //opens console window
        private void consoleButton_Click(object sender, RoutedEventArgs e)
        { try{ console.Show(); }catch{ console.Visibility = Visibility.Visible; } }

        //starts auto
        private void autoButton_Click(object sender, RoutedEventArgs e)
        { var taskAuto = Task.Run( () => { Auto.Start(); }); autoButton.Background = Brushes.Green; }

        //code to make consolebox scroll to bottom
        private void consoleBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        { if (consoleBox.IsVisible){ consoleBox.ScrollToLine(consoleBox.LineCount); } }

        //Code to run on window load
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.settingLines = Regex.Split(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.txt"), "\r\n|\r|\n");
                consoleBox.Text = consoleBox.Text + "Loaded Settings\n";
                settings.autoLines = Regex.Split(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "autonomous.txt"), "\r\n|\r|\n");
                consoleBox.Text = consoleBox.Text + "Loaded Autonomous\n";
            }
            catch
            {
                MessageBox.Show("Settings and Autonomous was not automatically loaded", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //starts teleop
        private void teleopButton_Click(object sender, RoutedEventArgs e)
        {   try{ teleopLoop = true; serialPort.Open(); teleopButton.Background = Brushes.Green; }
            catch{ errorMsg("Please select a port."); }
        }    

        //stops teleop
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {   teleopLoop = false;
            serialPort.Close();
            teleopButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            autoButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        }

        //Updates the Keys pressed and sends data
        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        { teleop.Update(); }

        //Updates the Keys pressed and sends data
        private void mainWindow_KeyUp(object sender, KeyEventArgs e)
        { teleop.Update(); }

        //Error Message Function
        public void errorMsg(string message)
        { MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        private void debugButton_Click(object sender, RoutedEventArgs e)
        {
            debugMode = !debugMode;
            if (debugMode) { debugButton.Background = Brushes.Green; }
            if (!debugMode) { debugButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)); }
        }

        public void debugConsole(string message)
        {

        }
    }
}
