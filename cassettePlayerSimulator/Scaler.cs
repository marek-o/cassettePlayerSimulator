using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace cassettePlayerSimulator
{
    class Scaler
    {
        static Scaler()
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    dpiScaleFactor = g.DpiX / 96.0f;
                }
            }
        }

        private static float dpiScaleFactor = 1.0f;

        public static float DpiScaleFactor => dpiScaleFactor;

        public static float Scale(float input, float factor)
        {
            return input * factor;
        }

        public static int Scale(int input, float factor)
        {
            return (int)(input * factor);
        }

        public static float DpiScale(float input)
        {
            return Scale(input, dpiScaleFactor);
        }

        public static int DpiScale(int input)
        {
            return Scale(input, dpiScaleFactor);
        }
    }
}
