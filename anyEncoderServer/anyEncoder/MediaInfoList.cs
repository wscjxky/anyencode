namespace anyEncoder
{
    using System;
    using System.Runtime.InteropServices;

    public class MediaInfoList
    {
        private IntPtr Handle = MediaInfoList_New();

        public void Close()
        {
            this.Close(-1);
        }

        public void Close(int FilePos)
        {
            MediaInfoList_Close(this.Handle, (IntPtr) FilePos);
        }

        public int Count_Get(int FilePos, StreamKind StreamKind)
        {
            return this.Count_Get(FilePos, StreamKind, -1);
        }

        public int Count_Get(int FilePos, StreamKind StreamKind, int StreamNumber)
        {
            return (int) MediaInfoList_Count_Get(this.Handle, (IntPtr) FilePos, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber);
        }

        ~MediaInfoList()
        {
            MediaInfoList_Delete(this.Handle);
        }

        public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter)
        {
            return this.Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text);
        }

        public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter)
        {
            return this.Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
        }

        public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {
            return Marshal.PtrToStringUni(MediaInfoList_GetI(this.Handle, (IntPtr) FilePos, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, (IntPtr) Parameter, (IntPtr) ((long) KindOfInfo)));
        }

        public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
        {
            return this.Get(FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
        }

        public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
        {
            return Marshal.PtrToStringUni(MediaInfoList_Get(this.Handle, (IntPtr) FilePos, (IntPtr) ((long) StreamKind), (IntPtr) StreamNumber, Parameter, (IntPtr) ((long) KindOfInfo), (IntPtr) ((long) KindOfSearch)));
        }

        public string Inform(int FilePos)
        {
            return Marshal.PtrToStringUni(MediaInfoList_Inform(this.Handle, (IntPtr) FilePos, IntPtr.Zero));
        }

        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfoList_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_New();
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);
        public void Open(string FileName)
        {
            this.Open(FileName, InfoFileOptions.FileOption_Nothing);
        }

        public int Open(string FileName, InfoFileOptions Options)
        {
            return (int) MediaInfoList_Open(this.Handle, FileName, (IntPtr) ((long) Options));
        }

        public string Option(string Option_)
        {
            return this.Option(Option_, "");
        }

        public string Option(string Option, string Value)
        {
            return Marshal.PtrToStringUni(MediaInfoList_Option(this.Handle, Option, Value));
        }

        public int State_Get()
        {
            return (int) MediaInfoList_State_Get(this.Handle);
        }
    }
}

