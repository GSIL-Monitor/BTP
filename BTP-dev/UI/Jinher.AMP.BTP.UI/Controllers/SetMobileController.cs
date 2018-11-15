using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using System.Text.RegularExpressions;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.FSP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class SetMobileController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 首页视图
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WeixinOAuthOpenId]
        public ActionResult Index(Guid? appId, Guid? promotionId)
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/').ToLower();

            if (!url.Contains("?"))
            {
                url += "?source=share";
            }
            if (!url.Contains("setmobile") && !url.Contains("source=") && !url.Contains("source=share"))
            {
                url += "&source=share";
            }

            //SrcType附值
            int SrcType = 0;
            bool isShare = false;
            if (!url.Contains("source=share"))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["SrcType"]))
                {
                    int.TryParse(Request.QueryString["SrcType"], out SrcType);
                    if (SrcType != 39 && SrcType != 40)
                    {
                        SrcType = 36;
                    }
                }
                else
                    SrcType = 36;
            }
            else
            {
                isShare = true;
                SrcType = 34;
            }

            string patProductType = @"producttype=[^&]*";
            Regex rProductType = new Regex(patProductType, RegexOptions.IgnoreCase);
            url = rProductType.Replace(url, "producttype=" + (isShare ? "webcjzy" : "appcjzy"));

            string pat = @"srctype=[^&]*";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            url = r.Replace(url, "srctype=" + SrcType);

            if (!url.Contains("source="))
            {
                url += "&source=internal";
            }

            if (!url.Contains("&type=") && !url.Contains("?type="))
            {
                url += "&type=tuwen";
            }
            if (!url.Contains("isshowsharebenefitbtn="))
            {
                url += "&isshowsharebenefitbtn=1";
            }

            string srcUrl = Request.QueryString["srcurl"];
            if (!string.IsNullOrWhiteSpace(srcUrl))
            {
                return Redirect(srcUrl);
            }
            else
            {
                string appidReg = @"appid=[^&]*";
                Regex rAppId = new Regex(appidReg, RegexOptions.IgnoreCase);
                url = rAppId.Replace(url, "");

                string param = url.Substring(url.IndexOf("?") + 1);
                var redirectUrl = string.Format(CustomConfig.H5HomePage, CustomConfig.ZPHAppId) + "&" + param;
                if (Request.Url.Scheme == "https")
                {
                    redirectUrl = redirectUrl.Replace("http://", "https://");
                }
                return Redirect(redirectUrl);
            }
        }

        /// <summary>
        /// 商品列表视图
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WeixinOAuthOpenId]
        public ActionResult CommodityList(Guid? appId, Guid? promotionId)
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/');
            if (url.Contains('?'))
            {
                url = url + "&isshowsharebenefitbtn=1=";
            }
            else
            {
                url = url + "?isshowsharebenefitbtn=1=";
            }
            string param = url.Substring(url.IndexOf("?"));
            return Redirect(CustomConfig.ZPHUrl + "Category/Index" + param);
        }

        /// <summary>
        /// 商品列表视图
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="actId"></param>
        /// <returns></returns>
        [WeixinOAuthOpenId]
        public ActionResult CommodityView(Guid? appId, Guid? actId)
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/');
            if (url.Contains('?'))
            {
                url = url + "&actId=" + actId;
            }
            else
            {
                url = url + "?actId=" + actId;
            }
            if (!url.Contains("isshowsharebenefitbtn="))
            {
                url += "&isshowsharebenefitbtn=1";
            }
            string param = url.Substring(url.IndexOf("?"));
            return Redirect(CustomConfig.ZPHUrl + "GeneralActivity/Index" + param);
        }

        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="setCategoryId"></param>
        /// <param name="fieldSort"></param>
        /// <param name="order"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetCommodity(Guid setCategoryId, int fieldSort, string order, int pageIndex, int pageSize)
        {
            //匿名账号
            if (this.ContextDTO.LoginUserID == Guid.Empty)
            {
                AuthorizeHelper.InitAuthorizeInfo();
            }

            var qryCommodityDTO = new QryCommodityDTO
            {
                FieldSort = fieldSort,
                Order = order,
                PageIndex = pageIndex,
                PageSize = pageSize,
                SetCategoryId = setCategoryId
            };

            var facade = new Jinher.AMP.BTP.ISV.Facade.AppSetFacade();
            var ret = facade.GetCommodityList(qryCommodityDTO);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取商品分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCategory()
        {
            //匿名账号
            if (this.ContextDTO.LoginUserID == Guid.Empty)
            {
                AuthorizeHelper.InitAuthorizeInfo();
            }
            var facade = new ISV.Facade.AppSetFacade();
            var result = facade.GetCategory(Guid.Empty);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetShoppongCartList(Guid userId)
        {
            //匿名账号
            if (this.ContextDTO.LoginUserID == Guid.Empty)
            {
                AuthorizeHelper.InitAuthorizeInfo();
            }
            var facade = new Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> Lists = facade.GetShoppongCartItems(userId, Guid.Empty);

            if (Lists != null && Lists.Count > 0)
            {
                foreach (var item in Lists)
                {
                    Dictionary<Guid, string> list = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(new List<Guid> { item.AppId });
                    if (list.Any() && list.ContainsKey(item.AppId))
                    {
                        item.AppName = list[item.AppId];
                    }
                }
            }

            return Json(Lists, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据搜索条件查询商品
        /// </summary>
        /// <param name="want"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetWantCommodity(string want, int pageIndex, int pageSize)
        {
            //匿名账号
            if (this.ContextDTO.LoginUserID == Guid.Empty)
            {
                AuthorizeHelper.InitAuthorizeInfo();
            }
            Jinher.AMP.BTP.ISV.Facade.AppSetFacade facade = new ISV.Facade.AppSetFacade();
            var result = facade.GetWantCommodity(want, pageIndex, pageSize);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MoreConfirmPayPrice(Guid MainOrderId, decimal Price)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            decimal realPrice = 0;
            List<MainOrdersDTO> list = facade.GetMianOrderList(MainOrderId);
            foreach (MainOrdersDTO m in list)
            {
                NewResultDTO newResult = facade.ConfirmPayPrice(m.OrderId, m.UserId);
                if (newResult.ResultCode == 1)
                {
                    facade.DeleteMainOrder(m.OrderId);
                    //return Json(new { Message = newResult.Message, ResultCode = 1 }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    realPrice += Convert.ToDecimal(newResult.Message);
                }
            }
            if (realPrice == 0)
            {
                LogHelper.Error("MoreConfirmPayPrice服务异常，调用ConfirmPayPrice返回实际支付金额为0，MainOrderId=" + MainOrderId);
                return Json(new { Message = "支付失败！", ResultCode = 1 }, JsonRequestBehavior.AllowGet);
            }

            if (Price == realPrice)
            {
                return Json(new { Message = realPrice, ResultCode = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = realPrice, ResultCode = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MorePayPrice(Guid MainOrderId, Guid UserId, Guid esAppId)
        {
            if (this.ContextDTO.LoginUserID == Guid.Empty)
            {
                AuthorizeHelper.InitAuthorizeInfo();
            }
            //是否直接到账
            int tradeType = Jinher.AMP.BTP.TPS.FSPSV.GetTradeSettingInfo(esAppId);
            if (tradeType == -1)
            {
                return Json(new { Message = "订单支付失败", Monery = 0, ResultCode = 1, AppId = 0 }, JsonRequestBehavior.AllowGet);
            }

            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facadeorder = new ISV.Facade.CommodityOrderFacade();
            //根据主订单获取 子订单信息
            List<MainOrdersDTO> list = facadeorder.GetMianOrderList(MainOrderId);
            if (!list.Any())
            {
                return Json(new { Message = "订单支付失败", Monery = 0, ResultCode = 1, AppId = 0 }, JsonRequestBehavior.AllowGet);
            }



            List<Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO> listSecuredTrans = new List<FSP.Deploy.CustomDTO.SecuredTransDTO>();
            foreach (MainOrdersDTO m in list)
            {
                //沈克涛说这块就按订单所在应用的应用主传，不用考虑直接到账的情况
                Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO modelSecuredTrans = new FSP.Deploy.CustomDTO.SecuredTransDTO();
                var result = APPSV.Instance.GetAppOwnerInfo(m.AppId);
                if (result != null)
                {
                    modelSecuredTrans.PayeeId = result.OwnerId;
                }
                modelSecuredTrans.BizId = m.OrderId;
                modelSecuredTrans.Money = Convert.ToDouble(m.RealPrice);
                modelSecuredTrans.AppId = m.AppId;
                listSecuredTrans.Add(modelSecuredTrans);
            }

            Jinher.AMP.FSP.Deploy.CustomDTO.PrePayBatchDTO preDto = new FSP.Deploy.CustomDTO.PrePayBatchDTO();
            preDto.MainBizId = MainOrderId;
            preDto.PayorId = UserId;
            preDto.NotifyUrl = CustomConfig.BtpDomain + "PaymentNotify/Goldpay";
            preDto.OrderList = listSecuredTrans;

            ReturnInfoDTO<List<ChildTransactionStatusDTO>> resultGoldPay = null;
            //担保交易
            if (tradeType == 0)
            {
                resultGoldPay = Jinher.AMP.BTP.TPS.FSPSV.Instance.PrePayBatch(preDto);
            }
            //直接到账
            else if (tradeType == 1)
            {
                resultGoldPay = Jinher.AMP.BTP.TPS.FSPSV.Instance.PreDirectPayBatch(preDto);
            }
            if (resultGoldPay == null)
            {
                return Json(new { Message = "订单支付失败", Monery = 0, ResultCode = 1, AppId = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (resultGoldPay.Code == 0)
            {
                if (Convert.ToDecimal(resultGoldPay.ExtBag["TotalMoney"]) != 0)
                {
                    return Json(new { Message = "成功", Monery = Convert.ToDecimal(resultGoldPay.ExtBag["TotalMoney"]), ResultCode = 0, AppId = listSecuredTrans[0].BizId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "订单支付失败", Monery = 0, ResultCode = 1, AppId = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "订单支付失败", Monery = 0, ResultCode = 1, AppId = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 广告位
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBiddingAD()
        {
            var biddingAdCDTO = new Jinher.AMP.ADM.Deploy.CustomDTO.BiddingAdCDTO
            {
                ADNum = 15,
                AppId = Guid.Empty,
                IsAnon = false,
                MessageTimeTag = null,
                ProductType = (ADM.Deploy.Enum.ADProductEnum)26,
                UserId = Guid.Empty
            };
            try
            {
                var ads = Jinher.AMP.BTP.TPS.ADMSV.Instance.GetBiddingAD(biddingAdCDTO);
                return this.Json(ads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SetMobileController-GetBiddingAD。biddingAdCDTO：{0}", biddingAdCDTO), ex);
                return this.Json(new System.Web.UI.MobileControls.List(), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 正品会商品列表商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult CommodityListZPH(CommoditySearchZPHDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetCommodityByZPHActId(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 首页视图
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WeixinOAuthOpenId]
        public ActionResult ZPHCategory(Guid? appId, Guid? promotionId)
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/');
            if (url.Contains('?'))
            {
                url = url + "&isshowsharebenefitbtn=1=";
            }
            else
            {
                url = url + "?isshowsharebenefitbtn=1=";
            }
            string param = url.Substring(url.IndexOf("?"));
            return Redirect(CustomConfig.ZPHUrl + "zph/CategoryList" + param);
        }

    }
}

