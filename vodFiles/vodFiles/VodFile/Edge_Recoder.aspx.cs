using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using VodFile.common;
using System.Diagnostics;
using System.Configuration;
using System.Globalization;
using System.Timers;

namespace VodFile
{
    public partial class Edge_Recoder : System.Web.UI.Page
    {
        private static Dictionary<string, string> dic;
        System.Timers.Timer t;
        private string pathFileName="";
        private string fsize = "";
        private string filecode = "";
        private string truedir = "";
        private string fileName = "";
        private string liveurl = "";
        private Process process;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["liveurl"] != null && Request.QueryString["Recscl"] != null)
                { //判断传入的文件网址是否为空
                    //将文件传入数据库，返回filecode
                    dic = new Dictionary<string, string>();
                    liveurl = Request.QueryString["liveurl"].ToString();
                    //string url=liveurl.Substring(0,liveurl.LastIndexOf("\\"));
                    //string urlFileName=liveurl.Substring(liveurl.LastIndexOf("\\")+1);
                    //string FileName = urlFileName.Substring(0, urlFileName.LastIndexOf("."));
                    fileName = DateTime.Now.ToString("ddHHmmssfff", DateTimeFormatInfo.InvariantInfo);
                    //string path1 =url;
                    //string path2 = fileName+ Path.GetExtension(urlFileName).ToLower();
                    //string path = Path.Combine(path1,path2);

                    filecode = func.GetRandomString(0x10).ToLower();

                    string webserver = System.Configuration.ConfigurationManager.AppSettings.Get("webserver").ToString();
                    string ischange = System.Configuration.ConfigurationManager.AppSettings.Get("ischange").ToString();
                    truedir = HttpContext.Current.Server.MapPath(".") + @"\";
         
                    if (!System.IO.Directory.Exists(@truedir+"tmpfiles\\"))
                    {
                        //目录不存在，建立目录 
                        System.IO.Directory.CreateDirectory(@truedir+"tmpfiles\\");
                    } 
                    //string truedir = url.Replace(HttpContext.Current.Server.MapPath("~/"), "").Replace(@"\", "/");
                    //if (!System.IO.Directory.Exists(@truedir+"tmpfiles"))
                    //{
                    // 目录不存在，建立目录 
                    //System.IO.Directory.CreateDirectory(@truedir+"tmpfiles");
                    // }
                    //System.IO.File.Copy(liveurl, @truedir+"tmpfiles\\" + path2);
                    //Directory.Move(liveurl, @truedir + "tmpfiles\\" + path2);
                    //System.IO.FileInfo fi = new System.IO.FileInfo(@truedir + "tmpfiles\\" + path2);
                    
                    int filetype = func.GetInt(pagebase.Get("filetype"));
                    string outfilename = getOutFileName(filetype);
                    string path_str = @FileUpload.CreatFolder();
                    string filedir = FileUpload.GetFlvFolder(path_str);
                    int times = Convert.ToInt32(Request.QueryString["Recscl"]);//分钟
                    //System.IO.FileInfo fi = new System.IO.FileInfo(@truedir + "tmpfiles\\" + fileName + ".flv");
                    string fileSize ="0";
                    dic.Add("filecode", filecode);
                    dic.Add("filename", fileName + ".flv");
                    dic.Add("filesize", fileSize);
                    dic.Add("outfilename", outfilename);
                    dic.Add("stat", ischange);
                    dic.Add("addtime", DateTime.Now.ToString());
                    dic.Add("filedir", filedir);
                    dic.Add("linkflv", "0");
                    dic.Add("truedir", truedir.Replace("\\", "\\\\"));
                    dic.Add("webserver", webserver);
                    dic.Add("filetype", "0");
                    dic.Add("prefilename", liveurl);
                    dic.Add("addip", func.GetIp());
                    dic.Add("errcount", "0");
                    dic.Add("isdel", "0");
                    dic.Add("times", times.ToString());
                    StringBuilder builder = new StringBuilder();
                    builder.Append("insert into ov_files (");
                    builder.Append("filecode,filename,filesize,outfilename,stat,addtime,filedir,linkflv,truedir,webserver,filetype,prefilename,addip,errcount,isdel,times)");
                    builder.Append("values (");
                    builder.AppendFormat("'{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}',{13},{14},{15})", dic["filecode"].ToString(), dic["filename"].ToString(), dic["filesize"].ToString(), dic["outfilename"].ToString(), Convert.ToInt32(dic["stat"].ToString()), Convert.ToDateTime(dic["addtime"].ToString()), dic["filedir"].ToString(), Convert.ToInt32(dic["linkflv"].ToString()), Convert.ToString(dic["truedir"]), dic["webserver"].ToString(), dic["filetype"].ToString(), dic["prefilename"].ToString(), dic["addip"].ToString(), Convert.ToInt32(dic["errcount"]), Convert.ToInt32(dic["isdel"]), Convert.ToInt32(dic["times"]));
                    builder.Append(";");
                    int result=MySqlHelper.ExecuteNonQuery(MyConn.ConnectionString, builder.ToString());
                    //if (result > 0)
                    //{
                    //    HttpContext.Current.Response.Write("filecode:  " + filecode);//成功
                    //}
                    //else
                    //{
                    //    HttpContext.Current.Response.Write("上传失败");//成功
                    //}
                    //////////////////////////////////////////////////////////
                    try
                    {
                        pathFileName = @truedir + "tmpfiles\\" + fileName + ".flv";
                        //到一定时间后，结束进程取消读取的直播流
                        int millisecond = times*60000;
                        //实例化Timer类，设置间隔时间为10000毫秒； 
                        t = new System.Timers.Timer(millisecond);
                        //到达时间的时候执行事件；
                        t.Elapsed += new System.Timers.ElapsedEventHandler(rtmpdump_exit_time);
                        t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                        t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

                        //启动流媒体进程 rtmpdump.exe
                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = AppDomain.CurrentDomain.BaseDirectory + @"rtmpdump\rtmpdump.exe";
                        //info.Arguments = "-o savename.flv -r rtmp://172.16.172.212/livepkgr/livestream -v";
                        info.Arguments = "-o " + @truedir + "tmpfiles\\" + fileName + ".flv" + " -r " + liveurl + " -v";
                        info.CreateNoWindow = true;//获取或设置指示是否在新窗口中启动该进程的值。
                        info.UseShellExecute = false;//获取或设置一个值，该值指示是否使用操作系统外壳程序启动进程。
                        process = Process.Start(info);
                        process.WaitForExit(0x2bf20);
                        //将文件写入到文件流中

                        if (result > 0)
                        {
                            HttpContext.Current.Response.Write("filecode:  " + filecode);//成功
                        }
                        else
                        {
                            HttpContext.Current.Response.Write("上传失败");//成功
                        }
                    }
                    catch (Exception)
                    {
                        t.Enabled = false;
                        killall();
                    }
                    //finally
                    //{
                        //killall();
                    //}
                    ///////////////////////////////////////////////////////////
                    
                }
                else
                {
                    HttpContext.Current.Response.Write("文件路径不能为空");
                }
            }
        }
        protected void rtmpdump_exit_time(object sender, ElapsedEventArgs e) 
        {
           // killall();
            if (!process.HasExited)
            {
                process.Kill();
            }
            t.Enabled = false;
            t.Close();
            t.Dispose();
            System.IO.FileInfo finfo = new System.IO.FileInfo(pathFileName);
            fsize = finfo.Length.ToString();
            StringBuilder ubuilder = new StringBuilder();
            ubuilder.AppendFormat("update ov_files set filesize='{0}' where filecode='{1}'", fsize, filecode);
            MySqlHelper.ExecuteNonQuery(MyConn.ConnectionString, ubuilder.ToString());
        }
        /// <summary>
        /// 获取外部文件路径
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        protected string getOutFileName(int fileType)
        {
            string str;
            if (fileType > 0)
            {
                str = func.GetRandomString(0x10).ToLower() + ".swf";
            }
            else
            {
                bool flag = false;
                try
                {
                    if (ConfigurationManager.AppSettings["outvideo"].Trim().ToLower() == ".mp4")
                    {
                        flag = true;
                    }
                }
                catch
                {
                    flag = false;
                }
                if (flag)
                {
                    str = func.GetRandomString(0x10).ToLower() + ".mp4";
                }
                else
                {
                    str = func.GetRandomString(0x10).ToLower() + ".flv";
                }
            }
            return str;
        }
        public static void killall()
        {
            //Process[] processes = Process.GetProcessesByName("rtmpdump");
            //if (processes[0] != null)
            //{
            //    processes[0].Kill();
            //}
            Process[] pro = Process.GetProcessesByName("rtmpdump");
            for (int k = 0; k < pro.Length; k++)
            {
                if ("rtmpdump".IndexOf(pro[k].ProcessName.ToLower()) >= 0)
                {
                    try
                    {
                        pro[k].Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}