namespace anyEncoder
{
    using ICSharpCode.SharpZipLib.Zip;
    using office2pdf;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Timers;
    using System.Windows.Forms;

    public class DocForm : Form
    {
        private string addtime;
        private IContainer components = null;
        private IniFiles configini = new IniFiles(Application.StartupPath + @"\config.ini");
        private int errcount;
        private string fcode;
        private string file_in;
        private string file_log;
        private string file_out;
        private string file_pdf;
        private int filesize;
        private string fname;
        private string id;
        private string imgdir;
        private bool iswmark;
        private TextBox logbox;
        private string outfilename;
        private IniFiles statini;
        private System.Timers.Timer tmchk = new System.Timers.Timer(5000.0);
        private string truedir;

        public DocForm()
        {
            this.InitializeComponent();
            if (func.chkdog2() || func.chksn())
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                this.tmchk.Elapsed += new ElapsedEventHandler(this.chkjob);
                this.tmchk.AutoReset = true;
                this.tmchk.Enabled = true;
                this.statini = new IniFiles(Application.StartupPath + @"\log\stat_999.ini");
            }
        }

        private void AppendLog(string logstr)
        {
            if (this.logbox.Lines.Length > 0xfa0)
            {
                this.logbox.Clear();
            }
            this.logbox.AppendText(DateTime.Now.ToString() + " : " + logstr + Environment.NewLine);
        }

        private void AppendLog_en()
        {
            string str;
            try
            {
                FileStream stream = new FileStream(this.file_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
                stream.Close();
                stream.Dispose();
            }
            catch
            {
                str = "";
            }
            if (!string.IsNullOrEmpty(str))
            {
                this.logbox.AppendText(str + Environment.NewLine);
            }
        }

        public void chkjob(object source, ElapsedEventArgs e)
        {
            this.tmchk.Enabled = false;
            int num = this.configini.ReadInteger("encoder", "maxerr", 3);
            DataView defaultView = null;
            try
            {
                defaultView = Conn.GetDataSet("select top 1 * from ov_files where stat=0 and isdel=0 and filetype=1 and errcount<" + num + "order by id asc").Tables[0].DefaultView;
            }
            catch (Exception exception)
            {
                this.tmchk.Enabled = true;
                this.AppendLog(exception.Message.ToString());
                return;
            }
            if (defaultView.Count > 0)
            {
                this.id = defaultView[0]["id"].ToString();
                Conn.ExecuteNonQuery("update ov_files set stat=2 where id=" + this.id);
                this.errcount = (int) defaultView[0]["errcount"];
                this.truedir = defaultView[0]["truedir"].ToString();
                this.filesize = func.GetInt(defaultView[0]["filesize"].ToString());
                this.addtime = this.truedir + defaultView[0]["truedir"].ToString();
                this.file_in = defaultView[0]["truedir"].ToString() + @"tmpfiles\" + defaultView[0]["filename"].ToString();
                this.imgdir = this.truedir + defaultView[0]["filedir"].ToString() + @"\img\";
                this.fname = defaultView[0]["filename"].ToString();
                this.outfilename = defaultView[0]["outfilename"].ToString();
                this.fcode = defaultView[0]["filecode"].ToString();
                this.file_out = this.truedir + defaultView[0]["filedir"].ToString() + @"\" + this.outfilename;
                this.file_log = Application.StartupPath + @"\log\encoder_" + this.outfilename + ".log";
                try
                {
                    if (!Directory.Exists(this.imgdir))
                    {
                        Directory.CreateDirectory(this.imgdir);
                    }
                }
                catch
                {
                }
                this.AppendLog("开始处理文件：" + this.fname + ",");
                this.statini.WriteString("encoder", "id", this.id);
                this.statini.WriteString("encoder", "stat", "1");
                this.statini.WriteString("encoder", "logfile", this.file_log);
                this.statini.WriteString("encoder", "statmsg", "正在处理文档");
                this.run();
            }
            else
            {
                this.tmchk.Enabled = true;
                this.AppendLog("转换队列为空");
                this.statini.WriteString("encoder", "stat", "0");
                this.statini.WriteString("encoder", "statmsg", "");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void enfile(string infile, string outfile)
        {
            FileStream input = new FileStream(infile, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            BinaryReader reader = new BinaryReader(input);
            BinaryWriter writer = new BinaryWriter(output);
            reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            writer.BaseStream.Seek(0L, SeekOrigin.End);
            writer.Write(reader.ReadByte());
            int num = int.Parse(func.GetRandomInt(3));
            byte[] buffer = new byte[num];
            Random random = new Random();
            for (int i = 0; i < num; i++)
            {
                buffer[i] = (byte) random.Next(0xff);
            }
            writer.Write(this.GetBigEndianBytesFromDouble((double) num));
            writer.Write(buffer);
            reader.BaseStream.Seek(3L, SeekOrigin.Begin);
            writer.Write(reader.ReadBytes((int) reader.BaseStream.Length));
            reader.Close();
            writer.Close();
        }

        private byte[] GetBigEndianBytesFromDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private void InitializeComponent()
        {
            this.logbox = new TextBox();
            base.SuspendLayout();
            this.logbox.Location = new Point(0, 0);
            this.logbox.Multiline = true;
            this.logbox.Name = "logbox";
            this.logbox.ScrollBars = ScrollBars.Vertical;
            this.logbox.Size = new Size(0x1d5, 270);
            this.logbox.TabIndex = 1;
            base.ClientSize = new Size(0x1d5, 270);
            base.Controls.Add(this.logbox);
            base.Name = "DocForm";
            this.Text = "文档转换";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void run()
        {
            ProcessStartInfo info;
            Process process;
            this.file_pdf = this.file_in;
            FileInfo info2 = new FileInfo(this.file_in);
            string str = info2.Extension.ToLower();
            if (str != ".pdf")
            {
                this.file_pdf = this.file_in + ".pdf";
                if (".jpg|.png|.gif|.tif|.bmp|.psd|.pcx|.zip".IndexOf(str) >= 0)
                {
                    string str2 = this.file_in;
                    if (str == ".zip")
                    {
                        string str3 = Application.StartupPath + @"\zipimg\";
                        if (Directory.Exists(str3))
                        {
                            try
                            {
                                func.DelDir(str3);
                            }
                            catch
                            {
                            }
                        }
                        if (this.UnZip(this.file_in, str3))
                        {
                            str2 = Application.StartupPath + @"\zipimg";
                        }
                    }
                    ProcessStartInfo info3 = new ProcessStartInfo();
                    info3.FileName = Application.StartupPath + @"\img2pdf\img2pdf.exe";
                    info3.Arguments = " -o \"" + this.file_pdf + "\" \"" + str2 + "\"";
                    info3.CreateNoWindow = true;
                    info3.UseShellExecute = false;
                    info = info3;
                    this.AppendLog("开始转换图片为pdf：" + info.FileName + " " + info.Arguments);
                    this.statini.WriteString("encoder", "enmsg", "正在处理图片");
                    process = Process.Start(info);
                    process.WaitForExit(0x7530);
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                else
                {
                    int tout = this.configini.ReadInteger("doc", "d_outtime", 300);
                    try
                    {
                        new convert(tout).topdf(this.file_in, this.file_pdf);
                        this.AppendLog("开始转换为pdf文件：" + this.file_pdf);
                        this.statini.WriteString("encoder", "enmsg", "正在处理文档");
                    }
                    catch (Exception exception)
                    {
                        this.AppendLog("错误：" + exception.Message.ToString());
                        this.statini.WriteString("encoder", "enmsg", "错误：" + exception.Message.ToString());
                    }
                }
            }
            if (!File.Exists(this.file_pdf))
            {
                this.tmchk.Enabled = true;
                this.taskfail(null, null);
            }
            if (!Directory.Exists(this.file_out))
            {
                Directory.CreateDirectory(this.file_out);
            }
            int num2 = this.configini.ReadInteger("doc", "d_flashversion", 9);
            bool flag = this.configini.ReadBool("doc", "d_dislink", false);
            bool flag2 = this.configini.ReadBool("doc", "d_enzip", true);
            bool flag3 = this.configini.ReadBool("doc", "d_shapes", false);
            bool flag4 = this.configini.ReadBool("doc", "d_usefont", false);
            bool flag5 = this.configini.ReadBool("doc", "d_poly2bitmap", false);
            int num3 = this.configini.ReadInteger("doc", "d_dpi", 0x48);
            string path = this.configini.ReadString("doc", "d_pdflogo", "");
            if (File.Exists(path))
            {
                string str5 = this.file_pdf + "_logo.pdf";
                ProcessStartInfo info4 = new ProcessStartInfo();
                info4.FileName = Application.StartupPath + @"\pdftk.exe";
                info4.Arguments = " \"" + this.file_pdf + "\" stamp \"" + path + "\" output \"" + str5 + "\"";
                info4.CreateNoWindow = true;
                info4.UseShellExecute = false;
                info = info4;
                this.AppendLog("开始添加水印：" + info.FileName + " " + info.Arguments);
                this.statini.WriteString("encoder", "enmsg", "正在添加水印");
                process = Process.Start(info);
                process.WaitForExit(0x2bf20);
                if (!process.HasExited)
                {
                    process.Kill();
                }
                else if (File.Exists(str5))
                {
                    File.Delete(this.file_pdf);
                    File.Move(str5, this.file_pdf);
                }
                this.iswmark = true;
            }
            else
            {
                this.iswmark = false;
            }
            if (num3 <= 1)
            {
                num3 = 0x48;
            }
            string str6 = "\"" + this.file_pdf + "\" \"" + this.file_out + "\\paper%.swf\" -s flashversion=" + num2.ToString();
            if (flag2)
            {
                str6 = str6 + " -s enablezlib";
            }
            if (flag)
            {
                str6 = str6 + " -s disablelinks";
            }
            if (flag4)
            {
                str6 = str6 + " -s storeallcharacters";
            }
            if (flag3 || (this.errcount > 0))
            {
                str6 = str6 + " -S";
            }
            if (flag5 && (".ppt|.pptx|.pptm|.pot|.potx".IndexOf(str) >= 0))
            {
                str6 = str6 + " -s poly2bitmap";
            }
            if (num3 != 0x48)
            {
                str6 = str6 + " -s zoom=" + num3;
            }
            if (Directory.Exists(@"C:\xpdf\xpdf-chinese-simplified"))
            {
                str6 = str6 + @" -s languagedir=C:\xpdf\xpdf-chinese-simplified";
            }
            ProcessCaller caller2 = new ProcessCaller(this);
            caller2.FileName = Application.StartupPath + @"\swftools\pdf2swf.exe";
            caller2.Arguments = str6;
            ProcessCaller caller = caller2;
            caller.StdErrReceived += new DataReceivedHandler(this.writeStreamInfo);
            caller.StdOutReceived += new DataReceivedHandler(this.writeStreamInfo);
            caller.Completed += new EventHandler(this.run_getimg);
            caller.Cancelled += new EventHandler(this.taskfail);
            caller.tsn = this.fcode;
            this.statini.WriteString("encoder", "enmsg", "正在生成swf文件");
            this.AppendLog("开始转换为flash文件：" + caller.FileName + " " + caller.Arguments);
            caller.Start();
        }

        private void run_getimg(object sender, EventArgs e)
        {
            string path = this.file_out + @"\paper1.swf";
            string str2 = "";
            if (File.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                for (int i = 1; i < 7; i++)
                {
                    string str3 = string.Concat(new object[] { this.file_out, @"\paper", i, ".swf" });
                    string str4 = string.Concat(new object[] { this.imgdir, this.fcode, "_", i, "_b.png" });
                    string thumbnailPath = string.Concat(new object[] { this.imgdir, this.fcode, "_", i, ".jpg" });
                    if (File.Exists(str3))
                    {
                        string str6;
                        if (func.Right(this.file_in, 4).ToLower() != ".pdf")
                        {
                            str6 = this.file_in + ".pdf";
                        }
                        else
                        {
                            str6 = this.file_in;
                        }
                        startInfo.FileName = Application.StartupPath + @"\pdfdraw.exe";
                        startInfo.Arguments = string.Concat(new object[] { "-r 150 -o \"", str4, "\" \"", str6, "\" ", i, "-", i });
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = false;
                        this.AppendLog("开始截图：" + startInfo.FileName + " " + startInfo.Arguments);
                        Process.Start(startInfo).WaitForExit();
                        if (File.Exists(str4))
                        {
                            func.imgResize(str4, thumbnailPath, 160, 160, "AUTO");
                            File.Delete(str4);
                        }
                        if (string.IsNullOrEmpty(str2))
                        {
                            str2 = i.ToString();
                        }
                        else
                        {
                            str2 = str2 + "," + i.ToString();
                        }
                    }
                }
                DirectoryInfo info2 = new DirectoryInfo(this.file_out);
                int length = info2.GetFiles().Length;
                FileInfo[] files = info2.GetFiles();
                this.AppendLog("开始加密数据");
                for (int j = 0; j < files.Length; j++)
                {
                    string fullName = files[j].FullName;
                    string outfile = files[j].FullName.Replace(@"\paper", @"\");
                    if (func.Right(fullName, 4).ToLower() == ".swf")
                    {
                        this.AppendLog("正在加密swf：" + j);
                        this.statini.WriteString("encoder", "enmsg", "正在加密swf:" + j);
                        this.enfile(fullName, outfile);
                        if (File.Exists(outfile))
                        {
                            File.Delete(fullName);
                        }
                        else
                        {
                            File.Move(fullName, outfile);
                        }
                    }
                }
                if (this.configini.ReadBool("doc", "d_pdftxt", false))
                {
                    startInfo.FileName = Application.StartupPath + @"\pdf2text\pdftotext.exe";
                    startInfo.Arguments = " \"" + this.file_pdf + "\" \"" + this.file_out + @"\" + this.fname + ".txt\" -l 5";
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    this.AppendLog("开始获取摘要：" + startInfo.FileName + " " + startInfo.Arguments);
                    this.statini.WriteString("encoder", "enmsg", "正在获取摘要");
                    Process.Start(startInfo).WaitForExit();
                }
                Conn.ExecuteNonQuery(string.Concat(new object[] { "update ov_files set stat=1,sendok=0,autoimg='", str2, "',times=", length, " where id=", this.id }));
                Zip(this.file_in, this.file_out + @"\" + this.fname + ".zip", 0x100000);
                File.Move(this.file_in, this.file_out + @"\" + this.fname);
                try
                {
                    string str9;
                    if (func.Right(this.file_in, 4).ToLower() != ".pdf")
                    {
                        str9 = this.file_out + @"\" + this.fname + ".pdf";
                        File.Move(this.file_pdf, str9);
                    }
                    else
                    {
                        str9 = this.file_out + @"\" + this.fname;
                    }
                    if (this.configini.ReadBool("doc", "d_outimg", false))
                    {
                        startInfo.FileName = Application.StartupPath + @"\pdfdraw.exe";
                        startInfo.Arguments = "-r 216 -o \"" + this.file_out + "\\%d.png\" \"" + str9 + "\"";
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = false;
                        this.AppendLog("正在生成ipad图片：" + startInfo.FileName + " " + startInfo.Arguments);
                        Process process = Process.Start(startInfo);
                    }
                    else if (this.configini.ReadBool("doc", "d_delpdf", false))
                    {
                        try
                        {
                            File.Delete(str9);
                        }
                        catch
                        {
                        }
                    }
                    if (this.configini.ReadBool("doc", "d_deldoc", false))
                    {
                        try
                        {
                            File.Delete(this.file_out + @"\" + this.fname);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
                this.AppendLog("转换任务成功。");
                this.tmchk.Enabled = true;
            }
            else
            {
                this.taskfail(null, null);
                this.tmchk.Enabled = true;
                if (this.file_in != this.file_pdf)
                {
                    try
                    {
                        File.Delete(this.file_pdf);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void taskfail(object sender, EventArgs e)
        {
            Conn.ExecuteNonQuery("update ov_files set stat=0,errcount=errcount+1 where id=" + this.id);
        }

        public bool UnZip(string sourceFile, string destinationFile)
        {
            bool flag = true;
            try
            {
                ZipEntry entry;
                ZipInputStream stream = new ZipInputStream(File.OpenRead(sourceFile));
                while ((entry = stream.GetNextEntry()) != null)
                {
                    bool flag2;
                    string directoryName = Path.GetDirectoryName(destinationFile);
                    if (!(Path.GetFileName(entry.Name) != string.Empty))
                    {
                        continue;
                    }
                    if (entry.CompressedSize == 0L)
                    {
                        break;
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile + entry.Name));
                    FileStream stream2 = File.Create(destinationFile + entry.Name);
                    int count = 0x800;
                    byte[] buffer = new byte[0x800];
                    goto Label_00C8;
                Label_0097:
                    count = stream.Read(buffer, 0, buffer.Length);
                    if (count > 0)
                    {
                        stream2.Write(buffer, 0, count);
                    }
                    else
                    {
                        goto Label_00CC;
                    }
                Label_00C8:
                    flag2 = true;
                    goto Label_0097;
                Label_00CC:
                    stream2.Close();
                    Application.DoEvents();
                }
                stream.Close();
            }
            catch (ZipException)
            {
                flag = false;
            }
            return flag;
        }

        private void writeStreamInfo(object sender, anyEncoder.DataReceivedEventArgs e)
        {
            string text = e.Text;
            this.logbox.AppendText(text + Environment.NewLine);
            if (text.IndexOf("Performing cleanups") > -1)
            {
            }
        }

        public static void Zip(string srcFile, string dstFile, int bufferSize)
        {
            using (FileStream stream = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream stream2 = new FileStream(dstFile, FileMode.Create, FileAccess.Write))
                {
                    using (ZipOutputStream stream3 = new ZipOutputStream(stream2))
                    {
                        int num;
                        byte[] buffer = new byte[bufferSize];
                        ZipEntry entry = new ZipEntry(Path.GetFileName(srcFile));
                        stream3.PutNextEntry(entry);
                        do
                        {
                            num = stream.Read(buffer, 0, buffer.Length);
                            stream3.Write(buffer, 0, num);
                        }
                        while (num > 0);
                        stream3.Flush();
                    }
                }
            }
        }
    }
}

