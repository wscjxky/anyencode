namespace VodFile
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    public class showimg : Page
    {
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image image;
            string str;
            try
            {
                image = Image.FromFile(originalImagePath);
            }
            catch (Exception)
            {
                HttpContext.Current.Response.Write("参数错误");
                return;
            }
            int num = width;
            int num2 = height;
            int x = 0;
            int y = 0;
            int num5 = image.Width;
            int num6 = image.Height;
            if (((str = mode) != null) && (str != "HW"))
            {
                if (!(str == "W"))
                {
                    if (str == "H")
                    {
                        num = (image.Width * height) / image.Height;
                    }
                    else if (str == "Cut")
                    {
                        if ((((double) image.Width) / ((double) image.Height)) > (((double) num) / ((double) num2)))
                        {
                            num6 = image.Height;
                            num5 = (image.Height * num) / num2;
                            y = 0;
                            x = (image.Width - num5) / 2;
                        }
                        else
                        {
                            num5 = image.Width;
                            num6 = (image.Width * height) / num;
                            x = 0;
                            y = (image.Height - num6) / 2;
                        }
                    }
                }
                else
                {
                    num2 = (image.Height * width) / image.Width;
                }
            }
            if (num > image.Width)
            {
                num = image.Width;
                num2 = image.Height;
            }
            Image image2 = new Bitmap(num, num2);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            HttpContext.Current.Response.ContentType = "image/jpeg";
            MemoryStream stream = new MemoryStream();
            try
            {
                image2.Save(stream, ImageFormat.Jpeg);
                stream.WriteTo(HttpContext.Current.Response.OutputStream);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                image.Dispose();
                image2.Dispose();
                graphics.Dispose();
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            string path = string.Empty;
            int width = 0x37;
            int height = 0x2c;
            if (HttpContext.Current.Request.QueryString["file"] != null)
            {
                path = HttpContext.Current.Request.QueryString["file"].ToString();
            }
            else
            {
                HttpContext.Current.Response.Write("参数错误");
                return;
            }
            try
            {
                path = base.Server.MapPath(path);
            }
            catch (Exception)
            {
                HttpContext.Current.Response.Write("原图不存在！");
                return;
            }
            MakeThumbnail(path, "", width, height, "W");
        }
    }
}

