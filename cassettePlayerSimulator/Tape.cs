using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using System.Windows.Forms;
using static cassettePlayerSimulator.Translations;

namespace cassettePlayerSimulator
{
    [Serializable]
    public class TapeList
    {
        public string Language;

        [XmlElement(ElementName = "Tape")]
        public List<Tape> Tapes;

        public TapeList()
        {
            Tapes = new List<Tape>();
        }

        public static TapeList Load(string path)
        {
            TapeList listOfTapes = new TapeList();

            if (File.Exists(path))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(TapeList));
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        var obj = serializer.Deserialize(stream);

                        listOfTapes = (TapeList)obj;
                    }
                }
                catch (Exception ex)
                {
                    var result = MessageBox.Show(_("Cannot load settings. Click OK to reset.")
                        + "\r\n\r\n" + ex.Message,
                        _("Error"), MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                    {
                        throw new Exception("abort");
                    }
                }
            }

            foreach (Tape t in listOfTapes.Tapes)
            {
                t.SideA.Parent = t;
                t.SideB.Parent = t;
            }

            return listOfTapes;
        }

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(TapeList));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);

                using (var stream2 = new FileStream(path, FileMode.Create))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(stream2);
                }
            }
        }
    }

    [Serializable]
    public class Tape
    {
        public TapeSide SideA;
        public TapeSide SideB;
        public float Position;
        public float Length;

        [XmlIgnore]
        public Color Color = Color.Red;

        [XmlElement("Color")]
        public string ColorString
        {
            get
            {
                var i = Color.ToArgb();
                return Convert.ToString(i, 16);
            }
            set
            {
                var i = Convert.ToInt32(value, 16);
                Color = Color.FromArgb(i);
            }
        }

        public Tape()
        {
            SideA = new TapeSide() { Parent = this };
            SideB = new TapeSide() { Parent = this };
        }
    }

    [Serializable]
    public class TapeSide
    {
        public string Label;
        public string FilePath;

        [XmlIgnore]
        public float Position
        {
            get
            {
                return (this == Parent.SideA) ? Parent.Position : Parent.Length - Parent.Position;
            }
            set
            {
                Parent.Position = (this == Parent.SideA) ? value : Parent.Length - value;
            }
        }

        [XmlIgnore]
        public Tape Parent;

        public TapeSide()
        {

        }
    }
}
