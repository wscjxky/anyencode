using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
using VodFile.common;

namespace VodFile
{
    public partial class statusQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // DataSet ds = MySqlHelper.ExecuteDataset(MyConn.ConnectionString, "select * from ov_files order by id desc");
                DataSet ds = Conn.GetDataSet("select * from ov_files order by id desc");
                DataTable dt = ds.Tables[0];
                this.datalist1.DataSource = dt.DefaultView;
                this.datalist1.DataBind();
            }
        }
        protected void lbtnOpenFile_Click(object sender, CommandEventArgs e)
        {
            string fileurl = e.CommandArgument.ToString().Replace("/",@"\");
            System.Diagnostics.Process.Start("explorer.exe", @fileurl);
        }
        public string url(string truedir,string filedir,string outfileName) {
            string u = truedir + filedir.Replace("/", @"\") + @"\" + outfileName;
            return u.Replace("\\","\\\\");
        }
    }
}