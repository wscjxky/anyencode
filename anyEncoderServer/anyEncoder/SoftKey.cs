namespace anyEncoder
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class SoftKey
    {
        [DllImport("kernel32.dll", EntryPoint="RtlMoveMemory")]
        public static extern void CopyByteToString(StringBuilder pDest, byte[] pSource, int ByteLenr);
        [DllImport("kernel32.dll", EntryPoint="RtlMoveMemory")]
        public static extern void CopyStringToByte(byte[] pDest, string pSourceg, int ByteLenr);
        public void DeCode(byte[] inb, byte[] outb, string Key)
        {
            int num;
            uint[] numArray = new uint[0x10];
            uint num2 = 0x9e3779b9;
            uint num3 = 0xc6ef3720;
            int length = Key.Length;
            int index = 0;
            for (num = 1; num <= length; num += 2)
            {
                string s = Key.Substring(num - 1, 2);
                numArray[index] = this.HexToInt(s);
                index++;
            }
            uint num6 = 0;
            uint num7 = 0;
            uint num8 = 0;
            uint num9 = 0;
            for (num = 0; num <= 3; num++)
            {
                num6 = (numArray[num] << (num * 8)) | num6;
                num7 = (numArray[num + 4] << (num * 8)) | num7;
                num8 = (numArray[(num + 4) + 4] << (num * 8)) | num8;
                num9 = (numArray[((num + 4) + 4) + 4] << (num * 8)) | num9;
            }
            uint num10 = 0;
            uint num11 = 0;
            for (num = 0; num <= 3; num++)
            {
                uint num12 = inb[num];
                num10 = (num12 << (num * 8)) | num10;
                num12 = inb[num + 4];
                num11 = (num12 << (num * 8)) | num11;
            }
            num = 0x20;
            while (num-- > 0)
            {
                num11 -= (((num10 << 4) + num8) ^ (num10 + num3)) ^ ((num10 >> 5) + num9);
                num10 -= (((num11 << 4) + num6) ^ (num11 + num3)) ^ ((num11 >> 5) + num7);
                num3 -= num2;
            }
            for (num = 0; num <= 3; num++)
            {
                outb[num] = Convert.ToByte((uint) ((num10 >> (num * 8)) & 0xff));
                outb[num + 4] = Convert.ToByte((uint) ((num11 >> (num * 8)) & 0xff));
            }
        }

        public void EnCode(byte[] inb, byte[] outb, string Key)
        {
            int num;
            uint[] numArray = new uint[0x10];
            uint num2 = 0x9e3779b9;
            uint num3 = 0;
            int length = Key.Length;
            int index = 0;
            for (num = 1; num <= length; num += 2)
            {
                string s = Key.Substring(num - 1, 2);
                numArray[index] = this.HexToInt(s);
                index++;
            }
            uint num6 = 0;
            uint num7 = 0;
            uint num8 = 0;
            uint num9 = 0;
            for (num = 0; num <= 3; num++)
            {
                num6 = (numArray[num] << (num * 8)) | num6;
                num7 = (numArray[num + 4] << (num * 8)) | num7;
                num8 = (numArray[(num + 4) + 4] << (num * 8)) | num8;
                num9 = (numArray[((num + 4) + 4) + 4] << (num * 8)) | num9;
            }
            uint num10 = 0;
            uint num11 = 0;
            for (num = 0; num <= 3; num++)
            {
                uint num12 = inb[num];
                num10 = (num12 << (num * 8)) | num10;
                num12 = inb[num + 4];
                num11 = (num12 << (num * 8)) | num11;
            }
            for (num = 0x20; num > 0; num--)
            {
                num3 = num2 + num3;
                num10 += (((num11 << 4) + num6) ^ (num11 + num3)) ^ ((num11 >> 5) + num7);
                num11 += (((num10 << 4) + num8) ^ (num10 + num3)) ^ ((num10 >> 5) + num9);
            }
            for (num = 0; num <= 3; num++)
            {
                outb[num] = Convert.ToByte((uint) ((num10 >> (num * 8)) & 0xff));
                outb[num + 4] = Convert.ToByte((uint) ((num11 >> (num * 8)) & 0xff));
            }
        }

        private uint HexToInt(string s)
        {
            string[] strArray = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            s = s.ToUpper();
            int num = 1;
            int num2 = 0;
            for (int i = s.Length; i > 0; i--)
            {
                string str = s.Substring(i - 1, 1);
                int num4 = 0;
                for (int j = 0; j < 0x10; j++)
                {
                    if (str == strArray[j])
                    {
                        num4 = j;
                    }
                }
                num2 += num4 * num;
                num *= 0x10;
            }
            return (uint) num2;
        }

        [DllImport("kernel32.dll")]
        public static extern int lstrlenA(string InString);
        public string StrDec(string InString, string Key)
        {
            int num;
            int num2;
            byte[] inb = new byte[8];
            byte[] outb = new byte[8];
            int length = InString.Length;
            if (length < 0x10)
            {
                num2 = 0x10;
            }
            num2 = length / 2;
            byte[] buffer3 = new byte[num2];
            byte[] array = new byte[num2];
            int index = 0;
            for (num = 1; num <= length; num += 2)
            {
                string s = InString.Substring(num - 1, 2);
                buffer3[index] = Convert.ToByte(this.HexToInt(s));
                index++;
            }
            buffer3.CopyTo(array, 0);
            for (num = 0; num <= (num2 - 8); num += 8)
            {
                index = 0;
                while (index < 8)
                {
                    inb[index] = buffer3[index + num];
                    index++;
                }
                this.DeCode(inb, outb, Key);
                for (index = 0; index < 8; index++)
                {
                    array[index] = outb[index + num];
                }
            }
            StringBuilder pDest = new StringBuilder("", num2);
            CopyByteToString(pDest, array, num2);
            return pDest.ToString();
        }

        public string StrEnc(string InString, string Key)
        {
            int num;
            int num2;
            byte[] inb = new byte[8];
            byte[] outb = new byte[8];
            int byteLenr = lstrlenA(InString) + 1;
            if (byteLenr < 8)
            {
                num2 = 8;
            }
            else
            {
                num2 = byteLenr;
            }
            byte[] pDest = new byte[num2];
            byte[] array = new byte[num2];
            CopyStringToByte(pDest, InString, byteLenr);
            pDest.CopyTo(array, 0);
            for (num = 0; num <= (num2 - 8); num += 8)
            {
                int index = 0;
                while (index < 8)
                {
                    inb[index] = pDest[index + num];
                    index++;
                }
                this.EnCode(inb, outb, Key);
                for (index = 0; index < 8; index++)
                {
                    array[index] = outb[index + num];
                }
            }
            string str = "";
            for (num = 0; num <= (num2 - 1); num++)
            {
                str = str + array[num].ToString("X2");
            }
            return str;
        }
    }
}

