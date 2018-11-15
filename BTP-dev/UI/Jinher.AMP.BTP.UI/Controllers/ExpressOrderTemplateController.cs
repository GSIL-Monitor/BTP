using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using System.Text;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Common;
using System.IO;
using Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.FileServer.ISV.Facade;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 模板管理相关处理
    /// </summary>
    public class ExpressOrderTemplateController : BaseController
    {

        public ActionResult Index()
        {
            return Redirect("/Content/ExpressTemplate/index.html?appId=" + this.Request.QueryString["appId"]);
        }

        [HttpPost]
        public ActionResult GetAll()
        {
            string reqdata = Request["AppId"];
            Guid appId = Guid.Parse(reqdata);
            try
            {
                ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
                var data = templateBP.GetExpressOrderTemplateByAppId(appId);
                return Json(new { isSuccess = true, Data = data, Messages = "获取数据成功" });
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取快递单模板数据异常。appId：{0}", appId), ex);
            }
            return Json(new { isSuccess = false, Messages = "获取快递单模板数据异常" });
        }


        [HttpPost]
        public ActionResult Save(ExpressOrderTemplateDTO dto)
        {
            if (this.Request.Files.Count > 0)
            {
                var file = this.Request.Files[0];

                int ImgSize = file.ContentLength;

                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName).ToLower();

                var fs = file.InputStream;
                fs.Position = 0;
                BinaryReader r = new BinaryReader(fs);

                byte[] postArray = r.ReadBytes((int)fs.Length);

                FileDTO fileDTO = new FileDTO();
                fileDTO.UploadFileName = fileName;
                fileDTO.FileData = postArray;
                fileDTO.FileSize = ImgSize;
                FileFacade fileFacade = new FileFacade();
                string produceFilePath = fileFacade.UploadFile(fileDTO);

                string fullPath = CustomConfig.FileServerUrl + produceFilePath;

                dto.ExpressImage = fullPath;
            }

            ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
            var data = templateBP.Save(dto);

            return Json(data);
        }

        [HttpPost]
        public ActionResult Remove(ExpressOrderTemplateDTO dto)
        {
            ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
            var data = templateBP.Remove(dto);
            return Json(data);
        }

        [HttpPost]
        public ActionResult GetUsed()
        {
            string reqdata = Request["AppId"];
            Guid appId = Guid.Parse(reqdata);
            ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
            var data = templateBP.GetUsed(appId);
            return Json(data);
        }

        [HttpPost]
        public ActionResult SaveUsed(CustomTemplateCollection dto)
        {
            ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
            var data = templateBP.SaveUsed(dto.AppId, dto.Ids);
            return Json(data);
        }

        [HttpPost]
        public ActionResult SaveProperty(CustomExpressProperty dto)
        {
            ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
            var data = templateBP.SaveProperty(dto.TemplateId, dto.Property);
            return Json(data);
        }

        public class CustomExpressProperty
        {
            public Guid TemplateId { get; set; }
            public List<ExpressOrderTemplatePropertyDTO> Property { get; set; }
        }

        public class CustomTemplateCollection
        {
            public Guid AppId { get; set; }
            public List<Guid> Ids { get; set; }
        }
    }
}

