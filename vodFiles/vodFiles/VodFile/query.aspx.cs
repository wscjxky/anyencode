using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VodFile;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace VodFile
{
    public partial class query : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {   
            Response.AddHeader("Access-Control-Allow-Origin","*");
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            string action=Request["action"]; //del  isconvert  
            string filecode = Request["filecode"];//文件filecode    filecode=xxx-xxx-xxx-xxx
            if (action == null || filecode == null)
            {
                Response.Write("非法访问!");
                HttpContext.Current.Response.End();
            }
            string[] codeArr = filecode.Split(new char[] { '-' }); //切分
            string inFilecodes = "";
            for (int i = 0; i < codeArr.Length-1; i++)
            {
                codeArr[i] = ("'" + codeArr[i] + "'");
                inFilecodes += (codeArr[i] + ",");
            }
            inFilecodes += "'"+codeArr[codeArr.Length-1]+"'";

            try
            {
                if (action == "isconvert")
                {
                    SqlDataReader dr = Conn.ExecuteReader("select stat,filecode,times,autoimg,flvsize,filetype  from ov_files where filecode in(" + inFilecodes + ")");//查询

                    if (dr.Read())
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();//json字典
                        if (dr["stat"].ToString() == "1")
                        {
                            dic.Add("result", "true");
                            dic.Add("filecode", dr["filecode"].ToString());  //filecode
                            dic.Add("times", dr["times"].ToString());
                            dic.Add("autoimg", dr["autoimg"].ToString());
                            dic.Add("flvsize", dr["flvsize"].ToString());
                            dic.Add("filetype", dr["filetype"].ToString());
                        }
                        else if (dr["stat"].ToString() == "0" || dr["stat"].ToString() == "2")
                            dic.Add("result", "false");
                        else if (dr["stat"].ToString() == "4")
                            dic.Add("result", "failure");
                        list.Add(dic);
                        while (dr.Read())
                        {
                            dic = new Dictionary<string, string>();//json字典
                            if (dr["stat"].ToString() == "1")  //转换成功
                            {
                                dic.Add("result", "true");
                                dic.Add("filecode", dr["filecode"].ToString());  //filecode
                                dic.Add("times", dr["times"].ToString());
                                dic.Add("autoimg", dr["autoimg"].ToString());
                                dic.Add("flvsize", dr["flvsize"].ToString());
                                dic.Add("filetype", dr["filetype"].ToString());
                            }
                            else if (dr["stat"].ToString() == "0" || dr["stat"].ToString() == "2")
                                dic.Add("result", "false");
                            else if (dr["stat"].ToString() == "3")
                                dic.Add("result", "failure");
                            list.Add(dic);
                        }
                        //关闭链接，必须手工关闭
                        Conn.CloseConnectionReader();
                        Response.Write(vodfile.JsonStringModule.ToJsonString(list));

                    }
                    else {
                        Conn.CloseConnectionReader();
                        Response.Write("{result:'false',info:'文件不存在'}");
                    }
                }
				//[[add by 境界@20160820:增加接口处理delete file的请求
				else if (action == "delete")
				{
					string perID = Request["perID"];
					if (perID != "73d2428effef619d51d8bcca966f7d98")
					{
						Response.Write("perID Invalid");
						//关闭链接，必须手工关闭
						Conn.CloseConnectionReader();
					}
					else
					{
						//从数据库的ov_files表中查询filecode对应的记录，记录项需要：filename, filedir , truedir, outfilename.
						//filename 是上传的源文件，需要删除；outfilename是转码完毕的目标文件，需要删除。还需要删除图标文件。还需要删除云存储上的文件。但此表中没有云存储的路径。
						SqlDataReader dr = Conn.ExecuteReader("select filecode,filename,filedir,truedir, outfilename  from ov_files where filecode in(" + inFilecodes + ")");//查询
						bool bDeleted = false;
						bool bFileNotexsit = true;
						bool brecodeExsit = false;
						if (dr.Read())
						{	
							bDeleted = true;
						    bFileNotexsit = false;
							brecodeExsit = true;

							//删除本地文件
							string strFileDir = dr["filedir"].ToString();
							string strSrcFileName = dr["filename"].ToString();
							string strDstFileName = dr["outfilename"].ToString();
							string strTrueDir = dr["truedir"].ToString();
							//拼接源文件路径
							string strSrcPathName = strTrueDir + @"tmpfiles\" + strSrcFileName;
							strSrcPathName=strSrcPathName.Replace(@"\\", @"\");
							strSrcPathName = strSrcPathName.Replace(@"/", @"\");
							strSrcPathName=strSrcPathName.Replace(@"\\", @"\");
							//删除源文件
							if (System.IO.File.Exists(strSrcPathName))
								System.IO.File.Delete(strSrcPathName);

							//拼接目标文件路径
							string strDstPathName = strTrueDir + strFileDir + @"\" + strDstFileName;
							strDstPathName=strDstPathName.Replace(@"\\", @"\");
							strDstPathName = strDstPathName.Replace(@"/", @"\");
							strDstPathName=strDstPathName.Replace(@"\\", @"\");
							//删除转码目标文件
							if (System.IO.File.Exists(strDstPathName))
								System.IO.File.Delete(strDstPathName);

							//删除图标文件
							string strImgpath = strTrueDir + strFileDir + @"\img\";
							strImgpath=strImgpath.Replace(@"\\", @"\");
							strImgpath = strImgpath.Replace(@"/", @"\");
							strImgpath=strImgpath.Replace(@"\\", @"\");
							foreach(string FileName in System.IO.Directory.GetFiles(strImgpath, dr["filecode"].ToString() + @"_*.jpg"))
							{
								string strFile = System.IO.Path.GetFileName(FileName);
								if (System.IO.File.Exists(strFile))
									System.IO.File.Delete(strFile);
							}


						    while (dr.Read())
						    {
								//删除本地文件
								strFileDir = dr["filedir"].ToString();
								strSrcFileName = dr["filename"].ToString();
								strDstFileName = dr["outfilename"].ToString();
								strTrueDir = dr["truedir"].ToString();
								//拼接源文件路径
								strSrcPathName = strTrueDir + @"tmpfiles\" + strSrcFileName;
								strSrcPathName=strSrcPathName.Replace(@"\\", @"\");
								strSrcPathName = strSrcPathName.Replace(@"/", @"\");
								strSrcPathName=strSrcPathName.Replace(@"\\", @"\");
								//删除源文件
								if (System.IO.File.Exists(strSrcPathName))
									System.IO.File.Delete(strSrcPathName);

								//拼接目标文件路径
								strDstPathName = strTrueDir + strFileDir + @"\" + strDstFileName;
								strDstPathName=strDstPathName.Replace(@"\\", @"\");
								strDstPathName = strDstPathName.Replace(@"/", @"\");
								strDstPathName=strDstPathName.Replace(@"\\", @"\");
								//删除转码目标文件
								if (System.IO.File.Exists(strDstPathName))
									System.IO.File.Delete(strDstPathName);

								//删除图标文件
								strImgpath = strTrueDir + strFileDir + @"\img\";
								strImgpath=strImgpath.Replace(@"\\", @"\");
								strImgpath = strImgpath.Replace(@"/", @"\");
								strImgpath=strImgpath.Replace(@"\\", @"\");
								foreach(string FileName in System.IO.Directory.GetFiles(strImgpath, dr["filecode"].ToString() + @"_*.jpg"))
								{
									string strFile = System.IO.Path.GetFileName(FileName);
									if (System.IO.File.Exists(strFile))
										System.IO.File.Delete(strFile);
								}

						    }
                       
                        
						    Response.Write(vodfile.JsonStringModule.ToJsonString(list));

						 }
						 

						 if (brecodeExsit)
						 {
						 	//删除数据库记录
							Conn.ExecuteNonQuery("delete from ov_files where filecode in filecode in(" + inFilecodes + ")");
						 }
						//回送结果
						if (bDeleted || bFileNotexsit)
						{
							//已经删除，或者文件不存在
							//回送结果
							Response.Write("Delete File Done");
						}
						//关闭数据库链接，必须手工关闭
						Conn.CloseConnectionReader();
					}

				}
				//]]

            }
            catch
            {
                Response.Write("{result:'false',info:'服务调用失败'}");
            }

        }
    }
}