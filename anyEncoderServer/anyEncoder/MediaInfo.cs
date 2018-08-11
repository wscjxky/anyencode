namespace anyEncoder
{
    using System;
    using System.Runtime.InteropServices;

    public class MediaInfo
    {
        private IntPtr Handle = MediaInfo_New();
        private bool MustUseAnsi;

        public MediaInfo()
        {
            if (Environment.OSVersion.ToString().IndexOf("Windows") == -1)
            {
                this.MustUseAnsi = true;
            }
            else
            {
                this.MustUseAnsi = false;
            }
        }

        public void Close()
        {
            MediaInfo_Close(this.Handle);
        }

        public int Count_Get(StreamKind StreamKind)
        {
            return this.Count_Get(StreamKind, -1);
        }

        public int Count_Get(StreamKind StreamKind, int StreamNumber)
        {
            return (int) MediaInfo_Count_Get(this.Handle, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber);
        }

        ~MediaInfo()
        {
            MediaInfo_Delete(this.Handle);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter)
        {
            return this.Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter)
        {
            return this.Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {
            if (this.MustUseAnsi)
            {
                return Marshal.PtrToStringAnsi(MediaInfoA_GetI(this.Handle, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, (IntPtr) Parameter, (IntPtr) ((long) KindOfInfo)));
            }
            return Marshal.PtrToStringUni(MediaInfo_GetI(this.Handle, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, (IntPtr) Parameter, (IntPtr) ((long) KindOfInfo)));
        }

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
        {
            return this.Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
        {
            if (this.MustUseAnsi)
            {
                IntPtr parameter = Marshal.StringToHGlobalAnsi(Parameter);
                string str = Marshal.PtrToStringAnsi(MediaInfoA_Get(this.Handle, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, parameter, (IntPtr) ((long) KindOfInfo), (IntPtr) ((long) KindOfSearch)));
                Marshal.FreeHGlobal(parameter);
                return str;
            }
            return Marshal.PtrToStringUni(MediaInfo_Get(this.Handle, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, Parameter, (IntPtr) ((long) KindOfInfo), (IntPtr) ((long) KindOfSearch)));
        }

        public string Inform()
        {
            if (this.MustUseAnsi)
            {
                return Marshal.PtrToStringAnsi(MediaInfoA_Inform(this.Handle, IntPtr.Zero));
            }
            return Marshal.PtrToStringUni(MediaInfo_Inform(this.Handle, IntPtr.Zero));
        }

        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
        public int Open(string FileName)
        {
            if (this.MustUseAnsi)
            {
                IntPtr fileName = Marshal.StringToHGlobalAnsi(FileName);
                int num = (int) MediaInfoA_Open(this.Handle, fileName);
                Marshal.FreeHGlobal(fileName);
                return num;
            }
            return (int) MediaInfo_Open(this.Handle, FileName);
        }

        public string Option(string Option_)
        {
            return this.Option(Option_, "");
        }

        public string Option(string Option, string Value)
        {
            if (this.MustUseAnsi)
            {
                IntPtr option = Marshal.StringToHGlobalAnsi(Option);
                IntPtr ptr2 = Marshal.StringToHGlobalAnsi(Value);
                string str = Marshal.PtrToStringAnsi(MediaInfoA_Option(this.Handle, option, ptr2));
                Marshal.FreeHGlobal(option);
                Marshal.FreeHGlobal(ptr2);
                return str;
            }
            return Marshal.PtrToStringUni(MediaInfo_Option(this.Handle, Option, Value));
        }

        public int State_Get()
        {
            return (int) MediaInfo_State_Get(this.Handle);
        }
    }
}

