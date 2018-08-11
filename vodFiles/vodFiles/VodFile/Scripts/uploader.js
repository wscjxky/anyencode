var swfu;

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
        $("#file_tr_" + fileId).get(0).cells[2].innerHTML = html;
    },

    updateOpt: function (fileId, html) {
        $("#file_tr_" + fileId).get(0).cells[3].innerHTML = html;
    },

    Handler: {
        fileQueued: function (file) {
            $("#photo-uploader-files-list").append("<tr id='file_tr_" + file.id + "'><td>" + file.name + "</td><td>" + Uploader.getFileSize(file.size) + "</td><td>等候上传</td><td class='opt'><a href='#'>删除</a></td></tr>");
        },

        uploadComplete: function (file) {
            alert('文件上传成功！');
            //递归实现自动批量上传
            this.startUpload();

        },

        uploadStart: function (file) {
            //开始上传此文件
            Uploader.updateStatus(file.id, "开始上传");
        },

        uploadProgress: function (filce, bytesLoaded, bytesTotal) {
            //	var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);
            //Uploader.updateStatus(file.id, percent + "%");
        },

        uploadSuccess: function (file, serverData) {
            Uploader.updateStatus(file.id, "上传完毕");
            var data = eval("(" + serverData + ")");
            Uploader.updateOpt(file.id, "<a href='" + data.fileUrl + "' target='_blank'>查看文件</a>");
        },

        fileQueueError: function (file, errorCode, message) {
            console.log(file);
            console.log(errorCode);
            console.log(message);
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
		button_height : 20,
		button_text : "<b>添加文件</b>",
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