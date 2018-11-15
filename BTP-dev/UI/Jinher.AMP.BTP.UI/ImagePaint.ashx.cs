using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;

namespace Jinher.AMP.BTP.UI
{
    /// <summary>
    /// ImagePaint 的摘要说明
    /// </summary>
    public class ImagePaint : IHttpHandler
    {
        private static string basepath = HttpContext.Current.Server.MapPath("~");
        private static int backwidth = 480;
        private static int backheight = 260;

        public void ProcessRequest(HttpContext context)
        {
            HttpPostedFile file = context.Request.Files[0];
            int FileLen = file.ContentLength;
            Byte[] imgData = new Byte[FileLen];
            Stream sr = file.InputStream;
            sr.Read(imgData, 0, FileLen);
            sr.Position = 0;
            string filename = Guid.NewGuid().ToString() + ".jpg";
            file.SaveAs(basepath + "Photos\\" + filename);
            sr.Position = 0;
            Image pic = Image.FromStream(sr);

            context.Response.Write("Photos\\" + filename + "^" + pic.Width + "^" + pic.Height);

            //上传图片小
            //if (pic.Width < backwidth && pic.Height < backheight)
            //{
            //    context.Response.Write("Photos\\" + filename + "^" + pic.Width + "^" + pic.Height + "^" + pic.Width + "^" + pic.Height);
            //}
            //else
            //{
            //    //图片相对较宽
            //    if ((pic.Width / pic.Height) > (480 / 260))
            //    {
            //        MakeThumbnail(basepath + "Photos\\" + filename, basepath + "SmallTempImg\\" + filename, 480, 0, "W");
            //        context.Response.Write("SmallTempImg\\" + filename + "^" + pic.Width + "^" + pic.Height + "^" + 480 + "^" + (int)(pic.Height * 480 / pic.Width));
            //    }
            //    else
            //    {
            //        MakeThumbnail(basepath + "Photos\\" + filename, basepath + "SmallTempImg\\" + filename, 0, 260, "H");
            //        context.Response.Write("SmallTempImg\\" + filename + "^" + pic.Width + "^" + pic.Height + "^" + (int)(pic.Width * 260 / pic.Height) + "^" + 260);
            //    }
            //}
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 将源图中固定区域保存为新的图片
        /// </summary>
        /// <param name="srcImage">源图地址</param>
        /// <param name="destImage">目标图片地址</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public static void DrawImage(string srcImage, string destImage, int x, int y, int width, int height)
        {
            using (System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(srcImage))
            {
                using (System.Drawing.Image templateImage = new System.Drawing.Bitmap(width, height))
                {
                    using (System.Drawing.Graphics templateGraphics = System.Drawing.Graphics.FromImage(templateImage))
                    {
                        templateGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        templateGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        templateGraphics.DrawImage(sourceImage, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(x, y, width, height), System.Drawing.GraphicsUnit.Pixel);
                        templateImage.Save(destImage, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }


        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, float zoom)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int ow = originalImage.Width;
            int oh = originalImage.Height;

            int towidth = Convert.ToInt32(ow * zoom);
            int toheight = Convert.ToInt32(oh * zoom);

            //新建一个bmp图片 
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(0, 0, ow, oh),
             GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}