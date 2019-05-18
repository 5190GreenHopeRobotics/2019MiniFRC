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

        public Boolean teleopLoop = false;
        public Boolean autoLoop = false;
        public Boolean debugMode = false;
        public Boolean connection = false;

        string lastKey;

        public string lastTeleop;
        public string lastAuto;

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
                SerialPort serialTester = new SerialPort();
                serialTester.PortName = port;
                serialTester.BaudRate = 9600;
                try
                {
                    serialTester.Open();
                    serialTester.Close();
                    portList.Items.Add(port);
                } catch { }
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
        {
            if(connection)
            {
                Thread th = new Thread(autonomous);
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                autoButton.Background = Brushes.Green;
            }
            else
            {
                errorMsg("Please connect first!");
            }
        }

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
                settings.offLines = Regex.Split(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "off.txt"), "r\n|\r|\n");
                consoleBox.Text = consoleBox.Text + "Loaded Off Settings\n";
            }
            catch
            {
                MessageBox.Show("Settings and Autonomous was not automatically loaded", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //starts teleop
        private void teleopButton_Click(object sender, RoutedEventArgs e)
        {   try
            {
                if (connection)
                {
                    teleopLoop = true;
                    teleopButton.Background = Brushes.Green;
                    Thread th = new Thread(teleopPackets);
                    th.Start();
                }
                else { errorMsg("Please connect first."); }
            }
            catch{ errorMsg("Please select a port."); }
        }    

        //stops all
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {   teleopLoop = false;
            serialPort.Close();
            off();
        }

        //Updates the Keys pressed and sends data
        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (lastKey != e.Key.ToString())
            {
                Parallel.Invoke(() => { teleop(); });
            }
            lastKey = e.Key.ToString();
        }

        //Updates the Keys pressed and sends data
        private void mainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            lastKey = "";
            Parallel.Invoke(() => { teleop(); });
        }

        //Error Message Function
        public void errorMsg(string message)
        { MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        private void debugButton_Click(object sender, RoutedEventArgs e)
        {
            debugMode = !debugMode;
            if (debugMode) { debugButton.Background = Brushes.Green; }
            if (!debugMode) { debugButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)); }
        }

        public void autonomous()
        {
            try
            {
                autoLoop = true;
                if (settings.autoLines != null)
                {
                    try
                    {
                        foreach (string instruction in settings.autoLines)
                        {
                            string[] instParts = Regex.Split(instruction, " ");
                            string message = "z1 " + instruction;
                            serialPort.WriteLine(message);
                            lastAuto = message;
                            Dispatcher.Invoke(() => { consoleBox.Text = consoleBox.Text + message + "\n"; });

                            Thread.Sleep(Convert.ToInt32(instParts[instParts.Length - 1]) + 10);

                            Dispatcher.Invoke(() => { consoleBox.Text = consoleBox.Text + serialPort.ReadExisting() + "\n"; });

                            string reset = "z0";
                            foreach (string i in settings.offLines)
                            {
                                message = message + " " + i;
                            }
                            serialPort.Write(message);
                        }
                    }
                    catch
                    {
                        errorMsg("Formatting was incorrect!");
                        autoLoop = false;
                        serialPort.Close();
                        Dispatcher.Invoke(() => { off(); });
                    }
                }
                else
                {
                    errorMsg("Please load auto in the settings menu first.");
                }
                autoLoop = false;
                Dispatcher.Invoke(() => { off(); });
            }
            catch
            {
                errorMsg("Please select a port.");
                autoLoop = false;
                Dispatcher.Invoke(() => { off(); });
            }
        }

        public void teleop()
        {
            if (connection && teleopLoop)
            {
                if (serialPort.IsOpen)
                {
                    if (settings.settingLines != null)
                    {
                        try
                        {
                            string message = "z0";

                            foreach (string instruction in settings.settingLines)
                            {
                                Key specifiedKey = new Key();
                                string[] instParts = Regex.Split(instruction, " ");
                                switch (instParts[0])
                                {
                                    case "axis":
                                        string text = "0";
                                        specifiedKey = InputIdentifier.identifier(instParts[1]);
                                        Key specifiedKey2 = InputIdentifier.identifier(instParts[2]);
                                        if (Keyboard.IsKeyDown(specifiedKey) && Keyboard.IsKeyDown(specifiedKey2)) { text = "0"; }
                                        if (Keyboard.IsKeyDown(specifiedKey) && !Keyboard.IsKeyDown(specifiedKey2)) { text = "1"; }
                                        if (Keyboard.IsKeyDown(specifiedKey2) && !Keyboard.IsKeyDown(specifiedKey)) { text = "-1"; }
                                        message = message + " " + text;
                                        break;
                                    case "button":
                                        specifiedKey = InputIdentifier.identifier(instParts[1]);
                                        Console.WriteLine(specifiedKey);
                                        if (Keyboard.IsKeyDown(specifiedKey))
                                        { message = message + " 1"; }
                                        else
                                        { message = message + " 0"; }
                                        break;
                                    default:
                                        errorMsg("Check your formatting!");
                                        Dispatcher.Invoke(() => { off(); });
                                        break;
                                }
                            }
                            try
                            {
                                serialPort.WriteLine(message);
                                consoleBox.Text = consoleBox.Text + message + "\n";
                                lastTeleop = message;
                                Console.WriteLine(message);
                            }
                            catch
                            {
                                errorMsg("Lost Connection!");
                                Dispatcher.Invoke(() => { off(); });
                            }
                        }
                        catch
                        {
                            errorMsg("An error occured! Check your formatting.");
                            Dispatcher.Invoke(() => { off(); });
                        }
                    }
                }
                else
                {
                    errorMsg("Lost Connection!");
                }
            }
            else
            {
                errorMsg("Please connect first.");
            }
        }

        public void teleopPackets()
        {
            while (teleopLoop)
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        Console.WriteLine("1teleopLoop");
                        serialPort.WriteLine(lastTeleop);
                        Thread.Sleep(20);
                        Console.WriteLine("2teleoploop");
                    }
                    catch
                    {
                        Console.WriteLine("teleoploopbreak");
                        Dispatcher.Invoke(() => { teleopLoop = false; });
                        errorMsg("port was closed!");
                        Dispatcher.Invoke(() => { off(); });
                        Console.WriteLine("teleoploopbreak2");
                    }
                }
                else
                {
                    errorMsg("Lost Connection!");
                }
            }
        }
        
        public void autoPackets()
        {
            while (autoLoop)
            {
                try
                {
                    serialPort.WriteLine(lastAuto);
                    Thread.Sleep(20);
                }
                catch
                {
                    errorMsg("port was closed!");
                    off();
                }
            }
        }

        public void off()
        {
            teleopButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            autoButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            connection = false;
            connectButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            try { serialPort.Close(); } catch { }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            connection = !connection;
            if (connection = true)
            {
                try
                {
                    serialPort.Open();
                    connectButton.Background = Brushes.Green;
                    connectButton.Content = "Connected!";
                }
                catch
                {
                    errorMsg("Please choose a port first!");
                }
            }
            else
            {
                serialPort.Close();
                off();
                connectButton.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                connectButton.Content = "Connect";
            }
        }
    }
}
