namespace VodFile
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Collections.Generic;

    public class filedata
    {
        public static bool saveDB(string filename, string oldname, string outfilename, string filedir, string truedir, int filesize, string fcode, int filetype)
        {

            
            StringBuilder builder = new StringBuilder();
            builder.Append("insert into ov_files (");
            builder.Append("filecode,filename,filesize,outfilename,stat,addip,addtime,adduser,filedir,linkflv,truedir,webserver,filetype,prefilename)");
            builder.Append("values (");
            builder.Append("@filecode,@filename,@filesize,@outfilename,@stat,@addip,getdate(),@adduser,@filedir,@linkflv,@truedir,@webserver,@filetype,@prefilename)");
            builder.Append(";");
            SqlParameter[] cmdParms = new SqlParameter[] { new SqlParameter("@filecode", SqlDbType.NVarChar, 0x10), new SqlParameter("@filename", SqlDbType.NVarChar, 0xff), new SqlParameter("@filesize", SqlDbType.Int), new SqlParameter("@outfilename", SqlDbType.NVarChar, 0xff), new SqlParameter("@stat", SqlDbType.Int), new SqlParameter("@addip", SqlDbType.NVarChar, 50), new SqlParameter("@adduser", SqlDbType.NVarChar, 50), new SqlParameter("@filedir", SqlDbType.NVarChar, 50), new SqlParameter("@linkflv", SqlDbType.Int), new SqlParameter("@truedir", SqlDbType.NVarChar, 50), new SqlParameter("@webserver", SqlDbType.NVarChar, 0xff), new SqlParameter("@filetype", SqlDbType.Int), new SqlParameter("@prefilename", SqlDbType.NVarChar, 0xff) };
            cmdParms[0].Value = fcode;
            cmdParms[1].Value = filename;
            cmdParms[2].Value = filesize;
            cmdParms[3].Value = outfilename;
            cmdParms[4].Value = 0;
            cmdParms[5].Value = func.GetIp();
            cmdParms[6].Value = pagebase.Get("u");
            cmdParms[7].Value = filedir;
            cmdParms[8].Value = 0;
            cmdParms[9].Value = truedir;
            cmdParms[10].Value = ConfigurationManager.AppSettings["snsurl"].Trim().ToLower()+"sendok&";//转码服务更新路径
            cmdParms[11].Value = filetype;
            cmdParms[12].Value = oldname;
            return (Conn.ExecuteNonQuery(builder.ToString(), cmdParms) == 1);
        }

        public static int saveDB2(string filecode, string filename, string filesize, string outfilename, string stat, string addtime, string filedir, string linkflv, string truedir, string webserver, string filetype, string addip)
        {
            string autoimg = "3";
            StringBuilder builder = new StringBuilder();
            builder.Append("insert into ov_files (");
            builder.Append("filecode,filename,filesize,outfilename,stat,addtime,filedir,linkflv,truedir,webserver,filetype,addip,autoimg)");
            builder.Append("values (");
            builder.AppendFormat("'{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", filecode, filename, filesize, outfilename, stat, Convert.ToDateTime(addtime), filedir, linkflv, truedir, webserver, filetype, addip, autoimg);
            builder.Append(";");
            return Conn.ExecuteNonQuery(builder.ToString());
        }
        public static int saveDB3(string filecode, string filename, string filesize, string outfilename, string stat, string addtime, string filedir, string linkflv, string truedir, string webserver, string filetype, string addip,string prefilename)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("insert into ov_files (");
            builder.Append("filecode,filename,filesize,outfilename,stat,addtime,filedir,linkflv,truedir,webserver,filetype,addip,prefilename)");
            builder.Append("values (");
            builder.AppendFormat("'{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", filecode, filename, filesize, outfilename, stat, Convert.ToDateTime(addtime), filedir, linkflv, truedir, webserver, filetype, addip,prefilename);
            builder.Append(";");
            return Conn.ExecuteNonQuery(builder.ToString());
        }

    }
}

