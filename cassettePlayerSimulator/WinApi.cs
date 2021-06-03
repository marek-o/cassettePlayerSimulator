using System;
using System.Runtime.InteropServices;

namespace cassettePlayerSimulator
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0xB;

        public static void SuspendRedraw(IntPtr handle)
        {
            SendMessage(handle, WM_SETREDRAW, new IntPtr(0), IntPtr.Zero);
        }

        public static void ResumeRedraw(IntPtr handle)
        {
            SendMessage(handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
        }
    }
}
