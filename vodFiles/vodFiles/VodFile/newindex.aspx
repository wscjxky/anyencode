<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="zh-CN" dir="ltr">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="css/newupload.css" />
    <link rel="stylesheet" type="text/css" href="css/buttons.css" />
    <script type="text/javascript" src="Scripts/jquery-1.4.3.min.js"></script>
    <script type="text/javascript" src="Scripts/newuploader.js"></script>
    <script type="text/javascript" src="Scripts/swfupload.js"></script>
    <title>多文件文件上传</title>
    <script language="javascript" type="text/javascript">
        $(function () {

            //      alert(document.body.clientHeight);


        });
    </script>
</head>
<body>
    <table cellspacing="0" cellpadding="0">
        <thead>
            <tr id="up_queue_header">
                <td>
                    文件名
                </td>
                <td>
                    文件大小
                </td>
                <td>
                    上传进度
                </td>
                <td>
                    文件信息
                </td>
            </tr>
        </thead>
        <tbody id="up_queue">
        </tbody>
    </table>
    <div id="control">
        <button id="spanSWFUploadButton">
            选择文件</button>
        <a href="#" class="btn button blue" onclick="swfu.startUpload();">UpLoad File</a>
    </div>
    <br />
    <br />
    <br />
    <div>
        <a href="statusQuery.aspx" class="btn button blue">查看视频转换状态</a>
    </div>
</body>
</html>
