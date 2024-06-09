using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    static class Program
    {
        internal static NGettext.Catalog tr;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            tr = new NGettext.Catalog(File.Open("../../../translations/pl.mo", FileMode.Open));
            MessageBox.Show(tr.GetString("hello"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimulatorForm());
        }
    }
}
