using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.FileServer.ISV.Facade;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class AppImageController : BaseController
    {
        /// <summary>
        /// 图片压缩比
        /// </summary>
        int Compression = 80;

        /// <summary>
        /// 
        /// </summary>
        const string CookieName = "AppCutImageSet";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int cutImgWidth = 0, int cutImgHeight = 0, int upload = 1)
        {
            if (cutImgWidth == 0 || cutImgHeight == 0)
            {
                HttpCookie cookie = Request.Cookies[CookieName];
                int.TryParse(cookie["width"], out cutImgWidth);
                int.TryParse(cookie["height"], out cutImgHeight);
            }
            else
            {
                HttpCookie cookie = new HttpCookie(CookieName);
                cookie["width"] = cutImgWidth.ToString();
                cookie["height"] = cutImgHeight.ToString();

                this.Response.Cookies.Add(cookie);
            }
            //InitPageInfo(480, 300, 360, 200, 1);
            ViewBag.CutImgWidth = cutImgWidth;
            ViewBag.CutImgHeight = cutImgHeight;
            ViewBag.Upload = upload;


            return View();
        }
        public ActionResult DirectUploadWHWx(int iHeight = 108, int iWidth = 108)
        {
            try
            {
                if (Request.Files == null || Request.Files.Count <= 0)
                {
                    return Content("false");
                }
                var file1 = Request.Files[0];
                Image img = Image.FromStream(file1.InputStream);
                if (img.Height != 316 || img.Width != 316)
                {
                    return Content("whsize");
                }

                return Content(SaveFileWx(file1));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Info(string.Format("Error_LogKey:Jinher.AMP.ZPH.UI.Controllers.Upload：上传图片异常。Message：{0}，StackTrack：{1}", ex.Message, ex.StackTrace));
                return Content("false");
            }
        }
        protected string SaveFileWx(HttpPostedFileBase file1)
        {
            int ImgSize = file1.ContentLength;
            string defImgSize = CustomConfig.DefaultImgSize;
            int defaultImgSize = string.IsNullOrEmpty(defImgSize) ? 51200 : Convert.ToInt32(defImgSize);

            if (ImgSize > defaultImgSize)
            {
                return "size";
            }

            //生成将要保存的随机文件名
            string fileName = Guid.NewGuid() + Path.GetExtension(file1.FileName).ToLower();

            //要上传的文件 
            var fs = file1.InputStream;
            fs.Position = 0;
            BinaryReader r = new BinaryReader(fs);
            //使用UploadFile方法可以用下面的格式 
            byte[] postArray = r.ReadBytes((int)fs.Length);

            FileDTO fileDTO = new FileDTO();
            fileDTO.UploadFileName = fileName;
            fileDTO.FileData = postArray;
            fileDTO.FileSize = ImgSize;
            FileFacade fileFacade = new FileFacade();
            string produceFilePath = fileFacade.UploadFile(fileDTO);

            return UploadFileFullPath(produceFilePath);
        }
        public ActionResult DirectUploadWH(int iHeight = 108, int iWidth = 108)
        {
            try
            {
                if (Request.Files == null || Request.Files.Count <= 0)
                {
                    return Content("false");
                }
                var file1 = Request.Files[0];
                //Image img = Image.FromStream(file1.InputStream);
                //if (img.Height != iHeight || img.Width != iWidth)
                //{
                //    return Content("whsize");
                //}

                return Content(SaveFile(file1));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Info(string.Format("Error_LogKey:Jinher.AMP.ZPH.UI.Controllers.Upload：上传图片异常。Message：{0}，StackTrack：{1}", ex.Message, ex.StackTrace));
                return Content("false");
            }
        }        
        protected string SaveFile(HttpPostedFileBase file1)
        {
            int ImgSize = file1.ContentLength;
            //string defImgSize = CustomConfig.DefaultImgSize;
            //int defaultImgSize = string.IsNullOrEmpty(defImgSize) ? 51200 : Convert.ToInt32(defImgSize);

            //if (ImgSize > defaultImgSize)
            //{
            //    return "size";
            //}

            //生成将要保存的随机文件名
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file1.FileName).ToLower();

            //要上传的文件 
            var fs = file1.InputStream;
            fs.Position = 0;
            BinaryReader r = new BinaryReader(fs);
            //使用UploadFile方法可以用下面的格式 
            byte[] postArray = r.ReadBytes((int)fs.Length);

            FileDTO fileDTO = new FileDTO();
            fileDTO.UploadFileName = fileName;
            fileDTO.FileData = postArray;
            fileDTO.FileSize = ImgSize;
            FileFacade fileFacade = new FileFacade();
            string produceFilePath = fileFacade.UploadFile(fileDTO);

            return UploadFileFullPath(produceFilePath);
        }
        string UploadFileFullPath(string produceFilePath)
        {
            return CustomConfig.FileServerUrl + produceFilePath;
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns>返回值</returns>
        public ActionResult UpLoad(int minWidth, int minHeight)
        {
            int width = 480;        //截图区域宽度
            int height = 300;       //截图区域高度
            //int maxWidth = 1000;    //最大宽度
            //int maxHeight = 720;    //最大高度
            //int minWidth = 720;   ////最小宽度
            //int minHeight = 400;  ////最小高度 

            decimal amplify = 1;

            System.Drawing.Image uploadImage = null;   ////原图

            bool success = false;
            string message = string.Empty;
            ImageInfo image = new ImageInfo();

            try
            {
                HttpFileCollectionBase file = Request.Files;
                PathModel path = UploadPathHelper.GetPath(PathType.Temp);

                if (file != null && file.Count > 0)
                {
                    //50M
                    if (file[0].ContentLength <= 1024 * 1024 * 50)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName).ToLower();
                        string uploadFilePath = path.TotalPath + "\\" + fileName;   ////缩放图文件路径

                        uploadImage = Image.FromStream(file[0].InputStream);
                        //如果图片小于要截取的图片大小
                        uploadImage = EnlargeImg(uploadImage, minWidth, minHeight);

                        string imageFormat = ".jpg";
                        int index = fileName.LastIndexOf('.');
                        if (index > -1)
                        {
                            if (string.Equals(fileName.Substring(index), ".png", StringComparison.CurrentCultureIgnoreCase))
                            {
                                imageFormat = ".png";
                            }
                        }

                        if (string.Equals(imageFormat, ".png"))
                        {
                            uploadImage.Save(uploadFilePath, ImageFormat.Png);
                        }
                        else
                        {
                            uploadImage.Save(uploadFilePath, ImageFormat.Jpeg);
                        }

                        //临时文件路径
                        image.ImagePath = path.Path + "\\" + fileName;
                        //图片宽高符合要求
                        if (uploadImage.Width < width && uploadImage.Height < height)
                        {
                            image.Ratio = amplify;
                            image.Height = (int)(uploadImage.Height / amplify);
                            image.Width = (int)(uploadImage.Width / amplify);
                        }
                        else
                        {
                            //计算缩放比例
                            //图片的宽高比大于截图操作区域的宽高比（图片显得宽）
                            if (uploadImage.Width / uploadImage.Height > width / height)
                            {
                                image.Ratio = (decimal)uploadImage.Width / width;
                                image.Width = width;
                                image.Height = (int)(uploadImage.Height / image.Ratio);
                            }
                            else
                            {
                                image.Ratio = (decimal)uploadImage.Height / height;
                                image.Height = height;
                                image.Width = (int)(uploadImage.Width / image.Ratio);
                            }
                        }
                        success = true;
                    }
                    else
                    {
                        message = "图片必须小于等于50M.";
                    }
                }
                else
                {
                    message = "上传文件为空";
                }

                return Json(new { Success = success, Msg = message, Img = image }, "text/html");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (uploadImage != null)
                {
                    uploadImage.Dispose();
                }
            }
        }

        /// <summary>
        /// 切图
        /// </summary>
        /// <param name="imageWidth">前台图片截取区域宽度</param>
        /// <param name="imageHeight">图片图片截取区域高度</param>
        /// <param name="pointX">要截取的左上角坐标X点</param>
        /// <param name="pointY">要截取的左上角坐标Y点</param>
        /// <param name="imgUrl">上传的临时图片文件的路径</param>
        /// <returns>截取后的图片路径</returns>
        public string CutAvatar(decimal imageWidth, decimal imageHeight, int cutWidth, int cutHeight, string pointX, string pointY, string imgUrl, decimal amplify = 1, int upload = 1)
        {
            System.Drawing.Bitmap bitmap = null;   ////按截图区域生成Bitmap
            System.Drawing.Bitmap thumbImg = null; ////被截图 
            System.Drawing.Graphics gps = null;    ////存绘图对象   
            try
            {
                if (imageWidth <= 0 || imageHeight <= 0 || cutWidth <= 0 || cutHeight <= 0)
                {
                    return "Error";
                }
                //int cutWidth = 720;        ////截图矩形的大小
                //int cutHeight = 400;

                int selectWidth = (int)(imageWidth * amplify);
                int selectHeight = (int)(imageHeight * amplify);

                if (!string.IsNullOrEmpty(pointX) && !string.IsNullOrEmpty(pointY) && !string.IsNullOrEmpty(imgUrl))
                {
                    string ext = System.IO.Path.GetExtension(imgUrl).ToLower();
                    thumbImg = new Bitmap(Server.MapPath(imgUrl));
                    //切图图片对象
                    bitmap = new Bitmap(selectWidth, selectHeight);

                    Rectangle srcRect = new Rectangle((int)(Convert.ToInt32(pointX) * amplify), (int)(Convert.ToInt32(pointY) * amplify), selectWidth, selectHeight);   ////得到截图矩形
                    Rectangle destRect = new Rectangle(0, 0, selectWidth, selectHeight);

                    gps = Graphics.FromImage(bitmap);

                    //可以用次方法将png转为jpg后的黑色背景变为白色背景。
                    //Color color = Color.FromArgb(255, 250, 255, 249);  
                    //gps.FillRectangle(new SolidBrush(color), destRect);

                    gps.DrawImage(thumbImg, destRect, srcRect, GraphicsUnit.Pixel);
                    //切图
                    System.Drawing.Image finalImg = ImageHelper.GetThumbNailImage(bitmap, cutWidth, cutHeight);

                    PathModel path = UploadPathHelper.GetPath(PathType.NewsImg);
                    string fileCode = Guid.NewGuid().ToString();

                    string imageExt = "jpg";
                    int index = imgUrl.LastIndexOf('.');
                    if (index > -1)
                    {
                        if (string.Equals(imgUrl.Substring(index), ".png", StringComparison.CurrentCultureIgnoreCase))
                        {
                            imageExt = "png";
                        }
                        else
                        {
                            imageExt = "jpg";
                        }
                    }

                    string fileName = string.Format("/{0}." + imageExt, fileCode);

                    //保存在本地，为了生成二维码所用
                    if (upload == 0)
                    {
                        string finalPath = path.TotalPath + "\\" + fileName;
                        if (selectHeight < cutHeight || selectWidth < cutWidth)
                        {
                            Bitmap dec = new Bitmap(finalImg, cutWidth, cutHeight);
                            dec.Save(finalPath);
                            dec.Dispose();
                        }
                        else
                        {
                            finalImg.Save(finalPath);
                            finalImg.Dispose();
                        }
                        return finalPath;
                    }

                    //压缩
                    if (selectHeight < cutHeight || selectWidth < cutWidth)
                    {
                        Bitmap dec = new Bitmap(finalImg, cutWidth, cutHeight);
                        ImageHelper.ImageCompress(dec, Compression, path, fileName);
                    }
                    else
                    {
                        ImageHelper.ImageCompress(finalImg, Compression, path, fileName);
                    }

                    //上传至文件服务器
                    string produceFilePath = UploadFileServer(path, ref fileName);


                    //删除
                    try
                    {
                        LogHelper.Info("切图路径：" + path.TotalPath + "\\" + fileName);
                        System.IO.File.Delete(path.TotalPath + "\\" + fileName);

                    }
                    catch (Exception e)
                    {
                        LogHelper.Error(string.Format("删除图片失败，path：{0}", path.TotalPath + "\\" + fileName), e);
                    }
                    //为适应部署环境的多域名 chenghb 20140116
                    string h = Request.Url.Host.ToLower();

                    return CustomConfig.FileServerUrl + produceFilePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("切图失败。imageWidth：{0}，imageHeight：{1}，cutWidth：{2}，cutHeight：{3}，pointX：{4}，pointY：{5}，imgUrl：{6}，1：{7}", imageWidth, imageHeight, cutWidth, cutHeight, pointX, pointY, imgUrl, 1), ex);
                return "Error";
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                if (thumbImg != null)
                {
                    thumbImg.Dispose();
                }
                if (gps != null)
                {
                    gps.Dispose();
                }
                //删除
                try
                {
                    LogHelper.Info("原图路径：" + Server.MapPath(imgUrl));
                    System.IO.File.Delete(Server.MapPath(imgUrl));

                }
                catch (Exception e)
                {
                    LogHelper.Error("删除图片失败：" + e.Message);
                    LogHelper.Error(string.Format("删除图片失败，path：{0}", Server.MapPath(imgUrl)), e);
                }
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="picUrl"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public string GenerateMinPic(string picUrl, PathModel path, string fileName, int width, int height)
        {
            System.Drawing.Image thumbImg = new Bitmap(Server.MapPath(picUrl));
            thumbImg = ImageHelper.GetThumbNailImage(thumbImg, width, height);
            //压缩
            PathModel filePath = ImageHelper.ImageCompress(thumbImg, Compression, path, fileName);

            return filePath.Path;
        }

        /// <summary>
        /// 放大图片
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        /// <returns></returns>
        private static System.Drawing.Image EnlargeImg(System.Drawing.Image uploadImage, int minWidth, int minHeight)
        {
            if (uploadImage.Width < minWidth || uploadImage.Height < minHeight)
            {
                int needWidth;
                int needHeight;
                //宽比
                double xRatio = (double)uploadImage.Width / minWidth;
                //高比
                double yRatio = (double)uploadImage.Height / minHeight;
                //实际图片显得宽，以高为标准
                if (xRatio > yRatio)
                {
                    needHeight = minHeight;
                    needWidth = (int)(uploadImage.Width * ((double)minHeight / uploadImage.Height));
                }
                else
                {
                    needWidth = minWidth;
                    needHeight = (int)(uploadImage.Height * ((double)minWidth / uploadImage.Width));
                }
                uploadImage = ImageHelper.EnlargeImg(uploadImage, needWidth, needHeight);
            }
            return uploadImage;
        }

        /// <summary>
        /// 上传到文件服务器
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string UploadFileServer(PathModel path, ref string fileName)
        {
            Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO fileDTO = new JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
            fileName = fileName.Substring(1);
            fileDTO.UploadFileName = fileName;

            using (FileStream stream = new FileStream(path.TotalPath + "\\" + fileName, FileMode.Open))
            {
                int fileLength = Convert.ToInt32(stream.Length);

                byte[] fileData = new byte[fileLength];
                stream.Read(fileData, 0, fileLength);
                fileDTO.FileData = fileData;
                fileDTO.FileSize = fileData.Length;
                fileDTO.StartPosition = 0;
                fileDTO.IsClient = false;
            }


            return Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFile(fileDTO);
        }
    }

    /// <summary>
    /// 创建上传文件路径的帮助类
    /// </summary>
    public class UploadPathHelper
    {
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="type">路径类型PathType</param>
        /// <returns></returns>
        public static PathModel GetPath(string type)
        {
            DateTime dt = DateTime.Now;
            string path = string.Format("/uploadimages/{0}/{1}/{2}/{3}", type, dt.Year, dt.Month, dt.Day);
            string root = HttpContext.Current.Server.MapPath("~" + path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            PathModel model = new PathModel() { Path = path, TotalPath = root };
            return model;
        }
    }

    /// <summary>
    /// 路径
    /// </summary>
    public class PathModel
    {
        /// <summary>
        /// web服务器上相对于根目录的路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 相对于文件系统的路径（保存路径）
        /// </summary>
        public string TotalPath { get; set; }
    }

    /// <summary>
    /// 上传文件类型
    /// </summary>
    public static class PathType
    {
        /// <summary>
        /// 商品图片
        /// </summary>
        public const string NewsImg = "CommodityImg";

        /// <summary>
        /// 商品缩略图片
        /// </summary>
        public const string PicMin = "CommodityPicMin";

        /// <summary>
        /// 内容图片
        /// </summary>
        public const string Concent = "Concent";

        /// <summary>
        /// 报纸logo
        /// </summary>
        public const string PaperLogo = "PaperLogo";

        /// <summary>
        /// 报纸logo
        /// </summary>
        public const string Temp = "Temp";

        /// <summary>
        /// 用户头像
        /// </summary>
        public const string UserPhoto = "UserPhoto";

        /// <summary>
        /// 二维码图片
        /// </summary>
        public const string QRCode = "QRCode";
    }

    /// <summary>
    /// 切图帮助类
    /// </summary>
    public partial class ImageHelper
    {
        #region 对给定的一个图片（Image对象）生成一个指定大小的缩略图

        /// <summary>
        /// 对给定的一个图片（Image对象）生成一个指定大小的缩略图。
        /// </summary>
        /// <param name="originalImage">原始图片</param>
        /// <param name="thumMaxWidth">缩略图的宽度</param>
        /// <param name="thumMaxHeight">缩略图的高度</param>
        /// <returns>返回缩略图的Image对象</returns>
        public static Image GetThumbNailImage(Image originalImage, int thumMaxWidth, int thumMaxHeight)
        {
            Size thumRealSize = System.Drawing.Size.Empty;
            Image newImage = originalImage;
            Graphics graphics = null;

            try
            {
                thumRealSize = GetNewSize(thumMaxWidth, thumMaxHeight, originalImage.Width, originalImage.Height);
                newImage = new Bitmap(thumRealSize.Width, thumRealSize.Height);
                graphics = Graphics.FromImage(newImage);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(
                    originalImage,
                    new Rectangle(0, 0, thumRealSize.Width, thumRealSize.Height),
                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                    GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (originalImage != null)
                {
                    originalImage.Dispose();
                }
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
            }

            return newImage;
        }

        #endregion

        #region 获取一个图片按等比例缩小后的大小

        /// <summary>
        /// 获取一个图片按等比例缩小后的大小。
        /// </summary>
        /// <param name="maxWidth">需要缩小到的宽度</param>
        /// <param name="maxHeight">需要缩小到的高度</param>
        /// <param name="originalWidth">图片的原始宽度</param>
        /// <param name="originalHeight">图片的原始高度</param>
        /// <returns>返回图片按等比例缩小后的实际大小</returns>
        public static Size GetNewSize(double maxWidth, double maxHeight, double originalWidth, double originalHeight)
        {
            double width = 0.0;
            double height = 0.0;
            if (originalWidth < maxWidth && originalHeight < maxHeight)
            {
                width = originalWidth;
                height = originalHeight;
            }
            else if ((originalWidth / originalHeight) > (maxWidth / maxHeight))
            {
                width = maxWidth;
                height = (width * originalHeight) / originalWidth;
            }
            else
            {
                height = maxHeight;
                width = (height * originalWidth) / originalHeight;
            }

            return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void FileDel(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        #endregion

        /// <summary>
        /// 放大图片，如果图片宽高达不到要求，则放大
        /// </summary>
        /// <param name="img">原始图片</param>
        /// <param name="minWidth">缩略图的宽度</param>
        /// <param name="minHeight">缩略图的高度</param>
        /// <returns>返回缩略图的Image对象</returns>
        public static Image EnlargeImg(Image img, int minWidth, int minHeight)
        {
            Size newSize = System.Drawing.Size.Empty;
            Image newImage;
            Graphics graphics = null;

            try
            {
                int oWidth = img.Width;
                int oHeight = img.Height;
                if (oWidth < minWidth)
                {
                    oWidth = minWidth;
                }
                if (oHeight < minHeight)
                {
                    oHeight = minHeight;
                }

                newSize = new Size(oWidth, oHeight);
                newImage = new Bitmap(newSize.Width, newSize.Height);
                graphics = Graphics.FromImage(newImage);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(
                    img,
                    new Rectangle(0, 0, newSize.Width, newSize.Height),
                    new Rectangle(0, 0, img.Width, img.Height),
                    GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
            }

            return newImage;
        }

        /// <summary>
        /// 缩小图片，如果图片宽高超出要求，则缩小
        /// </summary>
        /// <param name="img">原始图片</param>
        /// <param name="maxWidth">缩略图的宽度</param>
        /// <param name="maxHeight">缩略图的高度</param>
        /// <returns>返回缩略图的Image对象</returns>
        public static Image ShrinkImg(Image img, int maxWidth, int maxHeight)
        {
            Size newSize = System.Drawing.Size.Empty;
            Image newImage;
            Graphics graphics = null;

            try
            {
                int oWidth = img.Width;
                int oHeight = img.Height;
                if (oWidth > maxWidth)
                {
                    oWidth = maxWidth;
                }
                if (oHeight > maxHeight)
                {
                    oHeight = maxHeight;
                }

                newSize = new Size(oWidth, oHeight);
                newImage = new Bitmap(newSize.Width, newSize.Height);
                graphics = Graphics.FromImage(newImage);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(
                    img,
                    new Rectangle(0, 0, newSize.Width, newSize.Height),
                    new Rectangle(0, 0, img.Width, img.Height),
                    GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
            }

            return newImage;
        }

        /// <summary>
        /// 图片压缩
        /// </summary>
        /// <param name="sourceImg"></param>
        /// <param name="compression"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns>图片相对路径</returns>
        public static PathModel ImageCompress(System.Drawing.Image sourceImg, int compression, PathModel path, string fileName)
        {
            PathModel result = new PathModel();

            EncoderParameters ep = new EncoderParameters();
            try
            {
                //图片保存路径
                string finalPath = path.TotalPath + "\\" + fileName;

                //以下代码为保存图片时，设置压缩质量  
                //设置压缩的比例1-100
                long[] qy = new long[1] { compression };

                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;

                string ext = "JPEG";
                int index = fileName.LastIndexOf('.');
                if (index > -1)
                {
                    if (string.Equals(fileName.Substring(index), ".png", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ext = "PNG";
                    }
                    else
                    {
                        ext = "JPEG";
                    }
                }
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals(ext))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }

                if (jpegICIinfo != null)
                {
                    sourceImg.Save(finalPath, jpegICIinfo, ep);
                }
                else
                {
                    sourceImg.Save(finalPath, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sourceImg != null)
                {
                    sourceImg.Dispose();
                }
                if (ep != null)
                {
                    ep.Dispose();
                }
            }

            //图片相对路径
            result.Path = path.Path + fileName;
            result.TotalPath = path.TotalPath + "\\" + fileName;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceImg"></param>
        /// <param name="compression"></param>
        /// <param name="fileName"></param>
        /// <returns>压缩图片并上传到图片服务器</returns>
        public static string ImageCompress(System.Drawing.Image sourceImg, int compression, string fileName)
        {
            string fileUrl = string.Empty;
            MemoryStream stream = new MemoryStream();
            EncoderParameters ep = new EncoderParameters();
            try
            {
                //以下代码为保存图片时，设置压缩质量  
                //设置压缩的比例1-100
                long[] qy = new long[1] { compression };

                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    sourceImg.Save(stream, jpegICIinfo, ep);
                }
                else
                {
                    sourceImg.Save(stream, ImageFormat.Jpeg);
                }
                fileUrl = Jinher.AMP.BTP.UI.Helper.UploadFile(stream, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sourceImg != null)
                {
                    sourceImg.Dispose();
                }
                if (ep != null)
                {
                    ep.Dispose();
                }
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            return fileUrl;
        }
    }

}

