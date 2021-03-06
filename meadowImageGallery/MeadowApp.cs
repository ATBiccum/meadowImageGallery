using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;

namespace meadowImageGallery
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7735 display;
        GraphicsLibrary graphics;
        

        public MeadowApp()
        {
            var led = new RgbLed(Device, Device.Pins.OnboardLedRed, Device.Pins.OnboardLedGreen, Device.Pins.OnboardLedBlue);
            led.SetColor(RgbLed.Colors.Red);
            
            var config = new SpiClockConfiguration(
                 speedKHz: 6000,
                 mode: SpiClockConfiguration.Mode.Mode3);

            display = new St7735
            (
                device: Device,
                spiBus: Device.CreateSpiBus(
                    clock: Device.Pins.SCK,
                    copi: Device.Pins.MOSI,
                    cipo: Device.Pins.MISO,
                    config: config),
                chipSelectPin: Device.Pins.D02,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 128, height: 160,
                displayType: St7735.DisplayType.ST7735R
            );

            graphics = new GraphicsLibrary(display);

            DisplayJPG();
            led.SetColor(RgbLed.Colors.Green);
        }

        void DisplayJPG()
        {
            var jpgData = LoadResource("image2.jpeg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x, y, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }
            
            display.Show();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"meadowImageGallery.{filename}";
        

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}