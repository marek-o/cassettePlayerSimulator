using System.Drawing;

namespace cassettePlayerSimulator
{
    public class Common
    {
        public static readonly NAudio.Wave.WaveFormat WaveFormat = new NAudio.Wave.WaveFormat(44100, 16, 2);

        public static readonly Color BorderColor = Color.FromArgb(0, 0, 0);
        public static readonly Color CoverColor = Color.FromArgb(169, 169, 169);
        public static readonly Color ButtonFaceColor = Color.FromArgb(169, 169, 169);
        public static readonly Color ButtonTopColor = Color.FromArgb(211, 211, 211);
        public static readonly Color ButtonLeftColor = Color.FromArgb(128, 128, 128);
        public static readonly Color SymbolBlackColor = Color.FromArgb(0, 0, 0);
        public static readonly Color SymbolRedColor = Color.FromArgb(255, 0, 0);

        public static readonly Color TapeColor = Color.FromArgb(128, 64, 0);
        public static readonly Color SpoolColor = Color.FromArgb(240, 240, 240);
        public static readonly Color BlackWheelColor = Color.FromArgb(64, 64, 64);
        public static readonly Color AxisColor = Color.FromArgb(192, 192, 192);

        public static readonly Color CounterWheelColor = Color.FromArgb(96, 96, 96);

        public static readonly Color CassetteBodyColor = Color.FromArgb(0, 0, 0);

        public static readonly Pen BorderPen = new Pen(BorderColor);
        public static readonly Pen TapePen = new Pen(TapeColor);

        public static readonly Brush CoverBrush = new SolidBrush(CoverColor);
        public static readonly Brush ButtonFaceBrush = new SolidBrush(ButtonFaceColor);
        public static readonly Brush ButtonTopBrush = new SolidBrush(ButtonTopColor);
        public static readonly Brush ButtonLeftBrush = new SolidBrush(ButtonLeftColor);
        public static readonly Brush SymbolBlackBrush = new SolidBrush(SymbolBlackColor);
        public static readonly Brush SymbolRedBrush = new SolidBrush(SymbolRedColor);

        public static readonly Brush TapeBrush = new SolidBrush(TapeColor);
        public static readonly Brush SpoolBrush = new SolidBrush(SpoolColor);
        public static readonly Brush BlackWheelBrush = new SolidBrush(BlackWheelColor);
        public static readonly Brush AxisBrush = new SolidBrush(AxisColor);

        public static readonly Brush CounterWheelBrush = new SolidBrush(CounterWheelColor);

        public static readonly Brush CassetteBodyBrush = new SolidBrush(CassetteBodyColor);
    }
}
