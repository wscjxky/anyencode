namespace anyEncoder
{
    using DevComponents.DotNetBar;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Windows.Forms;

    public class main : Form
    {
        private Bar bar1;
        private IContainer components = null;
        private DockContainerItem dockContainerItem1;
        private DockContainerItem dockContainerItem2;
        private DockSite dockSite1;
        private DockSite dockSite2;
        private DockSite dockSite3;
        private DockSite dockSite4;
        private DockSite dockSite5;
        private DockSite dockSite6;
        private DockSite dockSite7;
        private DockSite dockSite8;
        private MdiClient mdiClient1;
        private DotNetBarManager NetBarManager;
        private PanelDockContainer panelDockContainer1;
        private PanelDockContainer panelDockContainer2;
        private int runtask;
        private TextBox syslogbox;
        private TabStrip tabStrip1;
        private System.Windows.Forms.Timer timer_send;

        public main()
        {
            this.InitializeComponent();
            this.Text = this.Text + ", build " + System.IO.File.GetLastWriteTime(base.GetType().Assembly.Location).ToString();
        }

        private void AppendLog(string logstr)
        {
            if (this.syslogbox.Lines.Length > 0xfa0)
            {
                this.syslogbox.Clear();
            }
            this.syslogbox.AppendText(DateTime.Now.ToString() + " : " + logstr + Environment.NewLine);
        }

        private void delvideo()
        {
            try
            {
                using (DataView view = Conn.GetDataSet("select top 10 * from ov_files where isdel=1").Tables[0].DefaultView)
                {
                    if (view.Count > 0)
                    {
                        for (int i = 0; i < view.Count; i++)
                        {
                            string str = view[i]["filecode"].ToString();
                            string str2 = view[i]["truedir"].ToString() + view[i]["filedir"].ToString();
                            string path = view[i]["truedir"].ToString() + @"tmpfiles\" + view[i]["filename"].ToString();
                            string str4 = str2 + @"\" + view[i]["outfilename"].ToString();
                            string str5 = str4.Replace(".flv", ".swf").Replace(".mp4", ".swf");
                            string str6 = str4.Replace(".flv", ".mp4");
                            string str7 = str4.Replace(".mp4", "_3g.mp4").Replace(".flv", "_3g.mp4");
                            try
                            {
                                if (view[i]["filetype"].ToString() == "1")
                                {
                                    func.DelDir(str4);
                                    Directory.Delete(str4);
                                }
                                else
                                {
                                    System.IO.File.Delete(str4);
                                    System.IO.File.Delete(str6);
                                    System.IO.File.Delete(str7);
                                    System.IO.File.Delete(str5);
                                }
                                System.IO.File.Delete(path);
                                for (int j = 1; j <= 6; j++)
                                {
                                    string str8 = string.Concat(new object[] { str2, @"\img\", str, "_", j, ".jpg" });
                                    System.IO.File.Delete(str8);
                                    System.IO.File.Delete(str8 + "_b.jpg");
                                }
                            }
                            catch
                            {
                            }
                            Conn.ExecuteNonQuery("delete ov_files where filecode='" + str + "'");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.AppendLog("删除文件错误：" + exception.Message.ToString());
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

        private void InitializeComponent()
        {
            this.components = new Container();
            this.mdiClient1 = new MdiClient();
            this.timer_send = new System.Windows.Forms.Timer(this.components);
            this.NetBarManager = new DotNetBarManager(this.components);
            this.dockSite4 = new DockSite();
            this.bar1 = new Bar();
            this.panelDockContainer1 = new PanelDockContainer();
            this.syslogbox = new TextBox();
            this.panelDockContainer2 = new PanelDockContainer();
            this.dockContainerItem1 = new DockContainerItem();
            this.dockContainerItem2 = new DockContainerItem();
            this.dockSite1 = new DockSite();
            this.dockSite2 = new DockSite();
            this.dockSite8 = new DockSite();
            this.dockSite5 = new DockSite();
            this.dockSite6 = new DockSite();
            this.dockSite7 = new DockSite();
            this.dockSite3 = new DockSite();
            this.tabStrip1 = new TabStrip();
            this.dockSite4.SuspendLayout();
            this.bar1.BeginInit();
            this.bar1.SuspendLayout();
            this.panelDockContainer1.SuspendLayout();
            base.SuspendLayout();
            this.mdiClient1.BackColor = Color.FromArgb(0xc2, 0xd9, 0xf7);
            this.mdiClient1.Dock = DockStyle.Fill;
            this.mdiClient1.Location = new Point(0, 0x1a);
            this.mdiClient1.Name = "mdiClient1";
            this.mdiClient1.Size = new Size(0x265, 0x109);
            this.mdiClient1.TabIndex = 0x19;
            this.timer_send.Enabled = true;
            this.timer_send.Interval = 0x1388;
            this.timer_send.Tick += new EventHandler(this.timer_send_Tick);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.F1);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlC);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlA);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlV);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlX);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlZ);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlY);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.Del);
            this.NetBarManager.AutoDispatchShortcuts.Add(eShortcut.Ins);
            this.NetBarManager.BottomDockSite = this.dockSite4;
            this.NetBarManager.DefinitionName = "";
            this.NetBarManager.EnableFullSizeDock = false;
            this.NetBarManager.LeftDockSite = this.dockSite1;
            this.NetBarManager.ParentForm = this;
            this.NetBarManager.RightDockSite = this.dockSite2;
            this.NetBarManager.Style = eDotNetBarStyle.Office2007;
            this.NetBarManager.ToolbarBottomDockSite = this.dockSite8;
            this.NetBarManager.ToolbarLeftDockSite = this.dockSite5;
            this.NetBarManager.ToolbarRightDockSite = this.dockSite6;
            this.NetBarManager.ToolbarTopDockSite = this.dockSite7;
            this.NetBarManager.TopDockSite = this.dockSite3;
            this.dockSite4.Controls.Add(this.bar1);
            this.dockSite4.Dock = DockStyle.Bottom;
            this.dockSite4.DocumentDockContainer = new DocumentDockContainer(new DocumentBaseContainer[] { new DocumentBarContainer(this.bar1, 0x265, 0x8f) }, eOrientation.Vertical);
            this.dockSite4.Location = new Point(0, 0x123);
            this.dockSite4.Name = "dockSite4";
            this.dockSite4.Size = new Size(0x265, 0x92);
            this.dockSite4.TabIndex = 0x1d;
            this.dockSite4.TabStop = false;
            this.bar1.AccessibleDescription = "DotNetBar Bar (bar1)";
            this.bar1.AccessibleName = "DotNetBar Bar";
            this.bar1.AutoSyncBarCaption = true;
            this.bar1.CloseSingleTab = true;
            this.bar1.Controls.Add(this.panelDockContainer1);
            this.bar1.Controls.Add(this.panelDockContainer2);
            this.bar1.GrabHandleStyle = eGrabHandleStyle.Caption;
            this.bar1.Items.AddRange(new BaseItem[] { this.dockContainerItem1, this.dockContainerItem2 });
            this.bar1.LayoutType = eLayoutType.DockContainer;
            this.bar1.Location = new Point(0, 3);
            this.bar1.Name = "bar1";
            this.bar1.SelectedDockTab = 0;
            this.bar1.Size = new Size(0x265, 0x8f);
            this.bar1.Stretch = true;
            this.bar1.Style = eDotNetBarStyle.Office2007;
            this.bar1.TabIndex = 0;
            this.bar1.TabStop = false;
            this.bar1.Text = "日志";
            this.panelDockContainer1.ColorSchemeStyle = eDotNetBarStyle.Office2007;
            this.panelDockContainer1.Controls.Add(this.syslogbox);
            this.panelDockContainer1.Location = new Point(3, 0x17);
            this.panelDockContainer1.Name = "panelDockContainer1";
            this.panelDockContainer1.Size = new Size(0x25f, 0x5c);
            this.panelDockContainer1.Style.Alignment = StringAlignment.Center;
            this.panelDockContainer1.Style.BackColor1.ColorSchemePart = eColorSchemePart.BarBackground;
            this.panelDockContainer1.Style.BackColor2.ColorSchemePart = eColorSchemePart.BarBackground2;
            this.panelDockContainer1.Style.BorderColor.ColorSchemePart = eColorSchemePart.BarDockedBorder;
            this.panelDockContainer1.Style.ForeColor.ColorSchemePart = eColorSchemePart.ItemText;
            this.panelDockContainer1.Style.GradientAngle = 90;
            this.panelDockContainer1.TabIndex = 0;
            this.syslogbox.Dock = DockStyle.Fill;
            this.syslogbox.Location = new Point(0, 0);
            this.syslogbox.Multiline = true;
            this.syslogbox.Name = "syslogbox";
            this.syslogbox.ScrollBars = ScrollBars.Vertical;
            this.syslogbox.Size = new Size(0x25f, 0x5c);
            this.syslogbox.TabIndex = 0;
            this.panelDockContainer2.ColorSchemeStyle = eDotNetBarStyle.Office2007;
            this.panelDockContainer2.Location = new Point(3, 0x17);
            this.panelDockContainer2.Name = "panelDockContainer2";
            this.panelDockContainer2.Size = new Size(0x25f, 0x5c);
            this.panelDockContainer2.Style.Alignment = StringAlignment.Center;
            this.panelDockContainer2.Style.BackColor1.ColorSchemePart = eColorSchemePart.BarBackground;
            this.panelDockContainer2.Style.BackColor2.ColorSchemePart = eColorSchemePart.BarBackground2;
            this.panelDockContainer2.Style.BorderColor.ColorSchemePart = eColorSchemePart.BarDockedBorder;
            this.panelDockContainer2.Style.ForeColor.ColorSchemePart = eColorSchemePart.ItemText;
            this.panelDockContainer2.Style.GradientAngle = 90;
            this.panelDockContainer2.TabIndex = 2;
            this.dockContainerItem1.Control = this.panelDockContainer1;
            this.dockContainerItem1.Name = "dockContainerItem1";
            this.dockContainerItem1.Text = "日志";
            this.dockContainerItem2.Control = this.panelDockContainer2;
            this.dockContainerItem2.Name = "dockContainerItem2";
            this.dockContainerItem2.Text = "系统";
            this.dockSite1.Dock = DockStyle.Left;
            this.dockSite1.DocumentDockContainer = new DocumentDockContainer();
            this.dockSite1.Location = new Point(0, 0);
            this.dockSite1.Name = "dockSite1";
            this.dockSite1.Size = new Size(0, 0x123);
            this.dockSite1.TabIndex = 0x1a;
            this.dockSite1.TabStop = false;
            this.dockSite2.Dock = DockStyle.Right;
            this.dockSite2.DocumentDockContainer = new DocumentDockContainer();
            this.dockSite2.Location = new Point(0x265, 0);
            this.dockSite2.Name = "dockSite2";
            this.dockSite2.Size = new Size(0, 0x123);
            this.dockSite2.TabIndex = 0x1b;
            this.dockSite2.TabStop = false;
            this.dockSite8.Dock = DockStyle.Bottom;
            this.dockSite8.Location = new Point(0, 0x1b5);
            this.dockSite8.Name = "dockSite8";
            this.dockSite8.Size = new Size(0x265, 0);
            this.dockSite8.TabIndex = 0x21;
            this.dockSite8.TabStop = false;
            this.dockSite5.Dock = DockStyle.Left;
            this.dockSite5.Location = new Point(0, 0);
            this.dockSite5.Name = "dockSite5";
            this.dockSite5.Size = new Size(0, 0x1b5);
            this.dockSite5.TabIndex = 30;
            this.dockSite5.TabStop = false;
            this.dockSite6.Dock = DockStyle.Right;
            this.dockSite6.Location = new Point(0x265, 0);
            this.dockSite6.Name = "dockSite6";
            this.dockSite6.Size = new Size(0, 0x1b5);
            this.dockSite6.TabIndex = 0x1f;
            this.dockSite6.TabStop = false;
            this.dockSite7.Dock = DockStyle.Top;
            this.dockSite7.Location = new Point(0, 0);
            this.dockSite7.Name = "dockSite7";
            this.dockSite7.Size = new Size(0x265, 0);
            this.dockSite7.TabIndex = 0x20;
            this.dockSite7.TabStop = false;
            this.dockSite3.Dock = DockStyle.Top;
            this.dockSite3.DocumentDockContainer = new DocumentDockContainer();
            this.dockSite3.Location = new Point(0, 0);
            this.dockSite3.Name = "dockSite3";
            this.dockSite3.Size = new Size(0x265, 0);
            this.dockSite3.TabIndex = 0x1c;
            this.dockSite3.TabStop = false;
            this.tabStrip1.AutoSelectAttachedControl = true;
            this.tabStrip1.CanReorderTabs = true;
            this.tabStrip1.CloseButtonVisible = false;
            this.tabStrip1.Dock = DockStyle.Top;
            this.tabStrip1.Location = new Point(0, 0);
            this.tabStrip1.MdiTabbedDocuments = true;
            this.tabStrip1.Name = "tabStrip1";
            this.tabStrip1.SelectedTab = null;
            this.tabStrip1.SelectedTabFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.tabStrip1.Size = new Size(0x265, 0x1a);
            this.tabStrip1.Style = eTabStripStyle.Office2007Document;
            this.tabStrip1.TabAlignment = eTabStripAlignment.Top;
            this.tabStrip1.TabIndex = 0x22;
            this.tabStrip1.TabLayoutType = eTabLayoutType.MultilineWithNavigationBox;
            this.tabStrip1.Text = "tabStrip1";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.ClientSize = new Size(0x265, 0x1b5);
            base.Controls.Add(this.tabStrip1);
            base.Controls.Add(this.dockSite2);
            base.Controls.Add(this.dockSite1);
            base.Controls.Add(this.dockSite3);
            base.Controls.Add(this.dockSite4);
            base.Controls.Add(this.dockSite5);
            base.Controls.Add(this.dockSite6);
            base.Controls.Add(this.dockSite7);
            base.Controls.Add(this.dockSite8);
            base.Controls.Add(this.mdiClient1);
            base.IsMdiContainer = true;
            base.Name = "main";
            this.Text = "anyEncoder编码器";
            base.FormClosed += new FormClosedEventHandler(this.main_FormClosed);
            base.Load += new EventHandler(this.main_Load);
            this.dockSite4.ResumeLayout(false);
            this.bar1.EndInit();
            this.bar1.ResumeLayout(false);
            this.panelDockContainer1.ResumeLayout(false);
            this.panelDockContainer1.PerformLayout();
            base.ResumeLayout(false);
        }

        private void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            func.killall();
        }

        private void main_Load(object sender, EventArgs e)
        {
            try
            {
                Conn.ExecuteNonQuery("update ov_files set stat=0 where stat=2 and isdel=0");
                foreach (string str in Directory.GetFileSystemEntries(Application.StartupPath + @"\log"))
                {
                    if ((func.Right(str, 4) == ".ini") || (func.Right(str, 4) == ".del"))
                    {
                        System.IO.File.Delete(str);
                    }
                }
            }
            catch (Exception exception)
            {
                this.AppendLog(exception.Message.ToString());
            }
            this.tabStrip1.MdiForm = this;
            if ((func.readdog(0) == "1") || func.chksn())
            {
                this.runtask = config.maxtask;
                for (int i = 1; i <= this.runtask; i++)
                {
                    EnForm form = new EnForm();
                    form.Name = "ef_" + i;
                    form.Text = "编码进程" + i;
                    form.taskid = i;
                    form.MdiParent = this;
                    form.WindowState = FormWindowState.Maximized;
                    form.Show();
                }
            }
            if ((func.readdog(1) == "1") || func.chksn())
            {
                DocForm form2 = new DocForm();
                form2.MdiParent = this;
                form2.WindowState = FormWindowState.Maximized;
                form2.Show();
            }
        }

        public string send(string WebSite, string strQuery)
        {
            WebSite = WebSite + "&rnd=" + func.GetRandomString(10);
            int num = 10;
            HttpWebResponse response = null;
            Encoding encoding = Encoding.UTF8;
            if (num == 0)
            {
                num = 0x2710;
            }
            else
            {
                num *= 0x3e8;
            }
            byte[] bytes = encoding.GetBytes(strQuery);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(WebSite);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = num;
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            try
            {
                response = (HttpWebResponse) request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                return exception.Message.ToString();
            }
        }

        private void sendok()
        {
            int maxerr = config.maxerr;
            ServicePointManager.Expect100Continue = false;
            try
            {
                using (DataView view = Conn.GetDataSet("select top 5 * from ov_files where (stat=1 or errcount>=" + maxerr + ") and sendok=0 order by senderr asc,id asc").Tables[0].DefaultView)
                {
                    if (view.Count > 0)
                    {
                        for (int i = 0; i < view.Count; i++)
                        {
                            string str = view[i]["webserver"].ToString();
                            string str2 = view[i]["filecode"].ToString();
                            string str3 = func.MD5(config.safecode + str2, 0x10);
                            string str4 = view[i]["filetype"].ToString();
                            string webSite = string.Concat(new object[] { 
                                str, "?action=sendok&stat=", view[i]["stat"], "&errcount=", view[i]["errcount"], "&fcode=", str2, "&scode=", str3, "&times=", view[i]["times"], "&autoimg=", view[i]["autoimg"], "&fsize=", view[i]["flvsize"], "&filetype=", 
                                str4, "&sourcefile=", view[i]["filename"]
                             });
                            string strQuery = "";
                            if (str4 == "1")
                            {
                                string path = string.Concat(new object[] { view[i]["truedir"], view[i]["filedir"].ToString(), @"\", view[i]["outfilename"], @"\", view[0]["filename"], ".txt" });
                                if (System.IO.File.Exists(path))
                                {
                                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                    string str8 = reader.ReadToEnd().Trim();
                                    if (!string.IsNullOrEmpty(str8))
                                    {
                                        strQuery = "&doctxt=" + HttpUtility.UrlEncode(str8);
                                    }
                                }
                            }
                            //发送指令到服务器执行send()方法--2014-10-24注释掉
                            //try
                            //{
                            //    string str9 = this.send(webSite, strQuery);
                            //    if ((str9 == "1") || (str9 == "-1"))
                            //    {
                            //        Conn.ExecuteNonQuery("update ov_files set sendok=1 where filecode='" + str2 + "'");
                            //    }
                            //    else
                            //    {
                            //        Conn.ExecuteNonQuery("update ov_files set senderr=senderr+1 where filecode='" + str2 + "'");
                            //        this.AppendLog("发送命令到web服务器失败，url=" + webSite + "," + str9);
                            //    }
                            //}
                            //catch (Exception exception)
                            //{
                            //    Conn.ExecuteNonQuery("update ov_files set senderr=senderr+1 where filecode='" + str2 + "'");
                            //    this.AppendLog("发送命令到web服务器失败，" + exception.Message.ToString() + "url=" + webSite);
                            //}
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                this.AppendLog("发送命令到web服务器错误：" + exception2.Message.ToString());
            }
        }

        private void timer_send_Tick(object sender, EventArgs e)
        {
            int num;
            this.timer_send.Enabled = false;
            int maxtask = config.maxtask;
            if (maxtask > this.runtask)
            {
                for (num = this.runtask + 1; num <= maxtask; num++)
                {
                    EnForm form = new EnForm();
                    form.Name = "ef_" + num;
                    form.Text = "编码进程" + num;
                    form.taskid = num;
                    form.MdiParent = this;
                    form.WindowState = FormWindowState.Maximized;
                    form.Show();
                }
                this.runtask = maxtask;
            }
            if (maxtask < this.runtask)
            {
                for (num = this.runtask; num > maxtask; num--)
                {
                    foreach (Form form2 in base.MdiChildren)
                    {
                        if (form2.Name == ("ef_" + num))
                        {
                            form2.Close();
                            form2.Dispose();
                        }
                    }
                }
                this.runtask = maxtask;
            }
            this.delvideo();
            this.sendok();
            this.timer_send.Enabled = true;
        }
    }
}

