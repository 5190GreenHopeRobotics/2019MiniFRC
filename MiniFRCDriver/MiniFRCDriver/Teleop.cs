using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniFRCDriver
{
    class Teleop
    {
        SerialPort serialPort;

        public void Update()
        {
            MainWindow main = new MainWindow();
            SettingsWindow settings = new SettingsWindow();
            InputIdentifier inId = new InputIdentifier();

            serialPort = main.serialPort;
            if (main.teleopLoop)
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
                        main.consoleBox.Text = main.consoleBox.Text + message + "\n";
                    }
                    catch
                    {
                        main.errorMsg("Formatting was incorrect!");
                    }
                }
            }
        }
    }
}
