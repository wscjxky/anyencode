namespace anyEncoder
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class IniFiles
    {
        public string FileName;

        public IniFiles(string AFileName)
        {
            FileInfo info = new FileInfo(AFileName);
            if (!info.Exists)
            {
                StreamWriter writer = new StreamWriter(AFileName, false, Encoding.Default);
                try
                {
                    writer.Write("[inifile]");
                    writer.Close();
                }
                catch
                {
                    func.addLog("Ini文件不存在：" + AFileName, config.errlog);
                }
            }
            this.FileName = info.FullName;
        }

        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, this.FileName);
        }

        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, this.FileName))
            {
                throw new ApplicationException("无法清除Ini文件中的Section");
            }
        }

        ~IniFiles()
        {
            this.UpdateFile();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        private void GetStringsFromBuffer(byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int index = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - index) > 0))
                    {
                        string str = Encoding.GetEncoding(0).GetString(Buffer, index, i - index);
                        Strings.Add(str);
                        index = i + 1;
                    }
                }
            }
        }

        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(this.ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return Default;
            }
        }

        public int ReadInteger(string Section, string Ident, int Default)
        {
            string str = this.ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return Default;
            }
        }

        public void ReadSection(string Section, StringCollection Idents)
        {
            byte[] retVal = new byte[0x4000];
            int bufLen = GetPrivateProfileString(Section, null, null, retVal, retVal.GetUpperBound(0), this.FileName);
            this.GetStringsFromBuffer(retVal, bufLen, Idents);
        }

        public void ReadSections(StringCollection SectionList)
        {
            byte[] retVal = new byte[0xffff];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, retVal, retVal.GetUpperBound(0), this.FileName);
            this.GetStringsFromBuffer(retVal, bufLen, SectionList);
        }

        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection idents = new StringCollection();
            this.ReadSection(Section, idents);
            Values.Clear();
            foreach (string str in idents)
            {
                Values.Add(str, this.ReadString(Section, str, ""));
            }
        }

        public string ReadString(string Section, string Ident, string Default)
        {
            byte[] retVal = new byte[0xffff];
            int length = GetPrivateProfileString(Section, Ident, Default, retVal, retVal.GetUpperBound(0), this.FileName);
            return Encoding.GetEncoding(0).GetString(retVal).Substring(0, length).Trim().Replace("\0", "");
        }

        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, this.FileName);
        }

        public bool ValueExists(string Section, string Ident)
        {
            StringCollection idents = new StringCollection();
            this.ReadSection(Section, idents);
            return (idents.IndexOf(Ident) > -1);
        }

        public void WriteBool(string Section, string Ident, bool Value)
        {
            this.WriteString(Section, Ident, Convert.ToString(Value));
        }

        public void WriteInteger(string Section, string Ident, int Value)
        {
            this.WriteString(Section, Ident, Value.ToString());
        }

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, this.FileName))
            {
                func.addLog("写Ini文件出错：" + Value, config.errlog);
            }
        }
    }
}

