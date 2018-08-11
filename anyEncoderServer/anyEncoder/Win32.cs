namespace anyEncoder
{
    using System;
    using System.Runtime.InteropServices;

    public class Win32
    {
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        public const int WM_DEVICECHANGE = 0x219;

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            public short dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        public class DEV_BROADCAST_DEVICEINTERFACE1
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x10, ArraySubType=UnmanagedType.U1)]
            public byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x80)]
            public char[] dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
        }
    }
}

