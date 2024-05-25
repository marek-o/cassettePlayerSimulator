using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Utils
{
    public class SoundWrapper
    {
        private uint bufferLengthBytes;
        private uint sampleRate;
        private ushort bitsPerSample;
        private ushort channels;

        public event EventHandler<NewDataEventArgs> NewDataPresent;
        public event EventHandler<NewDataEventArgs> NewDataRequested;

        private SoundCallbackDelegate soundCallbackDelegate; //to prevent GC from deleting
        private IntPtr waveHandle = IntPtr.Zero;

        public enum Mode
        {
            NotSet, Play, Record
        }

        public class DeviceInfo
        {
            public int index;
            public string name;

            public override string ToString()
            {
                return string.Format("{0}: {1}", index, name);
            }
        }
        
        private Mode mode;
        private IntPtr[] bufferIP = new IntPtr[2];
        private bool finishing = false;
        private object locker = new object();

        public SoundWrapper(Mode mode, ushort bitsPerSample, ushort channels, uint sampleRate, uint bufferLengthBytes)
        {
            this.mode = mode;
            this.bitsPerSample = bitsPerSample;
            this.channels = channels;
            this.sampleRate = sampleRate;
            this.bufferLengthBytes = bufferLengthBytes;
        }

        public IEnumerable<DeviceInfo> EnumerateDevices()
        {
            if (mode == Mode.Record)
            {
                uint count = waveInGetNumDevs();

                for (uint i = 0; i < count; ++i)
                {
                    WAVEINCAPS caps = new WAVEINCAPS();
                    if (waveInGetDevCaps(i, ref caps, 48) == 0)
                    {
                        yield return new DeviceInfo() { index = (int)i, name = caps.szPname };
                    }
                }
            }
            else if (mode == Mode.Play)
            {
                uint count = waveOutGetNumDevs();

                for (uint i = 0; i < count; ++i)
                {
                    WAVEOUTCAPS caps = new WAVEOUTCAPS();
                    if (waveOutGetDevCaps(i, ref caps, 48) == 0)
                    {
                        yield return new DeviceInfo() { index = (int)i, name = caps.szPname };
                    }
                }
            }
        }

        public void Start(int device)
        {
            if (waveHandle != IntPtr.Zero)
            {
                return;
            }

            WAVEFORMATEX waveFormatEx = new WAVEFORMATEX();
            waveFormatEx.wFormatTag = WAVE_FORMAT_PCM;
            waveFormatEx.nChannels = channels;
            waveFormatEx.nSamplesPerSec = sampleRate;
            waveFormatEx.wBitsPerSample = bitsPerSample;
            waveFormatEx.cbSize = 0;

            waveFormatEx.nBlockAlign = (ushort)(waveFormatEx.nChannels * waveFormatEx.wBitsPerSample / 8);
            waveFormatEx.nAvgBytesPerSec = waveFormatEx.nSamplesPerSec * waveFormatEx.nBlockAlign;

            if (mode == Mode.Record)
            {
                soundCallbackDelegate = SoundCallbackRecording;
                CheckError(waveInOpen(ref waveHandle, (uint)device, ref waveFormatEx, soundCallbackDelegate, this, CALLBACK_FUNCTION));
            }
            else if (mode == Mode.Play)
            {
                soundCallbackDelegate = SoundCallbackPlaying;
                CheckError(waveOutOpen(ref waveHandle, (uint)device, ref waveFormatEx, soundCallbackDelegate, this, CALLBACK_FUNCTION));
            }

            for (int i = 0; i < 2; ++i)
            {
                unsafe
                {
                    bufferIP[i] = Marshal.AllocHGlobal(48); // to prevent GC from deleting

                    WAVEHDR* bufptr = (WAVEHDR*)bufferIP[i].ToPointer();

                    bufptr->lpData = Marshal.AllocHGlobal((int)bufferLengthBytes);
                    bufptr->dwBufferLength = bufferLengthBytes;
                    bufptr->dwFlags = 0;

                    short[] data = new short[bufferLengthBytes / 2];
                    Marshal.Copy(data, 0, bufptr->lpData, (int)(bufferLengthBytes / 2));

                    if (mode == Mode.Record)
                    {
                        CheckError(waveInPrepareHeader(waveHandle, ref *bufptr, 48));
                        CheckError(waveInAddBuffer(waveHandle, ref *(WAVEHDR*)bufferIP[i].ToPointer(), 48));
                    }
                    else if (mode == Mode.Play)
                    {
                        CheckError(waveOutPrepareHeader(waveHandle, ref *bufptr, 48));
                        CheckError(waveOutWrite(waveHandle, ref *(WAVEHDR*)bufferIP[i].ToPointer(), 48));
                    }
                }
            }

            if (mode == Mode.Record)
            {
                CheckError(waveInStart(waveHandle));
            }
        }

        public void Stop()
        {
            if (waveHandle == IntPtr.Zero)
            {
                return;
            }

            finishing = true;

            lock (locker)
            {
                if (mode == Mode.Record)
                {
                    CheckError(waveInReset(waveHandle));
                }
                else
                {
                    CheckError(waveOutReset(waveHandle));
                }
            }

            for (int i = 0; i < 2; ++i)
            {
                unsafe
                {
                    WAVEHDR* bufptr = (WAVEHDR*)bufferIP[i].ToPointer();

                    if (mode == Mode.Record)
                    {
                        CheckError(waveInUnprepareHeader(waveHandle, ref *bufptr, 48));
                    }
                    else if (mode == Mode.Play)
                    {
                        CheckError(waveOutUnprepareHeader(waveHandle, ref *bufptr, 48));
                    }

                    Marshal.FreeHGlobal(bufptr->lpData);
                    Marshal.FreeHGlobal(bufferIP[i]);
                }
            }

            if (mode == Mode.Record)
            {
                CheckError(waveInClose(waveHandle));
            }
            else if (mode == Mode.Play)
            {
                CheckError(waveOutClose(waveHandle));
            }

            waveHandle = IntPtr.Zero;
            finishing = false;
        }

        private void CheckError(uint errorCode)
        {
            if (errorCode != 0)
            {
                const int maxLength = 256;
                StringBuilder sb = new StringBuilder(maxLength);

                if (mode == Mode.Record)
                {
                    waveInGetErrorText(errorCode, sb, maxLength);
                }
                else if (mode == Mode.Play)
                {
                    waveOutGetErrorText(errorCode, sb, maxLength);
                }

                throw new Exception(sb.ToString());
            }
        }

        public class NewDataEventArgs : EventArgs
        {
            public short[] data;

            public NewDataEventArgs(short[] data)
            {
                this.data = data;
            }
        }

        private struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        private struct WAVEHDR
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public IntPtr reserved;
        }

        private struct WAVEINCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String szPname;
            public uint dwFormats;
            public ushort wChannels;
            public ushort wReserved1;
        }

        private struct WAVEOUTCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String szPname;
            public uint dwFormats;
            public ushort wChannels;
            public ushort wReserved1;
        }

        private const uint WAVE_MAPPER = 0xffffffff;
        private const ushort WAVE_FORMAT_PCM = 1;
        private const uint CALLBACK_FUNCTION = 0x00030000;
        private const uint WIM_OPEN = 0x3BE;
        private const uint WIM_CLOSE = 0x3BF;
        private const uint WIM_DATA = 0x3C0;
        
        private const uint WOM_OPEN = 0x3BB;
        private const uint WOM_CLOSE = 0x3BC;
        private const uint WOM_DONE = 0x3BD;

        [DllImport("winmm.dll")]
        private static extern uint waveInOpen(ref IntPtr phwi, uint deviceID, ref WAVEFORMATEX wfx, SoundCallbackDelegate dwCallback, SoundWrapper dwInstance, uint dwFlags);
        [DllImport("winmm.dll")]
        private static extern uint waveInPrepareHeader(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveInAddBuffer(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveInStart(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveInGetErrorText(uint mmrError, StringBuilder pszText, uint cchText);
        [DllImport("winmm.dll")]
        private static extern uint waveInReset(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveInStop(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveInClose(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveInUnprepareHeader(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveInGetNumDevs();
        [DllImport("winmm.dll")]
        private static extern uint waveInGetDevCaps(uint uDeviceID, ref WAVEINCAPS pwic, uint cbwic);


        [DllImport("winmm.dll")]
        private static extern uint waveOutOpen(ref IntPtr phwi, uint deviceID, ref WAVEFORMATEX wfx, SoundCallbackDelegate dwCallback, SoundWrapper dwInstance, uint dwFlags);
        [DllImport("winmm.dll")]
        private static extern uint waveOutPrepareHeader(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveOutRestart(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveOutGetErrorText(uint mmrError, StringBuilder pszText, uint cchText);
        [DllImport("winmm.dll")]
        private static extern uint waveOutReset(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveOutPause(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveOutClose(IntPtr phwi);
        [DllImport("winmm.dll")]
        private static extern uint waveOutUnprepareHeader(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveOutWrite(IntPtr phwi, ref WAVEHDR pwh, uint cbwh);
        [DllImport("winmm.dll")]
        private static extern uint waveOutGetNumDevs();
        [DllImport("winmm.dll")]
        private static extern uint waveOutGetDevCaps(uint uDeviceID, ref WAVEOUTCAPS pwic, uint cbwic);

        private delegate void SoundCallbackDelegate(IntPtr hwi, uint uMsg, SoundWrapper dwInstance, ulong dwParam1, ulong dwParam2);

        private static unsafe void SoundCallbackRecording(IntPtr hwi, uint uMsg, SoundWrapper dwInstance, ulong dwParam1, ulong dwParam2)
        {
            if (uMsg == WIM_DATA)
            {
                WAVEHDR* bufptr = (WAVEHDR*)dwParam1;

                short[] data = new short[dwInstance.bufferLengthBytes / 2];
                Marshal.Copy(bufptr->lpData, data, 0, (int)(bufptr->dwBytesRecorded / 2));

                if (!dwInstance.finishing)
                {
                    uint retval, retval2, retval3;

                    lock (dwInstance.locker)
                    {
                        retval = waveInUnprepareHeader(dwInstance.waveHandle, ref *bufptr, 48);
                        retval2 = waveInPrepareHeader(dwInstance.waveHandle, ref *bufptr, 48);
                        retval3 = waveInAddBuffer(dwInstance.waveHandle, ref *bufptr, 48);
                    }

                    if (retval != 0 || retval2 != 0 || retval3 != 0)
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format("error in WIM_DATA: {0} {1} {2}\n", retval, retval2, retval3));
                    }
                }

                dwInstance.OnNewDataPresent(data);
            }
        }

        private static unsafe void SoundCallbackPlaying(IntPtr hwi, uint uMsg, SoundWrapper dwInstance, ulong dwParam1, ulong dwParam2)
        {
            if (uMsg == WOM_DONE)
            {
                WAVEHDR* bufptr = (WAVEHDR*)dwParam1;

                short[] data = new short[dwInstance.bufferLengthBytes / 2];
                dwInstance.OnNewDataRequested(data);

                Marshal.Copy(data, 0, bufptr->lpData, (int)(dwInstance.bufferLengthBytes / 2));

                if (!dwInstance.finishing)
                {
                    uint retval, retval2, retval3;

                    lock (dwInstance.locker)
                    {
                        retval = waveOutUnprepareHeader(dwInstance.waveHandle, ref *bufptr, 48);
                        retval2 = waveOutPrepareHeader(dwInstance.waveHandle, ref *bufptr, 48);
                        retval3 = waveOutWrite(dwInstance.waveHandle, ref *bufptr, 48);
                    }

                    if (retval != 0 || retval2 != 0 || retval3 != 0)
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format("error in WOM_DATA: {0} {1} {2}\n", retval, retval2, retval3));
                    }
                }
            }
        }

        private void OnNewDataPresent(short[] data)
        {
            if (NewDataPresent != null)
            {
                NewDataPresent(this, new NewDataEventArgs(data));
            }
        }

        private void OnNewDataRequested(short[] data)
        {
            if (NewDataRequested != null)
            {
                NewDataRequested(this, new NewDataEventArgs(data));
            }
        }
    }
}
