using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiniFRCDriver
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public string[] settingLines;
        public string[] autoLines;
        public string[] offLines;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void saveSetButton_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+"settings.txt", settingsBox.Text);
            settingLines = Regex.Split(settingsBox.Text, "\r\n|\r|\n");
        }

        private void saveAutoButton_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "autonomous.txt", autoBox.Text);
            autoLines = Regex.Split(autoBox.Text, "\r\n|\r|\n");
        }

        private void loadSetButton_Click(object sender, RoutedEventArgs e)
        {
            settingsBox.Text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.txt");
            settingLines = Regex.Split(settingsBox.Text, "\r\n|\r|\n");
        }

        private void loadAutoButton_Click(object sender, RoutedEventArgs e)
        {
            autoBox.Text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "autonomous.txt");
            autoLines = Regex.Split(autoBox.Text, "\r\n|\r|\n");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        private void saveOffButton_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "off.txt", offBox.Text);
            offLines = Regex.Split(offBox.Text, "\r\n|\r|\n");
        }

        private void loadOffButton_Click(object sender, RoutedEventArgs e)
        {
            offBox.Text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "off.txt");
            offLines = Regex.Split(offBox.Text, "\r\n|\r|\n");
        }
    }
}
