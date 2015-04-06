using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Math = System.Math;

// for the LEDs
using STM32F429I_Discovery.Netmf.Hardware;
using System.Collections;

/*
 * --== Inspiration taken from: == --
 * https://gist.github.com/stevenlr/824019#file-gistfile1-c 
 * http://lodev.org/cgtutor/plasma.html
 * 
 */

namespace NetMfPlasma
{
    public class Program
    {
        public static void Main()
        {
            Bitmap bmp;

            short SCREEN_WIDTH = (Int16)SystemMetrics.ScreenWidth;
            short SCREEN_HEIGHT = (Int16)SystemMetrics.ScreenHeight;
            float f = 0;

            byte[] r = new byte[256];
            byte[] g = new byte[256];
            byte[] b = new byte[256];

            var animation = new ArrayList();
            short pos = 0; // current animation position
            short inc = 1; 

            for (int x = 0; x < 256; x++)
            {
                r[x] = (byte)(255 - Math.Ceiling((Math.Sin(3.14159 * 2 * x / 255) + 1) * 127));
                g[x] = (byte)Math.Ceiling((Math.Sin(3.14159 * 2 * x / 127.0) + 1) * 64);
                b[x] = (byte)(255 - r[x]);
            }

            LED.LEDInit();
            LED.GreenLedOff();
            LED.RedLedOff();

            for (var a = 0; a < 12; a++)
            {
                bmp = new Bitmap(SystemMetrics.ScreenWidth / 2, SystemMetrics.ScreenHeight / 2);
                LED.RedLedToggle();
                for (int y = 0; y < SCREEN_HEIGHT / 2; y++)
                {
                    LED.GreenLedToggle();
                    for (int x = 0; x < SCREEN_WIDTH / 2; x++)
                    {
                        double c1 = Math.Sin(x / 50.0 + f + y / SCREEN_HEIGHT);
                        double c2 = Math.Sqrt((Math.Sin(0.8 * f) * 160 - x + 160) * (Math.Sin(0.8 * f) * 160 - x + 160) + (Math.Cos(1.2 * f) * 100 - y + 100) * (Math.Cos(1.2 * f) * 100 - y + 100));
                        c2 = Math.Sin(c2 / 50.0);
                        double c3 = (c1 + c2) / 2;

                        int res = (int)Math.Ceiling((c3 + 1) * 127);

                        bmp.SetPixel(x, y, (Color)(0xff0000 * r[res] + 0xff00 * g[res] + 0xff * b[res]));
                    }
                }

                var newBmp = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);

                newBmp.StretchImage(0, 0, // destination x, y
                                bmp, // source bitmap
                                240, // width of destination
                                320, // height of destination
                                0xff); // opacity

                animation.Add(newBmp);

                f += 0.25f;
            }


            while (true)
            {
                var bm = (Bitmap)animation[pos];
                bm.Flush();

                pos += inc;

                if (pos + inc == 12)
                {
                    inc *= -1;
                }
                else if (pos + inc < 0)
                {
                    inc *= -1;
                }

                bm = null;

                System.Threading.Thread.Sleep(200);
            }
        }
    }
}
