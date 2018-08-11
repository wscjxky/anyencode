<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="statusQuery.aspx.cs" Inherits="VodFile.statusQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文件转换状态</title>
    <script src="ckplayer/ckplayer.js" type="text/javascript"></script>
    <script type="text/javascript">
        function reloadPage() {
            location.reload();
        }
        function ckplayer(prefilename) {
            //alert("'" + prefilename + "'");
            //ck({ f: "'"+prefilename+"'" });
            //ck({ f: 'E:\锐杰网格\工作\视频转换\vodfile\VodFile\files\2014-04\m44z37cbh6it0d7t.mp4' });
            ck({ f: 'rtmp://172.16.172.211/livepkgr/livestream' });
          }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="http://localhost:9968/newindex.aspx">返回</a> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a
            href="#" onclick="reloadPage()">刷新</a>
        <br />
        <br />
    </div>
    <asp:DataList ID="datalist1" runat="server" Width="100%">
        <HeaderTemplate>
            <table width="98%">
                <tr width="98%">
                    <td width="5%">
                        编号
                    </td>
                    <td width="48%">
                        原文件名称
                    </td>
                    <td width="15%">
                        上传时间
                    </td>
                    <td width="22%">
                        状态(0:等待转换，1:成功，2:正在转)
                    </td>
                    <td width="8%">
                        错误次数
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr width="98%">
                <td width="5%">
                    <%# Eval("id") %>
                </td>
                <td width="48%">
                    <!--<asp:LinkButton ID="lbtnOpenFile" runat="server" CommandArgument='<# url(Eval("truedir").ToString(),Eval("filedir").ToString(),Eval("outfileName").ToString()) %>' OnCommand="lbtnOpenFile_Click"><# Eval("prefilename")%></asp:LinkButton>-->
                    <a href="javascript:ckplayer('<%# url(Eval("truedir").ToString(),Eval("filedir").ToString(),Eval("outfileName").ToString())%>');"><%# Eval("prefilename")%></a>
                </td>
                <td width="15%">
                    <%# Eval("addtime")%>
                </td>
                <td width="22%">
                    <%# Eval("stat")%>
                </td>
                <td width="8%">
                    <%# Eval("errcount")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </asp:DataList>
    </form>
</body>
</html>
