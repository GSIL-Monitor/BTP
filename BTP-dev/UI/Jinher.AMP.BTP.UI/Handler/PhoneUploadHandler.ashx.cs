using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
//using Jinher.AMP.News.Common;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.UI.Controllers;
using System.Drawing.Imaging;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
//using Jinher.AMP.News.Deploy;
//using Jinher.AMP.News.Common.ImagesUtility;

namespace Jinher.AMP.MoblieApp.UI
{
    /// <summary>
    /// PhoneUploadHandler 的摘要说明
    /// </summary>
    public class PhoneUploadHandler : IHttpHandler
    {
        private static string basepath = HttpContext.Current.Server.MapPath("~");
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                Image uploadImage = null;
                //string fileName = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName).ToLower();
                string fileName = Guid.NewGuid().ToString() + ext;
                string fileUrl = string.Empty;
                PathModel path = UploadPathHelper.GetPath(PathType.Temp);
                string uploadFilePath = path.TotalPath + "\\" + fileName;
                uploadImage = Image.FromStream(file.InputStream);
                try
                {
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                    {
                        if (string.Equals(ext, ".png"))
                        {
                            uploadImage.Save(uploadFilePath, ImageFormat.Png);
                        }
                        else
                        {
                            uploadImage.Save(uploadFilePath, ImageFormat.Jpeg);
                        }
                        uploadImage = ImageHelper.GetThumbNailImage(new Bitmap(uploadImage), 500, 1000);
                        string w = uploadImage.Width.ToString();
                        string h = uploadImage.Height.ToString();

                        ImageHelper.ImageCompress(uploadImage, 80,path, fileName);
                        
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
                        LogHelper.Error(string.Format("删除图片失败。path：{0}", path.TotalPath + "\\" + fileName), e);
                    }
                    //context.Session["ImgUrl"] = CustomConfig.FileServerUrl + produceFilePath;
                    //context.Session["ImgName"] = fileName;
                    string url=CustomConfig.FileServerUrl + produceFilePath;
                    JavaScriptSerializer seri = new JavaScriptSerializer();
                    context.Response.Clear();
                    // context.Response.Write(seri.Serialize(new { Url = CustomConfig.FileServerUrl + produceFilePath, Name = fileName }));
                    var temp = seri.Serialize(new { Url = CustomConfig.FileServerUrl + produceFilePath, Name = fileName });
                    context.Response.Write("<script>try{document.domain=\"iuoooo.com\";}catch(err){}</script>" + temp);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("PhoneUploadHandler出错", ex);
                    context.Response.Write(string.Empty);
                }
                finally
                {
                    if (uploadImage != null)
                    {
                        uploadImage.Dispose();
                    }
                }
            }
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
            //fileName = fileName.Substring(1);
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

        public static void SavePhonePic(Stream sr, string thumbnailPath)
        {
            Image originalImage = Image.FromStream(sr);
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            int width = ow;
            int height = oh;
            //新建一个bmp图片 
            Image bitmap = new Bitmap(width, height);

            //新建一个画板 
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, width, height),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);

            try
            {
                if (Path.GetExtension(thumbnailPath) == ".png")
                {
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    //以jpg格式保存缩略图 
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
                if (originalImage != null)
                {
                    originalImage.Dispose();
                }
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}