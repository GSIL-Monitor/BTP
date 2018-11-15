using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Jinher.JAP.Common.Loging;
namespace Jinher.AMP.BTP.UI.Commons
{
    public class QRCodeHelper : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="fileImg">临时图片名称，可以空</param>
        /// <param name="replaceUrl">要构建二维码的推广码的短地址</param>
        /// <param name="pictype">是否上传图片；2：上传自定义图片</param>
        /// <returns></returns>
        public static string GenerateImgTwoCode(string fileImg, string replaceUrl, int pictype)
        {
            string twodimcodeimg = "";
            try
            {


                //用户自已上传图片生成链接图片
                if (pictype == 2)
                {
                    //将图片的文件转换成二进制流
                    //string pServer = HttpContext.Server.MapPath("~/TempImage/" + fileImg);                    

                    //将网络图片保存在本地
                    //string saveFile = Guid.NewGuid().ToString() + Path.GetExtension(fileImg).ToLower();
                    //string pServer = GetPath() + "\\" + saveFile;
                    //getimages(fileImg, GetPath(), saveFile);

                    //本地图片读取
                    Byte[] imgfile = Jinher.AMP.EBC.Common.ImageHelper.ConvertFileToBinary(fileImg);
                    Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO qRDTO = new Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO();
                    qRDTO.IconDate = imgfile;
                    qRDTO.Source = replaceUrl;

                    //生成带图片的二维码
                    string codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCodeWithIcon(qRDTO);
                    twodimcodeimg = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;

                    //删除本地图片
                    System.IO.File.Delete(fileImg);
                }
                else
                {
                   //系统默认生成的不带图片的二维码                    
                    string codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCode(replaceUrl);
                    twodimcodeimg = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;
                }
            }
            catch (System.Exception ex)
            {
                string errStack = ex.Message + ex.StackTrace;
                while (ex.InnerException != null)
                {
                    errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                    ex = ex.InnerException;
                }
                LogHelper.Error(string.Format("生成二维码异常。fileImg：{0}，replaceUrl：{1}，pictype：{2}", fileImg, replaceUrl, pictype), ex);
            }
            return twodimcodeimg;
        }

        public static string GetPath()
        {
            string type = "QRCode";
            DateTime dt = DateTime.Now;
            string path = string.Format("/uploadimages/{0}/{1}/{2}/{3}", type, dt.Year, dt.Month, dt.Day);
            string root = System.Web.HttpContext.Current.Server.MapPath("~" + path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            return root;
        }

        public static string GetUrltoHtml(string Url, string type)
        {
            try
            {
                string result = string.Empty;
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding(type)))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }
        public static void getimages(string url, string path, string fileName)
        {
            System.Net.WebRequest imgRequst = System.Net.WebRequest.Create(url);
            System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequst.GetResponse().GetResponseStream());
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            downImage.Save(path + "\\" + fileName);
            downImage.Dispose();
            //用完一定要释放
        }
    }
}