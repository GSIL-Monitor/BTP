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

namespace Jinher.AMP.BTP.UI.Controllers
{
  /// <summary>
  /// 模板管理相关处理
  /// </summary>
    public class ExpressController : BaseController
    {
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAll()
        {
            ExpressFacade facade = new ExpressFacade();
            var data = facade.GetSystem();
            return Json(data);
        }

        /// <summary>
        /// 查询收藏
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCollectionAll()
        {
            Guid appId = Guid.Parse(this.Request.Params["appId"]);
            ExpressFacade facade = new ExpressFacade();
            var data = facade.GetAll(appId);
            return Json(data);
        }

        [HttpPost]
        public ActionResult GetUsed()
        {
            string reqdata = Request["AppId"];
            Guid appId = Guid.Parse(reqdata);
            ExpressFacade templateBP = new ExpressFacade();
            var data = templateBP.GetUsed(appId);
            return Json(data);
        }

        [HttpPost]
        public ActionResult SaveUsed(CustomExpressCollection dto)
        {
            ExpressFacade templateBP = new ExpressFacade();
            var data = templateBP.SaveUsed(dto.AppId, dto.Ids);
            return Json(data);
        }

        public class CustomExpressCollection
        {
            public Guid AppId { get; set; }
            public List<string> Ids { get; set; }
        }
    }
}

