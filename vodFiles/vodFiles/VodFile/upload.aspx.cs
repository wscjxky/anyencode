using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using VodFile.common;

namespace VodFile
{
    public partial class sns_upload : System.Web.UI.Page
    {
        public string domain;
        public string ext;
        public int filetype;
        private long onesize=0;
        private long spacesize=0;
        public string swfurl;
        public string fromApp;//调用app
        private static Dictionary<string, string> dic;
        /// <summary>
        /// 接受md5去重
        /// </summary>
        public string md5;
        //发送message
        public void SendResponse(string resp)
        {
            HttpContext.Current.Response.Write(resp);
            HttpContext.Current.Response.End();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           // Response.AddHeader("Access-Control-Allow-Origin", "*");
            //获取一系列传入参数
            string str = pagebase.Get("action");
            this.filetype = func.GetInt(pagebase.Get("filetype"));//文件属性
            this.fromApp = pagebase.Get("from");//调用app
            this.md5 = pagebase.Get("md5");//获取md5参数 
            if (md5 == string.Empty || str == string.Empty)
            {
                this.SendResponse("禁止非法调用上传服务！");
            }
            else
            {
                ////////////////////////md5去重秒传///////////////////////////
                ////查询ID  并且 写入md5
                StringBuilder builder = new StringBuilder();
                builder.Append("select VideoFileID from FileHash where FileHash ='");
                builder.Append(md5);
                builder.Append("'");
                SqlDataReader sdr = Conn.ExecuteReader(builder.ToString());
                if (sdr.Read())
                {
                    string id = sdr["VideoFileID"].ToString();
                   //暂时不处理MD5
                    sdr.Close();
                    builder = new StringBuilder();
                    builder.AppendFormat("select * from ov_files where id={0}", id);
                    sdr = Conn.ExecuteReader(builder.ToString());
                    if (sdr.Read())
                    {   
                        //秒传 
                        string filedir = sdr["filedir"].ToString();
                        string oldfilename = sdr["outfilename"].ToString();
                        string prefilename = sdr["prefilename"].ToString();
                        ///原始文件的名字
                        string filename = sdr["filename"].ToString();
                        string filesize = sdr["filesize"].ToString();
                        string filecode = sdr["filecode"].ToString();
                        this.SendResponse("{\"status\":\"2\",\"md5\":\"" + md5 + "\",\"filedir\":\"" +filedir + "\",\"outfilename\":\"" + oldfilename + "\",\"oldname\":\"" +prefilename + "\",\"filcode\":\"" + filecode + "\",\"filename\":\""+filename+"\",\"filesize\":\""+filesize+"\"}");
                    }                            
                }
               ////////////////////////////////////////////////////////
            }
            if (str.IndexOf("save")>=0)
            {
                //上传完成，返回后
                this.fromApp = str.Substring(str.LastIndexOf("=") + 1);//返回调用app参数
                this.save_upload();
            }
            else
            {
                //第一次加载初次打开页面
                //1、检验是否为允许上传服务器
                //非法调用直接返回
                if (!actCheck())
                {
                    this.SendResponse("禁止非法调用上传服务！");
                }
            }
        }
        /// <summary>
        /// 上传完成保存
        /// </summary>
        protected void save_upload()
        {
            UpFiles files;
            files = new UpFiles
            {
                SavePath = "tmpfiles"
            };
            //存储文件
            files.SaveUploadFiles();
            //写入数据库
           if ((files.GetPromptMessage() != null) || (files.listFile.Count == 0))//文件
            {
                base.Response.End();
            }
            else
            {
                try
                {
                    dic = new Dictionary<string, string>();
                    string fileName = files.listFile[0].fileName;
                    int fileSize = files.listFile[0].fileSize;
                    //文件标题名，不含扩展名
                    string oldname = files.listFile[0].oldname;
                    string ext = files.listFile[0].fileExt;
                    //文件代码
                    string fcode = func.GetRandomString(0x10).ToLower();
                    //[stat] 0
                    //对外访问路径
                    string filedir = func.GetFlvFolder();
                    //本地文件相对路径
                    string truedir = HttpContext.Current.Server.MapPath(".") + @"\";
                    
                    if (!System.IO.Directory.Exists(@truedir + "tmpfiles\\"))
                    {
                        //目录不存在，建立目录 
                        System.IO.Directory.CreateDirectory(@truedir + "tmpfiles\\");
                    } 
                    //对外文件名
                    string outfilename = getOutFileName(this.filetype);
                    string webserver = System.Configuration.ConfigurationManager.AppSettings.Get("webserver").ToString();
                    //string webserver = "";
                    string ischange = System.Configuration.ConfigurationManager.AppSettings.Get("ischange").ToString();
                    dic.Add("filecode", fcode);
                    dic.Add("filename", fileName);
                    dic.Add("filesize", fileSize.ToString());
                    dic.Add("outfilename", outfilename);
                    dic.Add("stat", ischange);
                    dic.Add("addtime", DateTime.Now.ToString());
                    dic.Add("filedir", filedir);
                    dic.Add("linkflv", "0");
                    dic.Add("truedir", truedir.Replace("\\", "\\\\"));
                    dic.Add("webserver", webserver);
                    dic.Add("filetype", "0");
                    dic.Add("prefilename", oldname+ext);
                    dic.Add("addip", func.GetIp());
                    dic.Add("errcount", "0");
                    dic.Add("isdel", "0");
                    StringBuilder builder = new StringBuilder();
                    builder.Append("insert into ov_files (");
                    builder.Append("filecode,filename,filesize,outfilename,stat,addtime,filedir,linkflv,truedir,webserver,filetype,prefilename,addip,errcount,isdel)");
                    builder.Append("values (");
                    builder.AppendFormat("'{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}',{13},{14})", dic["filecode"].ToString(), dic["filename"].ToString(), dic["filesize"].ToString(), dic["outfilename"].ToString(), Convert.ToInt32(dic["stat"].ToString()), Convert.ToDateTime(dic["addtime"].ToString()), dic["filedir"].ToString(), Convert.ToInt32(dic["linkflv"].ToString()), Convert.ToString(dic["truedir"]), dic["webserver"].ToString(), dic["filetype"].ToString(), dic["prefilename"].ToString(), dic["addip"].ToString(), Convert.ToInt32(dic["errcount"]), Convert.ToInt32(dic["isdel"]));
                    builder.Append(";");
                    int result = Conn.ExecuteNonQuery(builder.ToString());
                    if (result > 0)
                    {   
                        //获取插入的id  插入md5  
                        ///////////////////////////////////////////////////
                        ////查询ID  并且 写入md5
                        //异常暂不处理  失败返回null 
                       builder = new StringBuilder();
                       builder.Append("select id from ov_files where filecode='");
                        builder.Append(dic["filecode"]);
                        builder.Append("'");
                        SqlDataReader sdr = Conn.ExecuteReader(builder.ToString());
                        if (sdr.Read())
                        {
                            string id =sdr["id"].ToString();
                            sdr.Close();
                            builder = new StringBuilder();
                            builder.AppendFormat("insert into FileHash(FileHash,VideoFileID) values('{0}','{1}')", this.md5, id);
                            Conn.ExecuteNonQuery(builder.ToString());  
                        }
                        ////////////////////////////////////////////////////////
                        HttpContext.Current.Response.Write("{\"status\":\"1\",\"md5\":\""+md5+"\",\"filedir\":\"" + filedir + "\",\"outfilename\":\"" + outfilename + "\",\"oldname\":\"" + oldname+ext + "\",\"filcode\":\"" + fcode+"\",\"filename\":\""+fileName+"\",\"filesize\":\""+fileSize+"\"}");//成功
                    }
                    else
                    {
                        HttpContext.Current.Response.Write("上传失败");//成功
                    }
                }
                catch (Exception exception)
                {
                    base.Response.Write(exception.ToString());
                }
                base.Response.End();
            }
        }
        protected void show_upload()
        {
            //swf上传文件参数
            this.swfurl = "&max=" + ((this.onesize * 0x400)).ToString();
            this.swfurl = this.swfurl + "&space=" + ((this.spacesize * 0x400)).ToString();
            this.swfurl = this.swfurl + "&ftype=" + this.ext;
            //返回url参数
            string str = "sns_upload.aspx?action=save|from=" + this.fromApp;
            this.swfurl = this.swfurl + "&server=" + str;
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
        /// <summary>
        /// 调用来路判断
        /// </summary>
        /// <returns></returns>
         protected bool actCheck() {
             this.domain = ConfigurationManager.AppSettings["domain"].Trim().ToLower();
             string[] rqStr = Request.ServerVariables.GetValues("Http_Referer");//来路域名
             if (this.domain.Equals("*"))
             {           
                 if (rqStr == null) return false;
                 return true;//允许所有来路调用
             }        
             bool atc = true;
             if (rqStr == null)
             {
                 atc = false;
             }
             else if (rqStr[0].IndexOf(this.domain) < 0)
             {
                 atc = false;
             }
             return atc;
         }

    }
}