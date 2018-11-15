using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Jinher.AMP.BTP.UI.Handler
{
    /// <summary>
    /// ImgBatchUploadHandler 的摘要说明
    /// </summary>
    public class ImgBatchUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            List<string> filesList = new List<string>();
            int tmp = 0;

            //返回的描述信息
            string state = "SUCCESS";
            ////分辩率
            //int width = 640;
            //int height = 480;
            ////文件大小限制,单位mb
            //int size = 50;
            //传送方式
            string type = "form";
            //文件允许格式
            string[] accept = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };

            //if (context.Request["width"] != null && int.TryParse(context.Request["width"], out tmp))
            //{
            //    width = tmp;
            //}
            //if (context.Request["height"] != null && int.TryParse(context.Request["height"], out tmp))
            //{
            //    height = tmp;
            //}
            //if (context.Request["size"] != null && int.TryParse(context.Request["size"], out tmp))
            //{
            //    size = tmp;
            //}
            if (context.Request["type"] != null)
            {
                type = context.Request["type"].ToString();
            }
            if (context.Request["accept"] != null)
            {
                accept = context.Request["accept"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            var length = context.Request.Files.Count;

            //验证
            for (int i = 0; i < length; i++)
            {
                HttpPostedFile item = context.Request.Files[i];

                //格式验证              
                string curType = getFileExt(item.FileName);
                if (!accept.Contains(curType))
                {
                    state = "不允许的文件类型";
                }
                ////大小验证
                //if (item.ContentLength >= (size * 1024 * 1024))
                //{
                //    state = "文件大小超出网站限制";
                //}
                //Image uploadImg = Image.FromStream(item.InputStream);
                //if (uploadImg.Width != width || uploadImg.Height != height)
                //{
                //    state = "文件分辨率不符合网站规定";
                //}
                item.InputStream.Position = 0;

            }

            if (state == "SUCCESS")
            {
                //上传到文件服务器
                for (int i = 0; i < length; i++)
                {
                    HttpPostedFile item = context.Request.Files[i];

                    string filename = reName(item.FileName);
                    string url = FileUploadHelper.UploadFile(item.InputStream, filename);
                    filesList.Add(url);
                }

                if (type == "ajax")
                {
                    ImgResult ajaxResult = new ImgResult {
                        State=0,
                        StateTest = "SUCCESS",
                        FilesList = string.Join(",", filesList)
                    };
                    HttpContext.Current.Response.Write(JsonHelper.JsonSerializer(ajaxResult));
                }
                else
                {
                    context.Response.Write("<script>try { document.domain = 'iuoooo.com'; } catch (e) { } parent.AddImg('" + string.Join(",", filesList) + "','" + state + "');</script>");
                }
            }
            else
            {
                if (type == "ajax")
                {
                    ImgResult ajaxResult = new ImgResult
                    {
                        State = 1,
                        StateTest = state,
                        FilesList = string.Join(",", filesList)
                    };
                    HttpContext.Current.Response.Write(JsonHelper.JsonSerializer(ajaxResult));
                }
                else
                {
                    context.Response.Write("<script>try { document.domain = 'iuoooo.com'; } catch (e) { } parent.AddImg('','" + state + "');</script>");
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
        /**
      * 获取文件扩展名
      * @return string
      */
        private string getFileExt(string fileName)
        {
            string[] temp = fileName.Split('.');
            return "." + temp[temp.Length - 1].ToLower();
        }
        /**
    * 重命名文件
    * @return string
    */
        private string reName(string fileName)
        {
            return System.Guid.NewGuid() + getFileExt(fileName);
        }
    }

    //拖拽图片上传时返回结果
    public class ImgResult
    {
        public int State { get; set; }
        public string StateTest { get; set; }
        public string FilesList { get; set; }
    }
}