using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CommodityAttributeController : Controller
    {


        #region 尺寸列表
        public ActionResult SizeList(string attributeId)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
           
            SecondAttributeFacade sf = new SecondAttributeFacade();
            List<ColorAndSizeAttributeVM> sizeList=sf.GetAttributeBySellerID(appId,new Guid(attributeId));
            ViewBag.SizeList=sizeList;
            return View();
        }
        #endregion

        #region 颜色列表
        public ActionResult ColorList(string attributeId)
        {
            string strAppId =Request.QueryString["appId"];
            Guid appId;
           
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
           
            SecondAttributeFacade sf = new SecondAttributeFacade();
            List<ColorAndSizeAttributeVM> sizeList = sf.GetAttributeBySellerID(appId, new Guid(attributeId));
            ViewBag.SizeList = sizeList;
            return View();
        }
        #endregion

        #region 添加尺寸
        public ActionResult AddSize(string attributeId,string name)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            
            SecondAttributeFacade sf = new SecondAttributeFacade();
            ResultDTO result = sf.AddSecondAttribute(new Guid(attributeId), name, appId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "添加成功" });
            }
            return Json(new { Result = false, Messages = "添加失败" });
        }
        #endregion


        #region 添加颜色
        public ActionResult AddColor(string attributeId, string name)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
           
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
           
            SecondAttributeFacade sf = new SecondAttributeFacade();
            ResultDTO result = sf.AddSecondAttribute(new Guid(attributeId), name, appId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "添加成功" });
            }
            return Json(new { Result = false, Messages = "添加失败" });
        }
        #endregion
    }
}
