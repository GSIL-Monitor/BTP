using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.BaseApp.FileServer.ISV.Facade;
using Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO;
using System.IO;
using System.Text;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.EBC.UI.Controllers
{
    public class ImageUploadController : BaseController
    {
        private static string basedic = System.AppDomain.CurrentDomain.BaseDirectory;
        public ActionResult MakePic()
        {
            float zoom = float.Parse(Request.Form["zoom"]);
            float x1 = float.Parse(Request.Form["x1"]);
            float x2 = float.Parse(Request.Form["x2"]);
            float y1 = float.Parse(Request.Form["y1"]);
            float y2 = float.Parse(Request.Form["y2"]);
            string source = Request.Form["source"];
            
            try
            {
                string thumbName = Guid.NewGuid().ToString();
                ImagePaint.MakeThumbnail(basedic + "Photos\\" + source, basedic + "Photos\\" + thumbName + ".jpg", zoom);

                string makedname = Guid.NewGuid().ToString();
                ImagePaint.DrawImage(basedic + "Photos\\" + thumbName + ".jpg", basedic + "Photos\\" + makedname + ".jpg", (int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1));

                System.IO.File.Delete(basedic + "Photos\\" + thumbName + ".jpg");
               // System.IO.File.Delete(basedic + "Photos\\" + source);
                
                return Json(new { picname = makedname + ".jpg" });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="photoPath"></param>
        /// <returns></returns>
        public string UploadADPic(string photoPath, bool isLogo)
        {
            if (string.IsNullOrEmpty(photoPath) || photoPath.Length <= 10)
            {
                return "";
            }
            string realpath = Server.MapPath("\\Photos\\") + photoPath;
            string oldPhoto = realpath;
            if (!System.IO.File.Exists(realpath))
            {
                return photoPath;
            }

            string imageUrl = String.Empty;

            if (String.IsNullOrEmpty(photoPath))
            {
                return "";
            }
            if (isLogo)
            {
                realpath = CropImage(realpath.Substring(0, realpath.LastIndexOf(@"\")), realpath.Substring(realpath.LastIndexOf(@"\") + 1), 87, 87, 87, 87);
            }

            using (FileStream stream = new FileStream(realpath, FileMode.Open))
            {
                int fileLength = Convert.ToInt32(stream.Length);
               Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO file = new JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
                file.UploadFileName = Path.GetFileName(realpath);
                byte[] fileData = new byte[fileLength];
                stream.Read(fileData, 0, fileLength);
                file.FileData = fileData;
                file.FileSize = fileData.Length;
                file.StartPosition = 0;
                file.IsClient = false;
                //上传文件获得url
                imageUrl = Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFile(file);

                imageUrl = CustomConfig.FileServerUrl + imageUrl;
            }
            if (System.IO.File.Exists(realpath))
            {
                System.IO.File.Delete(realpath);
            }
            if (System.IO.File.Exists(oldPhoto))
            {
                System.IO.File.Delete(oldPhoto);
            }
            return imageUrl;
        }

        /// <summary>
        /// 复制图片
        /// </summary>
        /// <param name="imagePath">图片文件路径</param>
        /// <param name="imageName">图片名称</param>
        /// <param name="Width">长度</param>
        /// <param name="Height">高度</param>
        /// <param name="requiredWidth">缩放长度</param>
        /// <param name="requiredHeight">缩放高度</param>
        /// <returns></returns>
        public string CropImage(string imagePath, string imageName, int Width, int Height, int requiredWidth, int requiredHeight)
        {
            Image OriginalImage = System.Drawing.Image.FromFile(Path.Combine(imagePath, imageName));
            Bitmap bmp = Zoom(imagePath, imageName, requiredWidth, requiredHeight);
            string newImageName = Guid.NewGuid().ToString() + imageName.Substring(imageName.LastIndexOf("."));
            try
            {
                using (bmp)
                {
                    System.Drawing.Bitmap cropImage = new System.Drawing.Bitmap(Width, Height);
                    cropImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                    System.Drawing.Graphics Graphic = System.Drawing.Graphics.FromImage(cropImage);

                    Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                    Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    Width = bmp.Width;
                    Height = bmp.Height;
                    Graphic.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, Width, Height), 0, 0, Width, Height, System.Drawing.GraphicsUnit.Pixel);
                    Graphic.Dispose();

                    cropImage.Save(Path.Combine(imagePath, newImageName), OriginalImage.RawFormat);
                    cropImage.Dispose();
                }
                bmp.Dispose();
                OriginalImage.Dispose();
                return Path.Combine(imagePath, newImageName);
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }

        /// <summary>
        /// 图片缩放函数
        /// </summary>
        private static Bitmap Zoom(string imagePath, string imageName, int requiredWidth, int requiredHeight)
        {
            try
            {
                using (System.Drawing.Image OriginalImage = System.Drawing.Image.FromFile(Path.Combine(imagePath, imageName)))
                {
                    //int max = Math.Max(OriginalImage.Width, OriginalImage.Height);
                    double prec = (double)requiredWidth / OriginalImage.Width;
                    int width = (int)(OriginalImage.Width * prec);
                    int height = (int)(OriginalImage.Height * prec);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                    System.Drawing.Graphics Graphic = System.Drawing.Graphics.FromImage(bmp);

                    Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                    Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    Graphic.DrawImage(OriginalImage, new System.Drawing.Rectangle(0, 0, width, height));
                    Graphic.Dispose();

                    OriginalImage.Dispose();
                    return bmp;
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
    }
}
