

namespace anyEncoder
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Web;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Timers;
    using System.Windows.Forms;
    using SeasideResearch.LibCurlNet;
    using System.Net;
    using Qiniu.Conf;
    using Qiniu.IO;
    using Qiniu.RS;
    using System.Text;
    using System.Data.SqlClient;
    using System.Threading;
    using Qiniu.IO.Resumable;
    using Qiniu.RPC;

    class Upoader
    {
        private string _bucket;
        private string _putfileRet;
        //[[ljj@2016-10-11: 增加切片处理指令
        private string _fops;
        private string pipeline;
        public TextBox logbox;

        //]]
        public void init()
        {
            Qiniu.Conf.Config.ACCESS_KEY = "HZQxHGBv5-MZ2HtUJTbBKQ1VDOZTTZHqdq-XBS4_";
            Qiniu.Conf.Config.SECRET_KEY = "PqjYexTSybWtk6PQZE1-V0J2JsiUm_v22l7lryZE";
            _bucket = "vodresource";
            _putfileRet = "";


        }
        private void AppendLog(string logstr)
        {
            if (this.logbox.Lines.Length > 0xfa0)
            {
                this.logbox.Clear();
            }
            this.logbox.AppendText(DateTime.Now.ToString() + " : " + logstr + Environment.NewLine);
        }
        private static void extra_Notify(object sender, PutNotifyEvent e)
        {
            System.DateTime currentTime = new System.DateTime();
        
            string strlogfilename = string.Format("{0}{1}{2}_{3}{4}{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);

            new IniFiles(Application.StartupPath + @"\"+strlogfilename+"SUCCESS.ini").WriteString("system", "startlog", e.BlkIdx.ToString()+"----" + e.BlkSize.ToString());
            
            Console.WriteLine();
            //  e.Ret.offset.ToString();
            //2015年三月23日，提交了


        }

        private static void extra_NotifyErr(object sender, PutNotifyErrorEvent e)
        {

            System.DateTime currentTime = new System.DateTime();

            string strlogfilename = string.Format("{0}{1}{2}_{3}{4}{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);

            new IniFiles(Application.StartupPath + @"\" + strlogfilename + "ERROR.ini").WriteString("system", "startlog", e.BlkIdx.ToString() + e.BlkSize.ToString());

        }
        public string PutFile(string key, string fname)
        {
            _putfileRet = "";
            var policy = new PutPolicy(_bucket,7200);
            string upToken = policy.Token();
            //PutExtra extra = new PutExtra();
            //IOClient client = new IOClient();
            //PutRet ret = client.PutFile(upToken, key, fname, extra);
            Settings setting = new Settings();
            ResumablePutExtra extra = new ResumablePutExtra();
            extra.Notify += new EventHandler<PutNotifyEvent>(extra_Notify);
            extra.NotifyErr += new EventHandler<PutNotifyErrorEvent>(extra_NotifyErr);
                        ResumablePut client = new ResumablePut(setting, extra);

            CallRet ret = client.PutFile(upToken, fname, key);
            if (!ret.OK)
            {
                _putfileRet = string.Format("fsizelimit={0},deadline={1},{2},{3},{4}", policy.FsizeLimit, policy.Deadline,policy.CallBackBody,policy.ReturnBody) + ret.Response + "***exception:" + ret.Exception.ToString();
            }
            return _putfileRet;
        }
    }

    public class encoder
    {
        private string addtime;
        private IniFiles configini = new IniFiles(Application.StartupPath + @"\config.ini");
        private ProcessPriorityClass cpu_pri;
        private int cpuCount = 1;
        private int e_aBit;
        private int e_FrameRate;
        private int e_Height;
        private string e_MobileVideo;
        private int e_vBit;
        private int e_Width;
        private int errcount = 0;
        private string fcode;
        private string file_ff;
        private string file_in;
        private string file_log;
        private string file_me;
        private string file_out;
        private string file_swf;
        private int filesize;
        private string fname;
        public Form formobj;
        private string id;
        private int trancode;

        private string weburl;
        private string imgdir;
        private string isrec;
        public TextBox logbox;
        private int m_Bit;
        private string m_CodecID_Info;
        private int m_Duration;
        private string m_Format;
        private int m_FrameRate;
        private int m_Height;
        private bool m_isAudio;
        private bool m_isf4v;
        private string m_Tagged_Application;
        private int m_Width;
        private string outfilename;
        private string outfiletype = "mp4";
        private ProcessCaller p_encoder;
        private string runtype;
        private IniFiles statini;
        public int taskid;
        private System.Timers.Timer tmchk = new System.Timers.Timer(5000.0);
        private System.Timers.Timer tmchkdel = new System.Timers.Timer(2000.0);
        private string truedir;
        private Object theLock = new Object();

        public encoder(int tid)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.tmchk.Elapsed += new ElapsedEventHandler(this.chkjob);
            this.tmchk.AutoReset = true;
            this.tmchk.Enabled = true;
            this.tmchkdel.Elapsed += new ElapsedEventHandler(this.chkjobdel);
            this.tmchkdel.AutoReset = true;
            this.tmchkdel.Enabled = true;
            this.taskid = tid;
            switch (this.configini.ReadString("encoder", "cpu", "").Replace("\0", ""))
            {
                case "最高":
                    this.cpu_pri = ProcessPriorityClass.RealTime;
                    break;

                case "高":
                    this.cpu_pri = ProcessPriorityClass.High;
                    break;

                case "较高":
                    this.cpu_pri = ProcessPriorityClass.AboveNormal;
                    break;

                case "标准":
                    this.cpu_pri = ProcessPriorityClass.Normal;
                    break;

                case "较低":
                    this.cpu_pri = ProcessPriorityClass.BelowNormal;
                    break;

                case "低":
                    this.cpu_pri = ProcessPriorityClass.Idle;
                    break;

                default:
                    this.cpu_pri = ProcessPriorityClass.BelowNormal;
                    break;
            }
            if (this.cpuCount <= 0)
            {
                this.cpuCount = 4;
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

        public void cancel()
        {
            try
            {
                this.p_encoder.Cancel();
                File.Delete(this.file_ff);
                File.Delete(this.file_me);
            }
            catch
            {
            }
        }

        public void startlog(string log)
        {
            new IniFiles(Application.StartupPath + @"\stat2.ini").WriteString("system", "startlog", log);
        }
        public void chkjob(object source, ElapsedEventArgs e)
        {

            this.tmchk.Enabled = false;
            int num = this.configini.ReadInteger("encoder", "maxerr", 3);
            DataView defaultView = null;
            this.errcount = 0;
     
            this.AppendLog("开始读取数据库");
            this.startlog("开始读取数据库");

            // 加锁
            lock (theLock)
            {
                try
                {
                    defaultView = Conn.GetDataSet("select top 1 * from ov_files where stat=0 and isdel=0 and filetype=0").Tables[0].DefaultView;
                    if (defaultView.Table.Rows.Count > 0)
                    {
                        //设置状态

                        Conn.ExecuteNonQuery("update ov_files set stat=1 where id=" + defaultView[0]["id"]);
                    }
                    else
                    {
                        this.tmchk.Enabled = true;
                        return;
                    }
                }
                catch (Exception exception)
                {
                    this.tmchk.Enabled = true;
                    this.AppendLog(exception.Message.ToString());
                    this.startlog(exception.Message.ToString());
                }
            }



            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strlogfilename = string.Format("{0}{1}{2}_{3}{4}{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
            this.statini = new IniFiles(string.Concat(new object[] { Application.StartupPath, @"\log\stat_", strlogfilename, ".ini" }));
            this.id = defaultView[0]["id"].ToString();
            this.weburl = defaultView[0]["webserver"].ToString();
            this.errcount = (int)defaultView[0]["errcount"];
            this.truedir = defaultView[0]["truedir"].ToString();
            this.filesize = func.GetInt(defaultView[0]["filesize"].ToString());
            this.addtime = this.truedir + defaultView[0]["truedir"].ToString();
            this.file_in = defaultView[0]["truedir"].ToString() + @"tmpfiles\" + defaultView[0]["filename"].ToString();
            if (!File.Exists(this.file_in))
            {
                //this.file_in = defaultView[0]["truedir"].ToString() + defaultView[0]["filedir"].ToString() + @"\" + defaultView[0]["filename"].ToString();
                this.file_in = defaultView[0]["truedir"].ToString() + defaultView[0]["filename"].ToString();
            }
            this.fname = defaultView[0]["filename"].ToString();
            this.outfilename = defaultView[0]["outfilename"].ToString();
            this.fcode = defaultView[0]["filecode"].ToString();
            this.imgdir = this.truedir + defaultView[0]["filedir"].ToString() + @"\img\";
            this.isrec = defaultView[0]["isrec"].ToString();
            this.file_out = this.truedir + defaultView[0]["filedir"].ToString() + @"\" + this.outfilename;
            this.file_swf = this.file_out.Replace(".flv", ".swf").Replace(".mp4", ".swf");
            this.file_log = Application.StartupPath + @"\log\encoder_" + this.outfilename + ".log";

            this.AppendLog("展示1：" + this.trancode.ToString());
            this.startlog("展示1：" + this.trancode.ToString() + strlogfilename);


            if (func.Right(this.outfilename, 4) == ".mp4")
            {
                this.outfiletype = "mp4";
            }
            this.file_ff = this.file_in + ".ff.flv";
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
            this.AppendLog("开始处理文件：" + this.fname + "," + func.ShowSize((float)this.filesize));
            this.statini.WriteString("encoder", "id", this.id);
            this.statini.WriteString("encoder", "stat", "1");
            this.statini.WriteString("encoder", "logfile", this.file_log);
            this.statini.WriteString("encoder", "statmsg", "处理中");
            this.run();
        }

        public void chkjobdel(object source, ElapsedEventArgs e)
        {
            string path = Application.StartupPath + @"\log\" + this.id + ".del";
            if (File.Exists(path))
            {
                this.cancel();
                try
                {
                    File.Delete(path);
                }
                catch
                {
                }
            }
        }

        public void close()
        {
            this.tmchk.Enabled = false;
            this.tmchk.Close();
            this.tmchk.Dispose();
            this.tmchkdel.Enabled = false;
            this.tmchkdel.Close();
            this.tmchkdel.Dispose();
            try
            {
                this.p_encoder.Cancel();
                File.Delete(this.file_ff);
                File.Delete(this.file_me);
            }
            catch
            {
            }
        }

        private Control FindControlByName(Control ctrl, string ctlName)
        {
            if (ctlName != " ")
            {
                foreach (Control control in ctrl.Controls)
                {
                    if (control.Name.Equals(ctlName))
                    {
                        return control;
                    }
                    Control control2 = this.FindControlByName(control, ctlName);
                    if (control2 != null)
                    {
                        return control2;
                    }
                }
            }
            return null;
        }

        private void getinfo(string filename)
        {
            MediaInfo info = new MediaInfo();
            info.Open(filename);
            this.m_FrameRate = func.GetInt(info.Get(StreamKind.Video, 0, "FrameRate").Replace(".000", ""));
            this.m_Bit = func.GetInt(info.Get(StreamKind.Video, 0, "BitRate").ToString());
            this.m_Width = func.GetInt(info.Get(StreamKind.Video, 0, "Width").ToString());
            this.m_Height = func.GetInt(info.Get(StreamKind.Video, 0, "Height").ToString());
            this.m_Duration = func.GetInt(info.Get(StreamKind.General, 0, "Duration").ToString());
            if (this.m_Duration <= 0)
            {
                this.m_Duration = func.GetInt(info.Get(StreamKind.Video, 0, "Duration").ToString());
            }
            if (this.m_Duration <= 0)
            {
                this.m_Duration = func.GetInt(info.Get(StreamKind.Audio, 0, "Duration").ToString());
            }
            this.m_Format = info.Get(StreamKind.General, 0, "Format").ToString();
            this.m_CodecID_Info = info.Get(StreamKind.Video, 0, "CodecID/Info").ToString();
            this.m_Tagged_Application = info.Get(StreamKind.Video, 0, "Tagged_Application").ToString();
            if (string.IsNullOrEmpty(info.Get(StreamKind.Video, 0, "FrameRate").ToString()) && string.IsNullOrEmpty(info.Get(StreamKind.Video, 0, "Format").ToString()))
            {
                this.m_isAudio = true;
            }
            else
            {
                this.m_isAudio = false;
            }
            if ((this.m_Format.ToLower() == "f4v") || (this.m_CodecID_Info.ToLower().IndexOf("f4v") > -1))
            {
                this.m_isf4v = true;
            }
            else
            {
                this.m_isf4v = false;
            }
            if (info.Get(StreamKind.Video, 0, "DisplayAspectRatio").ToString().Trim() == "1.778")
            {
                this.m_Height = (this.m_Width / 0x10) * 9;
            }
            this.AppendLog(string.Concat(new object[] { "获取文件信息：m_FrameRate:", this.m_FrameRate, ",m_Bit:", this.m_Bit, ",m_Width:", this.m_Width, ",m_Duration:", this.m_Duration, ",m_Format:", this.m_Format, ",m_CodecID_Info:", this.m_CodecID_Info }));
        }

        private void run()
        {
            this.getinfo(this.file_in);
            this.statini.WriteInteger("encoder", "duration", this.m_Duration / 0x3e8);
            if ((this.m_Format.ToLower() == "flash video") && this.configini.ReadBool("encoder", "skipflv", false))
            {
                File.Copy(this.file_in, this.file_out, true);
                this.AppendLog("检测到为flv文件格式，直接copy");
                this.statini.WriteString("encoder", "enmsg", "检测到为flv文件格式，直接copy");
                this.run_getimg(null, null);
            }
            else if (func.Right(this.file_in, 4).ToLower() == ".swf")
            {
                File.Copy(this.file_in, this.file_swf, true);
                this.file_out = this.file_swf;
                this.AppendLog("检测到为swf文件格式，直接copy");
                this.statini.WriteString("encoder", "enmsg", "检测到为swf文件格式，直接copy");
                this.run_getimg(null, null);
            }
            else if (((this.m_CodecID_Info.ToLower().IndexOf("Microsoft MPEG-4 V2".ToLower()) > -1) && !string.IsNullOrEmpty(this.m_CodecID_Info)) || (this.errcount > 100))
            {
                this.statini.WriteString("encoder", "enmsg", "转入mencoder预处理");
                this.AppendLog("转入mencoder预处理");
                this.run_mencoder();
            }
            else
            {
                this.run_ffmpeg(null, null);
            }
        }

        private void run_Canceled(object sender, EventArgs e)
        {
            if (!this.tmchk.Enabled)
            {
                this.tmchk.Enabled = true;
            }
            //////失败通知.告知网站转码失败//////  
            try
            {
                string filecode = this.fcode;
                string status = "0";
                WebClient client = new System.Net.WebClient();
                string uri = "http://www.jianpianzi.com/cloud/transcodeStatus?status=1&filecode=" + filecode;

                /* string filecode = this.fcode;
                 string status = "0";
                 WebClient client = new System.Net.WebClient();
                 string uri = "http://www.jianpianzi.com/cloud/transcodeStatus?status=1&filecode=" + filecode;
                 client.DownloadData(uri);
                 * */
            }
            catch (Exception ex)
            {
                ////////    
            }

        }

        private void run_ffmpeg(object sender, EventArgs e)
        {
            string str;
            string str2;
            int height;
            string str3;
            string str4 = "";
            string str5 = this.file_in;
            if (!(string.IsNullOrEmpty(this.file_me) || !File.Exists(this.file_me)))
            {
                str5 = this.file_me;
            }
            this.e_Height = func.GetInt(this.configini.ReadString("encoder", "height", ""));
            this.e_Width = func.GetInt(this.configini.ReadString("encoder", "width", ""));
            this.e_FrameRate = func.GetInt(this.configini.ReadString("encoder", "fps", ""));
            this.e_vBit = func.GetInt(this.configini.ReadString("encoder", "vbit", ""));
            this.e_aBit = func.GetInt(this.configini.ReadString("encoder", "abit", ""));
            this.e_MobileVideo = this.configini.ReadString("encoder", "mobilevideo", "");
            string path = this.configini.ReadString("logo", "logo_img", "");
            int @int = func.GetInt(this.configini.ReadString("logo", "logo_top", ""));
            int num3 = func.GetInt(this.configini.ReadString("logo", "logo_bot", ""));
            int num4 = func.GetInt(this.configini.ReadString("logo", "logo_left", ""));
            int num5 = func.GetInt(this.configini.ReadString("logo", "logo_right", ""));
            string str7 = this.configini.ReadString("logo", "logo_ali", "");
            string str8 = this.configini.ReadString("logo", "logo_type", "图片水印");
            if (this.e_FrameRate < 15)
            {
                this.e_FrameRate = 15;
            }
            if (str8 == "文字水印")
            {
                path = Application.StartupPath + @"\images\logotext.png";
            }
            if (this.m_Width > 0)
            {
                height = (this.m_Height * this.e_Width) / this.m_Width;
            }
            else
            {
                height = 0;
            }
            if ((height % 2) != 0)
            {
                height--;
            }
            if (height <= 0)
            {
                height = this.m_Height;
            }
            string str9 = string.Concat(new object[] { " -s ", this.e_Width, "x", height });
            if (!(this.configini.ReadBool("encoder", "allwh", true) || (this.e_Width <= this.m_Width)))
            {
                str9 = "";
            }
            switch (str7)
            {
                case "0":
                    str = num4.ToString() + ":" + @int.ToString();
                    break;

                case "1":
                    str = "main_w/2-overlay_w/2:" + @int;
                    break;

                case "2":
                    str = string.Concat(new object[] { "main_w-overlay_w-", num5, ":", @int });
                    break;

                case "3":
                    str = num4 + ":main_h/2-overlay_h/2";
                    break;

                case "4":
                    str = "main_w/2-overlay_w/2:main_h/2-overlay_h/2";
                    break;

                case "5":
                    str = "main_w-overlay_w-" + num5 + ":main_h/2-overlay_h/2";
                    break;

                case "6":
                    str = num4 + ":main_h-overlay_h-" + num3;
                    break;

                case "7":
                    str = "main_w/2-overlay_w/2:main_h-overlay_h-" + num3;
                    break;

                case "8":
                    str = string.Concat(new object[] { "main_w-overlay_w-", num5, ":main_h-overlay_h-", num3 });
                    break;

                default:
                    str = "";
                    break;
            }
            str = " -vf \"movie=0:png:" + path.Replace(@"\", "/") + " [wm];[in][wm] overlay=" + str + ":1 [out]\" ";
            if (!File.Exists(path))
            {
                str = "";
            }
            if (this.m_isf4v)
            {
                int num6 = func.GetInt(this.configini.ReadString("encoder", "f4vsplit", ""));
                if (num6 > 0)
                {
                    str4 = " -ss 00:00:" + num6 + " ";
                }
            }
            if ((this.e_MobileVideo == "是") || this.m_isAudio)
            {
                //                str2 = " -deinterlace -subq 5 -me_method umh -me_range 16 -bf 0  -i_qfactor 1.4 -b_qfactor 1.3 -flags +loop -level 31 ";
                //ljj@2016-5-27:去掉反隔行相关命令。提升画面清晰度。
                str2 = " -bf 0  -i_qfactor 1.4 -b_qfactor 1.3 -qmin 10 -qmax 51 -flags +loop -level 31 ";
            }
            else
            {
                //str2 = " -deinterlace -subq 7 -me_method umh -me_range 16 -bf 3  -i_qfactor 1.4 -b_qfactor 1.3 -coder 1 -refs 3 -qmin 10 -qmax 51 -sc_threshold 40 -flags2 +dct8x8  -flags +loop -trellis 1 ";
                //ljj@2016-5-27:去掉反隔行相关命令。提升画面清晰度。
                str2 = " -bf 3  -i_qfactor 1.4 -b_qfactor 1.3 -coder 1 -refs 3 -qmin 10 -qmax 51 -sc_threshold 40 -flags2 +dct8x8  -flags +loop -trellis 1 ";
            }
            if (this.outfiletype == "mp4")
            {
                str3 = this.file_out;
            }
            else
            {
                str3 = this.file_ff;
            }
            //string str10 = string.Concat(new object[] {
            //    "-i \"", str5, "\" ", str4, " -y -async 1 -vsync 1 -acodec libfaac -ab ", this.e_aBit, "k -ac 2 ", str9, " -vcodec libx264 -x264opts keyint= ",this.e_FrameRate, str2, " -threads 0  -b ", this.e_vBit, "k -maxrate 5000k -bufsize 1 -g ", this.e_FrameRate, " -r ", this.e_FrameRate,
            //    " ", str, "\"", str3, "\" "
            // });
            string str10 = string.Concat(new object[] {
                "-i \"", str5, "\" ", str4, " -y -async 1 -vsync 1 -acodec libfaac -ab ", this.e_aBit, "k -ac 2 ", str9, " -vcodec libx264 ", str2, " -threads 0  -b ", this.e_vBit, "k -maxrate 5000k -bufsize 1 -g ", this.e_FrameRate, " -r ", this.e_FrameRate,
                " ", str, "\"", str3, "\" "
             });


            //ffmpeg.exe -i "test.mp4" -y -async 1 -vsync 1 -acodec libfaac -ab 64k -ac 2 -s 640x360 -vcodec libx264  -bf 3 -i_qfactor 1.4 -b_qfactor 1.3 -coder 1 -refs 3 -qmin 10 -qmax 51 -sc_threshold 40 -trellis 1 -b 500k -maxrate 600k -bufsize 1 -g 30 -r 30  -vf "movie=0:png:D:/anyencode/anyEncoderServer/bin/Debug/images/logotext.png[wm];[in][wm] overlay=main_w-overlay_w-10:10:1 [out]"  "test_out.mp4"
            this.p_encoder = new ProcessCaller(this.formobj);
            this.p_encoder.FileName = Application.StartupPath + @"\ffmpeg\ffmpeg.exe";
            this.p_encoder.Arguments = str10;
            this.p_encoder.StdErrReceived += new DataReceivedHandler(this.writeStreamInfo);
            this.p_encoder.StdOutReceived += new DataReceivedHandler(this.writeStreamInfo);
            if (this.outfiletype == "mp4")
            {
                if (this.configini.ReadBool("encoder", "mobilevideo", false))
                {
                    this.p_encoder.Completed += new EventHandler(this.run_mobilevideo);
                }
                else
                {

                    this.p_encoder.Completed += new EventHandler(this.run_getimg);
                }
            }
            else
            {
                this.p_encoder.Completed += new EventHandler(this.run_yamdi);
            }
            this.p_encoder.Cancelled += new EventHandler(this.run_Canceled);
            this.p_encoder.tsn = this.fcode;
            this.p_encoder.PriorityClass = this.cpu_pri;
            this.statini.WriteString("encoder", "enmsg", "");
            this.AppendLog("开始ffmpeg转码：" + this.p_encoder.FileName + " " + this.p_encoder.Arguments);
            this.runtype = "ffmpeg";
            this.p_encoder.Start();
        }
        private void uploadqiniut()
        {
            string rnd = this.random();
            string mp4Key = GetTimeStamp() + "/" + rnd + ".mp4";
            //this.statini.WriteString("encoder", "qiniu1", "开始上传七牛：mp4Key=" + "1535444433/16152214132012111710.mp4" + ",filepath=" + "D:\\04ew9sl8wivn89zi_dst.mp4");
            //this.AppendLog("开始上传七牛：mp4Key=" + mp4Key + ",filepath=" + filepath);

            Upoader up = new Upoader();
            this.AppendLog("开始上传七牛");

            up.init();
            this.AppendLog("开始上传七牛");
            string retstring = up.PutFile(mp4Key, "D:\\04ew9sl8wivn89zi_dst.mp4");
            if (retstring == "")
            {
                //删除目标文件。已经上传成功，不需要保留了。
                //System.IO.File.Delete(filepath);
                //this.AppendLog("yeyeyeyeye+" + this.id.ToString());
                //this.statini.WriteString("encoder", "qiniuret2", "上传调用完成！返回字符串：" + retstring);
                //回调
                string filecode = "2yoy9n8r9fo0wusb";
                string status = "1";
                WebClient client = new System.Net.WebClient();
                string uri = "http://www.jianpianzi.com/cloud/transcodeStatus?status=1&filecode=" + filecode + "&mp4file=http://7xl6yy.com1.z0.glb.clouddn.com/" + mp4Key;
                byte[] bt = client.DownloadData(uri);
                Conn.ExecuteNonQuery("update ov_files set stat=2 where id=1249");

                //this.statini.WriteString("encoder", "uploadret", "url=" + uri);
                //System.IO.FileStream fs = System.IO.File.Create("c:\\encoderupload.txt");
                //fs.Write(bt, 0, bt.Length);
                //fs.Close();
                //修改数据库的地址为七牛
                this.AppendLog("encoder" + "uploadret" + "url=" + uri);

            }
            else
            {
                //this.statini.WriteString("encoder", "qiniuret", "上传失败，返回字符串为：" + retstring);
                this.AppendLog("上传失败，返回字符串为：" + retstring);

            }

        }
        private void run_getimg(object sender, EventArgs e)
        {
            if (!File.Exists(this.file_out))
            {
                this.AppendLog("无文件退出,错误+1");
                Conn.ExecuteNonQuery("update ov_files set stat=0 where id=" + this.id);
                this.run_Canceled(null, null);

            }
            else
            {
                string str = "";
                int num = this.configini.ReadInteger("encoder", "maxcap", 6);
                int width = this.configini.ReadInteger("encoder", "capwidth", 160);
                int height = this.configini.ReadInteger("encoder", "cap_height", 120);
                MediaInfo info = new MediaInfo();
                info.Open(this.file_out);
                int num4 = func.GetInt(info.Get(StreamKind.General, 0, "Duration").ToString()) / 0x3e8;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                if ((num4 > 0) && !this.m_isAudio)
                {
                    int num5;
                    this.statini.WriteString("encoder", "enmsg", "正在截图...");
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    if (this.m_Width > 0)
                    {
                        num5 = (this.m_Height * width) / this.m_Width;
                    }
                    else
                    {
                        num5 = 0;
                    }
                    if ((num5 % 2) != 0)
                    {
                        num5--;
                    }
                    if (num5 <= 0)
                    {
                        width = this.m_Width;
                        num5 = this.m_Height;
                    }
                    while (num6 < num4)
                    {
                        num8++;
                        num7 = num6;
                        startInfo.FileName = Application.StartupPath + @"\ffmpeg\ffmpeg.exe";
                        string thumbnailPath = this.imgdir + this.fcode + "_" + num8.ToString() + ".jpg";
                        string path = thumbnailPath + "_b.jpg";
                        startInfo.Arguments = string.Concat(new object[] { "-ss ", num7, " -i ", this.file_out, " -f image2 -vframes 1 ", path });
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = false;
                        this.AppendLog("开始截图：" + startInfo.FileName + " " + startInfo.Arguments);
                        Process.Start(startInfo).WaitForExit();
                        if (File.Exists(path))
                        {
                            func.imgResize(path, thumbnailPath, width, height, "CUT");
                        }
                        if (string.IsNullOrEmpty(str))
                        {
                            str = num8.ToString();
                        }
                        else
                        {
                            str = str + "," + num8.ToString();
                        }
                        num6 += func.GetInt(num4 / num);
                        if (num8 == num)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    str = "-1";
                }
                if (File.Exists(this.file_swf))
                {
                    startInfo.FileName = Application.StartupPath + @"\swftools\swfrender.exe";
                    startInfo.Arguments = string.Concat(new object[] { "\"", this.file_in, "\" -X ", width, " -o \"", this.imgdir, this.fcode, "_1.jpg\"" });
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    this.AppendLog("开始截图：" + startInfo.FileName + " " + startInfo.Arguments);
                    this.statini.WriteString("encoder", "enmsg", "正在对swf截图:1");
                    Process.Start(startInfo).WaitForExit();
                    str = "1";
                }
                if ((num4 > 0) || File.Exists(this.file_swf))
                {
                    FileInfo info3 = new FileInfo(this.file_out);
                    long length = info3.Length;
                    Conn.ExecuteNonQuery(string.Concat(new object[] { "update ov_files set stat=1,sendok=0,times=", num4, ",flvsize=", length, ",autoimg='", str, "' where id=", this.id }));
                    try
                    {
                        File.Delete(this.file_ff);
                    }
                    catch
                    {
                        this.statini.WriteString("encoder", "delfile_ff", "删除文件失败:" + this.file_ff);
                    }
                    try
                    {
                        File.Delete(this.file_me);
                    }
                    catch
                    {
                        this.statini.WriteString("encoder", "delfile_me", "删除文件失败:" + this.file_me);
                    }
                    try
                    {
                        File.Delete(this.file_log);
                    }
                    catch
                    {
                        this.statini.WriteString("encoder", "delfile_log", "删除文件失败:" + this.file_log);
                    }
                    
                    this.statini.WriteString("encoder", "enmsg", "处理文件成功");
                    this.AppendLog("处理文件成功：" + this.file_in);
                    //处理文件成功在这里添加一个测试方法 看是否成功
                    string imgPath = this.imgdir + this.fcode + "_" + "3.jpg";
                    string bigimgpath = imgPath + "_b.jpg";
                    // postFile(imgPath,bigimgpath);
                }
                else
                {
                    this.statini.WriteString("encoder", "enmsg", "处理文件失败");
                    this.AppendLog("处理文件错误,错误+1：" + this.file_in);
                    Conn.ExecuteNonQuery("update ov_files set stat=0 where id=" + this.id);
                }
                this.tmchk.Enabled = true;
            }
            try
            {
                //上传七牛 - 上传前判断如果是mp4，需要过一遍qt-faststart.exe
                this.AppendLog("filetype：" + this.outfiletype);

                if (this.outfiletype == "mp4")
                {
                    this.AppendLog("开始调用qt-faststart流化MP4");

                    this.statini.WriteString("encoder", "mp4streamconvert_begin", "开始调用qt-faststart流化MP4");
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\qt-faststart.exe";
                    string dstMP4File = this.file_out.Replace(".mp4", "_dst.mp4");
                    startInfo.Arguments = string.Concat(new object[] { this.file_out, " ", dstMP4File });
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    Process.Start(startInfo).WaitForExit();
                    if (System.IO.File.Exists(dstMP4File))
                    {
                        System.IO.File.Delete(this.file_out); //删除原文件
                        this.file_out = dstMP4File;
                    }
                    this.AppendLog("结束调用qt-faststart流化MP4");

                    this.statini.WriteString("encoder", "mp4streamconvert_end", "结束调用qt-faststart流化MP4");
                }

          
                this.AppendLog("展示3：" + this.trancode.ToString());
                this.statini.WriteString("encoder", "展示3：", this.id);

                string rnd = this.random();
                string mp4Key = GetTimeStamp() + "/" + rnd + ".mp4";
                string filepath = this.file_out.Replace("////", "\\").Replace("/", "\\");
                this.statini.WriteString("encoder", "qiniu1", "开始上传七牛：mp4Key=" + mp4Key + ",filepath=" + filepath);
                this.AppendLog("开始上传七牛：mp4Key=" + mp4Key + ",filepath=" + filepath);

                Upoader up = new Upoader();
                up.init();
                string retstring = up.PutFile(mp4Key, filepath);
                if (retstring == "")
                {
                    this.statini.WriteString("encoder", "qiniuret", "上传成功！mp4Key = " + mp4Key);
                    //删除目标文件。已经上传成功，不需要保留了。
                    System.IO.File.Delete(filepath);
                    this.AppendLog("yeyeyeyeye+" + this.id.ToString());
                    this.statini.WriteString("encoder", "qiniuret2", "上传调用完成！返回字符串：" + retstring);
                    //回调
                    string filecode = this.fcode;
                    string status = "1";
                    WebClient client = new System.Net.WebClient();
                    string uri = "http://www.jianpianzi.com/cloud/transcodeStatus?status=1&filecode=" + filecode + "&mp4file=http://7xl6yy.com1.z0.glb.clouddn.com/" + mp4Key;
                    byte[] bt = client.DownloadData(uri);
                    Conn.ExecuteNonQuery("update ov_files set stat=2 where id=" + this.id);

                    this.statini.WriteString("encoder", "uploadret", "url=" + uri);
                    //System.IO.FileStream fs = System.IO.File.Create("c:\\encoderupload.txt");
                    //fs.Write(bt, 0, bt.Length);
                    //fs.Close();
                    //修改数据库的地址为七牛
                    this.AppendLog("encoder" + "uploadret" + "url=" + uri);

                }
                else
                {
                    this.statini.WriteString("encoder", "qiniuret", "上传失败，返回字符串为：" + retstring);
                    this.AppendLog("上传失败，返回字符串为：" + retstring);

                }



            }
            catch (Exception ex)
            {
                this.statini.WriteString("encoder", "qiniuret3", "有异常，日志存入c:\\log.txt");
                System.IO.FileStream fs = System.IO.File.Create("c:\\log.txt");
                byte[] btarr = System.Text.Encoding.Default.GetBytes(ex.ToString());
                fs.Write(btarr, 0, btarr.Length);
                fs.Close();
            }


        }
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public string random()
        {
            int[] idlength = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 21, 22, 23, 24 };
            int[] result = new int[10];
            for (int i = 1; i <= 10; i++)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                int rang = 24 - i;//随机数下标的范围
                int index = random.Next(0, rang);//随机获取一个0到rang的下标
                result[i - 1] = idlength[index];//取值赋给result
                idlength[index] = idlength[rang];
            }
            return string.Join("", result);
        }

        public void postFile(string imgpath, string bigimgpath)
        {
            try
            {
                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                // <form action="http://mybox/cgi-bin/myscript.cgi
                //  method="post" enctype="multipart/form-data">
                MultiPartForm mf = new MultiPartForm();

                // <input name="frmUsername">
                mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "filecode",
                    CURLformoption.CURLFORM_COPYCONTENTS, this.fcode,
                    CURLformoption.CURLFORM_END);

                // <input type="File" name="f1">
                mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "file",
                    CURLformoption.CURLFORM_FILE, this.file_out,
                    CURLformoption.CURLFORM_CONTENTTYPE, "application/binary",
                    CURLformoption.CURLFORM_END);
                mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "img",
                    CURLformoption.CURLFORM_FILE, imgpath,
                    CURLformoption.CURLFORM_CONTENTTYPE, "application/binary",
                    CURLformoption.CURLFORM_END);

                mf.AddSection(CURLformoption.CURLFORM_COPYNAME, "bigimg",
                    CURLformoption.CURLFORM_FILE, bigimgpath,
                    CURLformoption.CURLFORM_CONTENTTYPE, "application/binary",
                    CURLformoption.CURLFORM_END);


                Easy easy = new Easy();

                Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
                easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);
                easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);

                Easy.ProgressFunction pf = new Easy.ProgressFunction(OnProgress);
                easy.SetOpt(CURLoption.CURLOPT_PROGRESSFUNCTION, pf);

                easy.SetOpt(CURLoption.CURLOPT_URL, this.weburl);
                easy.SetOpt(CURLoption.CURLOPT_HTTPPOST, mf);

                easy.Perform();
                easy.Cleanup();
                mf.Free();

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                FileStream fs1 = new FileStream("D:\\test\\1.txt", FileMode.OpenOrCreate);
                StreamWriter sw1 = new StreamWriter(fs1);
                sw1.Write(ex.ToString());
                sw1.Close();
                fs1.Close();
            }

            FileStream fs = new FileStream("D:\\test\\A.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write("aaaa ");
            sw.Close();
            fs.Close();


        }
        public static void OnDebug(CURLINFOTYPE infoType, String msg,
            Object extraData)
        {
            // print out received data only
            if (infoType == CURLINFOTYPE.CURLINFO_DATA_IN)
                Console.WriteLine(msg);
        }


        public static Int32 OnProgress(Object extraData, Double dlTotal,
            Double dlNow, Double ulTotal, Double ulNow)
        {
            Console.WriteLine("Progress: {0} {1} {2} {3}",
                dlTotal, dlNow, ulTotal, ulNow);
            return 0; // standard return from PROGRESSFUNCTION
        }

        private void run_mencoder()
        {
            int num = 600;
            if (this.m_Bit > 0)
            {
                num = this.m_Bit / 0x3e8;
            }
            int frameRate = 15;
            if (this.m_FrameRate > 15)
            {
                frameRate = this.m_FrameRate;
            }
            this.file_me = this.file_in + ".me.flv";
            this.p_encoder = new ProcessCaller(this.formobj);
            this.p_encoder.FileName = Application.StartupPath + @"\mencoder\mencoder.exe";
            this.p_encoder.Arguments = string.Concat(new object[] { this.file_in, " -o ", this.file_me, "  -ofps ", frameRate, " -of lavf -lavfopts format=flv -ovc x264 -x264encopts bitrate=", num, "00:vbv_maxrate=10000:vbv_bufsize=50000 -oac faac -faacopts mpeg=4:object=2:br=64:raw -srate 44100  -mc 1" });
            this.p_encoder.StdErrReceived += new DataReceivedHandler(this.writeStreamInfo);
            this.p_encoder.StdOutReceived += new DataReceivedHandler(this.writeStreamInfo);
            this.p_encoder.Completed += new EventHandler(this.run_ffmpeg);
            this.p_encoder.Cancelled += new EventHandler(this.run_Canceled);
            this.p_encoder.tsn = this.fcode;
            this.p_encoder.PriorityClass = this.cpu_pri;
            this.statini.WriteString("encoder", "enmsg", "正在进行预处理编码...");
            this.AppendLog("开始mencoder转码：" + this.p_encoder.FileName + " " + this.p_encoder.Arguments);
            this.runtype = "mencoder";
            this.p_encoder.Start();
        }

        private void run_mobilevideo(object sender, EventArgs e)
        {
            int num;
            if (this.outfiletype != "mp4")
            {
                string str = this.file_out.Replace(".flv", ".mp4");
                ProcessStartInfo info2 = new ProcessStartInfo();
                info2.FileName = Application.StartupPath + @"\ffmpeg\ffmpeg.exe";
                info2.Arguments = "-i " + this.file_out + " -y -acodec copy -vcodec copy " + str;
                info2.CreateNoWindow = true;
                info2.UseShellExecute = false;
                ProcessStartInfo startInfo = info2;
                this.AppendLog("开始生成mp4文件：" + startInfo.FileName + " " + startInfo.Arguments);
                this.statini.WriteString("encoder", "enmsg", "正在生成mp4文件");
                Process.Start(startInfo).WaitForExit();
            }
            string str2 = "";
            if (this.outfiletype != "mp4")
            {
                str2 = this.file_out.Replace(".flv", "_3g.mp4");
            }
            else
            {
                str2 = this.file_out.Replace(".mp4", "_3g.mp4");
            }
            int num2 = 320;
            if (this.m_Width > 0)
            {
                num = (this.m_Height * num2) / this.m_Width;
            }
            else
            {
                num = 0;
            }
            if ((num % 2) != 0)
            {
                num--;
            }
            if (num <= 0)
            {
                num2 = 320;
                num = 240;
            }
            this.p_encoder = new ProcessCaller(this.formobj);
            this.p_encoder.FileName = Application.StartupPath + @"\ffmpeg\ffmpeg.exe";
            this.p_encoder.Arguments = string.Concat(new object[] { "-i \"", this.file_out, "\"  -y -s 320x", num, " -acodec libfaac -ab 32k  -vcodec libx264 -subq 5 -me_method umh -me_range 16 -bf 0  -i_qfactor 1.4 -b_qfactor 1.3 -flags +loop -level 31 -threads 0  -b 300k -maxrate 600k -bufsize 1 -g 15 -r 15 \"", str2, "\"" });
            this.p_encoder.StdErrReceived += new DataReceivedHandler(this.writeStreamInfo);
            this.p_encoder.StdOutReceived += new DataReceivedHandler(this.writeStreamInfo);
            this.p_encoder.Completed += new EventHandler(this.run_getimg);
            this.p_encoder.Cancelled += new EventHandler(this.run_Canceled);
            this.p_encoder.tsn = this.fcode;
            this.p_encoder.PriorityClass = this.cpu_pri;
            this.statini.WriteString("encoder", "enmsg", "正在进行手机视频编码");
            this.AppendLog("开始生成手机视频文件：" + this.p_encoder.FileName + " " + this.p_encoder.Arguments);
            this.runtype = "ffmpeg";
            this.p_encoder.Start();
        }

        private void run_transcode_plug(object sender, EventArgs e)
        {
            this.p_encoder = new ProcessCaller(this.formobj);
            this.p_encoder.FileName = "D:\\TPTranscode\\bin_Win32Debug\\TPTransCodeTool.exe";
            this.p_encoder.Arguments = this.file_in + " " + this.file_out;
            this.p_encoder.Completed += new EventHandler(this.run_getimg);
            this.p_encoder.Cancelled += new EventHandler(this.run_Canceled);
            this.p_encoder.tsn = this.fcode;
            this.p_encoder.PriorityClass = this.cpu_pri;
            this.statini.WriteString("encoder", "enmsg", "");
            this.AppendLog("开始插件转码：" + this.p_encoder.FileName + " " + this.p_encoder.Arguments);
            this.runtype = "transcodePlug";
            this.p_encoder.Start();
        }

        private void run_yamdi(object sender, EventArgs e)
        {
            if (!File.Exists(this.file_ff))
            {
                this.run_getimg(null, null);
            }
            this.p_encoder = new ProcessCaller(this.formobj);
            this.p_encoder.FileName = Application.StartupPath + @"\yamdi.exe";
            this.p_encoder.Arguments = "-i \"" + this.file_ff + " \" -o \"" + this.file_out + "\" -l";
            this.p_encoder.Cancelled += new EventHandler(this.run_Canceled);
            if (this.configini.ReadBool("encoder", "mobilevideo", false))
            {
                this.p_encoder.Completed += new EventHandler(this.run_mobilevideo);
            }
            else
            {
                this.p_encoder.Completed += new EventHandler(this.run_getimg);
            }
            this.p_encoder.tsn = "";
            this.p_encoder.PriorityClass = this.cpu_pri;
            this.statini.WriteString("encoder", "enmsg", "正在写入关键帧...");
            this.AppendLog("开始yamdi命令：" + this.p_encoder.FileName + " " + this.p_encoder.Arguments);
            this.p_encoder.Start();
        }

        private void writeStreamInfo(object sender, anyEncoder.DataReceivedEventArgs e)
        {
            string text = e.Text;
            int @int = 0;
            switch (this.runtype)
            {
                case "ffmpeg":
                    if (text.IndexOf("time") > -1)
                    {
                        @int = func.GetInt(Regex.Replace(text, @"[\s\S]*time=([^\s]*)\.[\s\S]*", "$1"));
                    }
                    if (text.IndexOf("kb/s:") > -1)
                    {
                        @int = 0xf423f;
                    }
                    this.AppendLog(text);
                    this.statini.WriteInteger("encoder", "entime", @int);
                    break;

                case "mencoder":
                    if (text.IndexOf("Pos") > -1)
                    {
                        @int = func.GetInt(Regex.Replace(text, @"[\s\S]*Pos:  ([^\s]*)\.[\s\S]*", "$1"));
                    }
                    if (text.IndexOf("Writing index") > -1)
                    {
                        @int = 0xf423f;
                    }
                    this.statini.WriteInteger("encoder", "entime", @int);
                    break;
            }
            ProcessCaller caller = (ProcessCaller)sender;
            if (!caller.HasExited)
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - caller.StartTime);
                this.statini.WriteInteger("encoder", "pruntime", (int)span.TotalSeconds);
                try
                {
                    this.statini.WriteInteger("encoder", "cpu", (int)(((caller.TotalProcessorTime.TotalMilliseconds / span.TotalMilliseconds) / ((double)this.cpuCount)) * 100.0));
                }
                catch
                {
                }
            }
        }
    }
}

