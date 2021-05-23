using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace cassettePlayerSimulator
{
    public class Scaler
    {
        public Scaler(float factor = 1.0f)
        {
            scalingFactor = factor;
        }

        private float scalingFactor = 1.0f;

        public float ScalingFactor
        {
            get
            {
                return scalingFactor;
            }
            set
            {
                scalingFactor = value;
            }
        }

        public float S(float input)
        {
            return input * scalingFactor;
        }

        public int S(int input)
        {
            return (int)(input * scalingFactor);
        }

        public Rectangle S(Rectangle input)
        {
            return new Rectangle(
                S(input.X),
                S(input.Y),
                S(input.Width),
                S(input.Height));
        }
    }

    public class DpiScaler : Scaler
    {
        private static float dpiScaleFactor = 1.0f;

        public static float DpiScaleFactor => dpiScaleFactor;

        static DpiScaler()
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    dpiScaleFactor = g.DpiX / 96.0f;
                }
            }
        }

        public DpiScaler(): base(dpiScaleFactor)
        {

        }
    }
}
