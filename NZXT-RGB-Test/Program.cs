using CAM.Hardware.COM.Models.Light;
using CAM.Hardware.COM.Models.Light.Tab;
using CAM.Hardware.PC.Controllers;
using CAM.Hardware.USB.Controllers.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NZXT_RGB_Test
{
    class Program
    {
        static Computer computer = Computer.Instance();

        static SmartDevice smartDevice;
        static Channel lightChannel;

        static void Main(string[] args)
        {
            InitRGB();
            new Thread(RGBUpdateThread).Start();

            Console.ReadLine();
        }

        static void RGBUpdateThread()
        {
            bool alt = false;
            var leds = lightChannel.CustomTab.Profiles[0].Colors;

            var red = new Color() { R = 255, G = 0, B = 0 };
            var blue = new Color() { R = 0, G = 0, B = 255 };

            // create red and blue alternating pattern
            while (true)
            {
                for(int i = 0; i < 20; i++)
                {
                    if (i % 2 == (alt ? 1 : 0))
                        leds[i] = red;
                    else
                        leds[i] = blue;
                }
                smartDevice.UpdateChannel(lightChannel, 0, computer.Properties.SmartDevices[0].DeviceNumber, true);
                alt = !alt;

                Thread.Sleep(500);
            }
        }

        static void InitRGB()
        {
            computer.Detect_NZXT_USB_Products(); // search for smart devices

            while(computer.Properties.SmartDevices == null) // wait until search is complete
                Thread.Sleep(10);

            //get instance of smart device and create a new custom light channel
            smartDevice = SmartDevice.Instance();
            lightChannel = new Channel() { TabIndex = "CUSTOM", CustomTab = Custom.InitDefault(20) };
        }
    }
}
