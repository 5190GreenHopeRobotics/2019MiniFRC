using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MiniFRCDriver
{
    class Autonomous
    {
        SerialPort serialPort;

        public void Start()
        {
            MainWindow main = new MainWindow();
            SettingsWindow settings = new SettingsWindow();
            try
            {
                serialPort = main.serialPort;
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
                            main.Dispatcher.Invoke(() => { main.consoleBox.Text = main.consoleBox.Text + message + "\n"; });

                            Thread.Sleep(Convert.ToInt32(instParts[instParts.Length - 1]) + 2);

                            main.Dispatcher.Invoke(() => { main.consoleBox.Text = main.consoleBox.Text + serialPort.ReadExisting() + "\n"; });
                        }
                    }
                    catch
                    {
                        main.errorMsg("Formatting was incorrect!");
                        serialPort.Close();
                    }
                }
                else
                {
                    main.errorMsg("Please load auto in the settings menu first.");
                }
                serialPort.Close();
            }
            catch
            {
                main.errorMsg("Please select a port.");
                serialPort.Close();
            }
        }
    }
}
