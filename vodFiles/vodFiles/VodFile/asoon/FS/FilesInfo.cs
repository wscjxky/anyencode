namespace VodFile
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FilesInfo
    {
        public string fileName;
        public string filePath;
        public int fileSize;
        public string fileContentType;
        public string fileExt;
        public string oldname;
        public string filecode;
        public string outfilename;
        public string stat;
        public string addip;
        public string addtime;
        public string filedir;
    }
}

