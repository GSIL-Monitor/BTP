using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;


namespace Jinher.AMP.BTP.UI.Controllers
{
    public class ReviewController : Controller
    {
        #region 查询所有评价
        public ActionResult Index()
        {
            //Guid appId = new Guid("3BA8661D-CFD2-4046-A9CF-349630E7B6B7");
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            //string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString(); ;
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            if (appId != null)
            {
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }

            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int rowCount = 0;
            ReviewFacade rf = new ReviewFacade();
            List<ReviewVM> reviewList = rf.GetReviewList(appId, pageSize, pageIndex, "", "", "", "", out rowCount);
            ViewBag.ReviewList = reviewList;
            ViewBag.Count = rowCount;
            return View();
        }


        [HttpPost]
        public PartialViewResult PartialIndex(string startTime, string endTime, string commodityName, string content)
        {

            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            int rowCount = 0;
            ReviewFacade rf = new ReviewFacade();
            List<ReviewVM> reviewList = rf.GetReviewList(appId, pageSize, pageIndex, startTime, endTime, commodityName, content, out rowCount);
            ViewBag.ReviewList = reviewList;
            ViewBag.Count = rowCount;
            return PartialView();
        }
        #endregion

        #region 某一商品评价
        public ActionResult CommodityReview(Guid commodityId)
        {
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 10;
            int rowCount = 0;
            ReviewFacade rf = new ReviewFacade();
            CommodityReplyVM reply = rf.GetReplyByCommodityId(commodityId, pageSize, pageIndex, out rowCount);
            ViewBag.ReviewList = reply;
            ViewBag.Count = rowCount;
            return View();
        }


        #endregion


        #region 评价回复
        public ActionResult RespondComment(string reviewID, string content)
        {
            content = HttpUtility.UrlDecode(content);
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            ReviewFacade rf = new ReviewFacade();
            ReviewDTO reviewSDTO = rf.GetReviewById(new Guid(reviewID));


            ResultDTO result = rf.RespondComment(reviewSDTO.UserId, new Guid(reviewID), appId, content);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "回复成功" });
            }
            return Json(new { Result = true, Messages = "回复失败" });
        }
        #endregion


        /// <summary>
        /// 接收评价成功后的通知
        /// </summary>
        /// <returns></returns>
        public ActionResult ReviewSuccessNotify(Guid businessId, Guid productId)
        {

            try
            {
                string s = "进入ReviewSuccessNotify,参数 => businessId:{0},productId:{1}";
                s = string.Format(s, businessId, productId);
                LogHelper.Debug(s);

                ReviewNofityDTO rnDto = new ReviewNofityDTO();
                rnDto.OrderItemId = businessId;
                rnDto.CommodityId = productId;

                Jinher.AMP.BTP.ISV.Facade.ReviewFacade rf = new Jinher.AMP.BTP.ISV.Facade.ReviewFacade();
                var result = rf.ReviewNofity(rnDto);

                string sr = "ReviewSuccessNotify结果：{0}";
                sr = string.Format(sr, JsonHelper.JsonSerializer<ResultDTO>(result));
                LogHelper.Debug(sr);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ReviewSuccessNotify异常，异常信息：", ex);
                return Json(new ResultDTO() { Message = "异常", ResultCode = -9 });
            }
        }
        /// <summary>
        /// 接收评价成功后的通知
        /// </summary>
        /// <returns></returns>
        public ActionResult ReviewSuccessNotifyComOnly(Guid productId)
        {
            try
            {
                string s = "进入ReviewSuccessNotifyComOnly,参数 => productId:{0}";
                s = string.Format(s, productId);
                LogHelper.Debug(s);

                ReviewNofityDTO rnDto = new ReviewNofityDTO();
                rnDto.CommodityId = productId;

                Jinher.AMP.BTP.ISV.Facade.ReviewFacade rf = new Jinher.AMP.BTP.ISV.Facade.ReviewFacade();
                var result = rf.ReviewNofityComOnly(rnDto);

                string sr = "ReviewSuccessNotifyComOnly结果：{0}";
                sr = string.Format(sr, JsonHelper.JsonSerializer<ResultDTO>(result));
                LogHelper.Debug(sr);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ReviewSuccessNotifyComOnly异常，异常信息：", ex);
                return Json(new ResultDTO() { Message = "异常", ResultCode = -9 });
            }
        }
    }
}
