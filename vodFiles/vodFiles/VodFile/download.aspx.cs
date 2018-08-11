using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using VodFile;
namespace VodFile
{
    public partial class download : System.Web.UI.Page
    {

        public void Error()
        {
            Response.Redirect("http://www.jianpianzi.com");
            Response.Close();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string filecode = Request["filecode"]; //del  isconvert 
            string type = Request["Type"];//0 1 2 3
            if (filecode == null || type==null)
            {
                Error();
                return;
            }
            else
            {
                SqlDataReader sdr = Conn.ExecuteReader("select * from ov_files where filecode='" + filecode + "'");
                if (sdr != null)
                {
                    if (sdr.Read())
                    {
                        /////取出数据 
                        string filedir = sdr["filedir"].ToString();
                        string truedir = sdr["truedir"].ToString();
                        string outfilename = sdr["outfilename"].ToString();
                        string prefilename=sdr["prefilename"].ToString();
                        string filename = sdr["filename"].ToString();
                       // string ext = filename.Substring(filename.LastIndexOf('.'));
                        string path = "";
                        path = truedir + @"tmpfiles\" + filename;
                        path=path.Replace(@"\\", @"\");
                        path = path.Replace(@"/", @"\");
                        path=path.Replace(@"\\", @"\");
                        Response.AddHeader("Content-type","application/octet-stream");
                        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + prefilename + "\"");
                        Response.WriteFile(path);
                        ////设置header 





                        /////输出文件 ...ok
                    }
                    else
                    {
                        Error();     
                    }
                }
                else
                {
                    Error();
                }
            }


        }
    }
}