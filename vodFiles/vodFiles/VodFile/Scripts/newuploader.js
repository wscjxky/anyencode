var swfu;
var index = 0;
var count= 0;
var Uploader = {
    getFileSize: function (num) {
        if (isNaN(num)) {
            return false;
        }
        num = parseInt(num);
        var units = [" B", " KB", " MB", " GB"];
        for (var i = 0; i < units.length; i += 1) {
            if (num < 1024) {
                num = num + "";
                if (num.indexOf(".") != -1 && num.indexOf(".") != 3) {
                    num = num.substring(0, 4);
                } else {
                    num = num.substring(0, 3);
                }
                break;
            } else {
                num = num / 1024;
            }
        }
        return num + units[i];
    },

    updateStatus: function (fileId, html) {
        //  $("#file_tr_" + fileId).get(0).cells[2].innerHTML = html;
    },

    updateOpt: function (fileId, html) {
        //$("#file_tr_" + fileId).get(0).cells[3].innerHTML = html;
    },

    Handler: {
        fileQueued: function (file) {
          //  swfu.cancelUpload();
            // $("#photo-uploader-files-list").append("<tr id='file_tr_" + file.id + "'><td>" + file.name + "</td><td>" + Uploader.getFileSize(file.size) + "</td><td>等候上传</td><td class='opt'><a href='#'>删除</a></td></tr>");
            if (index == 0) {
                $("#up_queue").append("<tr  class='up_queue_item one'   id='file_tr_" + file.id + "'><td>" + file.name + "</td><td >" + Uploader.getFileSize(file.size) + "</td><td class='progress' >等候上传</td><td class='opt'><a href='#'>删除</a></td></tr>");
                index = 1;
            }
            else if (index == 1) {
                $("#up_queue").append("<tr  class='up_queue_item two'   id='file_tr_" + file.id + "'><td>" + file.name + "</td><td >" + Uploader.getFileSize(file.size) + "</td><td class='progress'>等候上传</td><td class='opt'><a href='#'>删除</a></td></tr>");
                index = 0;
            }
            $("#file_tr_" + file.id + "  a").click(function () {
                if (($("#file_tr_" + file.id + "  a").text()) == "删除") {
                    $("#up_queue  #file_tr_" + file.id).remove(); //删除元素
                    swfu.cancelUpload(file.id, null);  //从队列删除文件
                }
                else {
                    ;
                }
            });
        },

        uploadComplete: function (file) {
            ;
            //递归实现自动批量上传
            this.startUpload();

        },

        uploadStart: function (file) {
            //开始上传此文件
            Uploader.updateStatus(file.id, "开始上传");
        },

        uploadProgress: function (file, bytesLoaded, bytesTotal) {
            var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);
            $("#file_tr_" + file.id + " td.progress").html(percent + "%");

        },

        uploadSuccess: function (file, serverData) {
            //  Uploader.updateStatus(file.id, "上传完毕");
            //   var data = eval("(" + serverData + ")");
            //   $("#file_tr_" + file.id + " a").attr("href", data.fileUrl).html("查看文件");
            //  $("#file_tr_" + file.id + " a").attr("click", "");
            alert(serverData);

        },

        fileQueueError: function (file, errorCode, message) {

        }
    }
};

$(function(){
	swfu = new SWFUpload({
		//debug:true,
		upload_url : "upload.aspx",
		flash_url : "swfupload/swfupload.swf",
		file_size_limit : "500 MB",
		file_types : "*.*",
		file_types_description : "All Image Files",
		file_post_name: "file",
		use_query_string: true,  //传递参数否则无法使用
		post_params: {           //传递给应用的参数
		    "action": "save",
            "from":"",//请求的应用 
            "filetype": ""//文件属性  
	    },
		button_placeholder_id : "spanSWFUploadButton",
		button_width : 60,
		button_height : 30,
		button_text : "<b>Add Files</b>",
		button_text_left_padding : 3,
		button_text_top_padding : 2,
		button_cursor : SWFUpload.CURSOR.HAND,
		
		//handler
		file_queued_handler : Uploader.Handler.fileQueued,
		file_queue_error_handler : Uploader.Handler.fileQueueError,
		upload_complete_handler : Uploader.Handler.uploadComplete,
		upload_start_handler : Uploader.Handler.uploadStart,
		upload_progress_handler : Uploader.Handler.uploadProgress,
		upload_success_handler : Uploader.Handler.uploadSuccess
	});
})

function lock() {
    
    var m = "mask";
    var newMask = document.createElement("div"); //动态创建div  利用document对象
    newMask.id = m; //设置div的id属性 通过js 
    newMask.style.position = "absolute"; //设置绝对定位
    newMask.style.zIndex = "1000"; //设置z-index
    _scrollWidth = Math.max(document.body.scrollWidth, document.documentElement.scrollWidth); //宽度完全屏蔽 必须的
    _scrollHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight); //高度 必须的
    newMask.style.width = _scrollWidth + "px"; //掩码图片的宽度
    newMask.style.height = _scrollHeight + "px"; //掩码图片的高度 
    newMask.style.top = "0px"; //顶部
    newMask.style.left = "0px"; //左边
    newMask.style.background = "#122222"; //屏蔽颜色必须设置否则不生效 
    newMask.style.filter = "Alpha(opacity=70)"; //实现透明的关键代码 设置透明度针对IE  css的 filter属性
    newMask.style.opacity = "0.8"; //W3C标准透明
    document.body.appendChild(newMask);  //增加屏蔽图层到body
    
}
