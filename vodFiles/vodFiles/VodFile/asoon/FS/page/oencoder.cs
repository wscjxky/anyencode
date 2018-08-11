namespace VodFile
{
    using System;
    using System.IO;
    using System.Web;

    public class oencoder : pagebase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (pagebase.Get("action") == "test")
            {
                this.test();
            }
            else
            {
                //this.save_upload();
            }
        }

        //protected void save_upload()
        //{
        //    //string[] strArray = pagebase.get_uinfo();
        //    if (strArray[0] == "0")
        //    {
        //        base.Response.Write(strArray[1]);
        //    }
        //    else
        //    {
        //        string str = "flv|fv|f4v|mp4";
        //        int @int = func.GetInt(strArray[2]);
        //        if (@int > 0x1f4000)
        //        {
        //            @int = 0x1f4000;
        //        }
        //        UpFiles files = new UpFiles {
        //            FileExtensions = str.Split(new char[] { '|' }),
        //            MaxSize = @int,
        //            SavePath = "tmpfiles"
        //        };
        //        files.SaveUploadFiles();
        //        if ((files.GetPromptMessage() != null) || (files.listFile.Count == 0))
        //        {
        //            base.Response.Write(files.GetPromptMessage());
        //            base.Response.End();
        //        }
        //        else
        //        {
        //            try
        //            {
        //                string fileName = files.listFile[0].fileName;
        //                int fileSize = files.listFile[0].fileSize;
        //                string oldname = files.listFile[0].oldname;
        //                string fcode = func.GetRandomString(0x10).ToLower();
        //                if (filedata.saveDB(fileName, oldname, fileSize, fcode, 0))
        //                {
        //                    base.Response.Write("true");
        //                }
        //                else
        //                {
        //                    File.Delete(HttpContext.Current.Server.MapPath(files.listFile[0].filePath));
        //                    base.Response.Write("写入数据库错误。");
        //                }
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //}

        protected void test()
        {
            //string[] strArray = pagebase.get_uinfo();
            //if (strArray[0] == "0")
            //{
            //    base.Response.Write(strArray[1]);
            //}
            //else
            //{
            //    base.Response.Write("true");
            //}
        }
    }
}

