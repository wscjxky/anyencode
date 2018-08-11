namespace VodFile
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;

    public class files : pagebase
    {
        /// <summary>
        /// 删除正式视频文件
        /// </summary>
        private void del_file()
        {
            string str = pagebase.Get("fcode");
            SqlParameter parameter = Conn.MakeInParam("@fcode", SqlDbType.Char, 0x10, str);
            if (Conn.ExecuteNonQuery("update ov_files set isdel=1 where filecode=@fcode", new SqlParameter[] { parameter }) > 0)
            {
                base.Response.Write("1");
            }
            else if (!Conn.Exists("select * from ov_files where filecode=@fcode", new SqlParameter[] { parameter }))
            {
                base.Response.Write("1");
            }
            else
            {
                base.Response.Write("0");
            }
        }
        //删除视频已上传未转吗的临时文件
        private void clear_file()
        {
            string str = pagebase.Get("fcode");
            SqlParameter parameter = Conn.MakeInParam("@fcode", SqlDbType.Char, 0x10, str);
            SqlDataReader sdr = Conn.ExecuteReader("select * from ov_files  where filecode=@fcode", new SqlParameter[] { parameter });
            if (sdr.Read())
            {
                //如果转换完成，删除原先文件
                try
                {
                    if (sdr["stat"].ToString() == "1")
                    {
                        string filename = sdr["truedir"].ToString() + "tmpfiles\\" + sdr["filename"].ToString();

                        FileInfo fi = new FileInfo(filename);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;

                        File.Delete(filename);

                    }
                }
                catch (Exception e)
                {
                    base.Response.Write("0");//删除失败
                }

                base.Response.Write("1");//删除成功
            }
            else
            {
                base.Response.Write("1");//已经删除
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string str2 = pagebase.Get("action");
            if (str2 == "delete")
            {
                this.del_file();
            }
            if (str2 == "clear")
            {
                this.clear_file();
            }
        }
    }
}

