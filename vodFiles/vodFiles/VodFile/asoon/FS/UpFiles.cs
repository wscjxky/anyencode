namespace VodFile
{
    using Brettle.Web.NeatUpload;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Web;
    public class UpFiles
    {
        public int errorCode;
        public int fileCount;
        private string fileExtension;
        private string[] fileExtensions;
        private string fileName;
        private FolderTypes folderType;
        public List<FilesInfo> listFile = new List<FilesInfo>();
        private long maxSize;
        private string savePath;
        private UploadTypes uploadType;

        private bool CheckContentLength(long size, long maxSize)
        {
            if (size > 0)
            {
                if (maxSize == 0)
                {
                    return true;
                }
                if (maxSize == -1)
                {
                    return false;
                }
                if ((size / 0x400) < maxSize)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckFileExtension(string extension)
        {
            if (extension.Length != 0)
            {
                if (this.fileExtensions == null)
                {
                    return true;
                }
                for (int i = 0; i < this.fileExtensions.Length; i++)
                {
                    if (string.Equals("." + this.fileExtensions[i].Trim(), extension, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CreateFolder()
        {
            if (this.fileName == null)
            {
                string folder = this.GetFolder();
                if (!Directory.Exists(this.savePath))
                {
                    try
                    {
                        Directory.CreateDirectory(this.savePath);
                        if (folder.Length == 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (!Directory.Exists(Path.Combine(this.savePath, folder)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(this.savePath, folder));
                        return true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return true;
        }

        public string GetFileName()
        {
            if (this.fileName != null)
            {
                return this.fileName;
            }
            switch (this.folderType)
            {
                case FolderTypes.Other:
                    return this.fileName;

                case FolderTypes.NewFolderDateFile:
                case FolderTypes.NoFolderDateFile:
                    return DateTime.Now.ToString("ddHHmmssfff", DateTimeFormatInfo.InvariantInfo);

                case FolderTypes.NewFolderRandomFile:
                case FolderTypes.NoFolderRandomFile:
                    return Path.GetRandomFileName();
            }
            return DateTime.Now.ToString("ddHHmmssfff", DateTimeFormatInfo.InvariantInfo);
        }

        private string GetFolder()
        {
            switch (this.folderType)
            {
                case FolderTypes.Other:
                    return string.Empty;

                case FolderTypes.NewFolderDateFile:
                case FolderTypes.NewFolderRandomFile:
                    return DateTime.Now.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo);

                case FolderTypes.NoFolderDateFile:
                case FolderTypes.NoFolderRandomFile:
                    return string.Empty;
            }
            return DateTime.Now.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo);
        }

        public string GetPromptMessage()
        {
            switch (this.errorCode)
            {
                case 1:
                    return "服务器不支持该上传组件";

                case 2:
                    return "未选定上传组件";

                case 3:
                    return "未选定上传文件";

                case 4:
                    return ("最大允许上传文件的大小为：" + this.maxSize + "KB");

                case 5:
                    return "文件类型（扩展名）不正确";

                case 6:
                    return "对上传目录/文件无修改权限，请在服务器上进行正确的设置";
            }
            return null;
        }
        /// <summary>
        /// 存储已上传数据文件
        /// </summary>
        private void SaveDefault()
        {

            if (HttpContext.Current.Request.Files.Count > 0)
            {

                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    if ((file.ContentLength != 0) || (file.FileName.Length != 0))
                    {
                        this.fileExtension = Path.GetExtension(file.FileName).ToLower();

                        if (this.CheckFileExtension(this.fileExtension))
                        {

                            if (this.CheckContentLength((long)file.ContentLength, this.maxSize))
                            {
                                if (this.CreateFolder())
                                {
                                    try
                                    {
                                        string path = Path.Combine(this.savePath, this.FileName + this.fileExtension);

                                        if (this.fileName == null)
                                        {
                                            while (File.Exists(path))
                                            {
                                                path = Path.Combine(this.savePath, this.FileName + this.fileExtension);

                                            }
                                        }
                                        string filename = HttpContext.Current.Server.MapPath(path);
                                        file.SaveAs(filename);
                                        HttpFileCollection files = HttpContext.Current.Request.Files;
                                        FilesInfo item = new FilesInfo
                                        {
                                            fileContentType = file.ContentType,
                                            fileName = Path.GetFileName(filename),
                                            filePath = path.Replace(HttpContext.Current.Server.MapPath("~/"), "").Replace(@"\", "/"),
                                            fileSize = file.ContentLength,
                                            fileExt = this.fileExtension,
                                            oldname = file.FileName.Replace(this.fileExtension, "")
                                        };
                                        this.fileCount++;
                                        this.listFile.Add(item);
                                    }
                                    catch (Exception)
                                    {
                                        this.errorCode = 6;
                                        this.fileCount = 0;
                                    }
                                }
                                else
                                {
                                    this.errorCode = 6;
                                }
                            }
                            else
                            {
                                this.errorCode = 4;
                            }
                        }
                        else
                        {
                            this.errorCode = 5;
                        }
                    }
                }
            }
            else
            {
                this.errorCode = 3;
            }
        }

        public void SaveUploadFiles()
        {
            string str = "neat";
            try
            {
                str = ConfigurationManager.AppSettings["uptype"].Trim();
            }
            catch
            {
                str = "neat";
            }

            this.SaveDefault();

        }

        public int FileCount
        {
            get
            {
                return this.fileCount;
            }
        }

        public string[] FileExtensions
        {
            get
            {
                return this.fileExtensions;
            }
            set
            {
                this.fileExtensions = value;
            }
        }

        public string FileName
        {
            get
            {
                return this.GetFileName();
            }
            set
            {
                this.fileName = value;
            }
        }

        private string FilePathAndName
        {
            get
            {
                if (this.fileName != null)
                {
                    return this.fileName;
                }
                return (this.GetFolder() + @"\" + this.FileName);
            }
        }

        public FolderTypes FolderType
        {
            get
            {
                return this.folderType;
            }
            set
            {
                this.folderType = value;
            }
        }

        public long MaxSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                this.maxSize = value;
            }
        }

        public string SavePath
        {
            get
            {
                return this.savePath;
            }
            set
            {
                this.savePath = value;
            }
        }

        public UploadTypes UploadType
        {
            get
            {
                return this.uploadType;
            }
            set
            {
                this.uploadType = value;
            }
        }
    }
}

