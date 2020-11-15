using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassettePlayerSimulator
{
    class Tape
    {
        public TapeSide SideA;
        public TapeSide SideB;

        public Tape(float sideLengthSeconds)
        {
            SideA = new TapeSide() { Length = sideLengthSeconds };
            SideB = new TapeSide() { Length = sideLengthSeconds };
        }

        public Tape(string pathA, string pathB)
        {
            SideA = new TapeSide(pathA);
            SideB = new TapeSide(pathB);
        }
    }

    class TapeSide
    {
        public string Label;
        public string FilePath;
        public float Position;
        public float Length;

        public TapeSide()
        {

        }

        public TapeSide(string path)
        {
            FilePath = path;
            Label = "no name";
            Position = 0.0f;
            Length = 123.0f; //FIXME
        }
    }
}
