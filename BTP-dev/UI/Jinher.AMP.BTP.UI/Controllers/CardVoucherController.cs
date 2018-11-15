using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CardVoucherController : Controller
    {
        //
        // GET: /CardVoucher/
        public ActionResult Index()
        {
            string UserId = Request["UserId"].ToString();
            string AppId = Request["AppId"].ToString();
            ViewBag.UserId = UserId;
            ViewBag.AppId = AppId;
            if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(AppId))
            {
                var result = YJBSV.GetMyAllScore(Guid.Parse(AppId), Guid.Parse(UserId));

                if (result != null)
                {
                    ViewBag.Score = result[0].Capital;
                    ViewBag.ReScore = result[1].Capital;
                }
                else
                {
                    ViewBag.Score = 0;
                    ViewBag.ReScore = 0;
                }
            }
            else
            {
                ViewBag.Score = 0;
                ViewBag.ReScore = 0;
            }
            return View();
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetCoupon(string UserId, string AppId, int PageSize, int page, string appName)
        {
            AMP.Coupon.ISV.Facade.CouponFacade cf = new Coupon.ISV.Facade.CouponFacade();
            var coupon = new ListCouponRequestDTO();
            coupon.UserId = Guid.Parse(UserId);
            coupon.ShopId = Guid.Parse(AppId);
            coupon.CouponState = Coupon.Deploy.Enum.CouponState.All;
            coupon.AppName = appName;
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            var CouponList = cf.ListCoupon(coupon);

            if (CouponList == null)
            {
                return View();
            }

            int rowCount = CouponList.TotalCount;
            IList<string> show = new List<string>();
            show.Add("CouponId");
            show.Add("AppName");
            show.Add("Name");
            show.Add("CouponType");
            show.Add("Cash");
            show.Add("CouponState");
            show.Add("BindTime");
            show.Add("ModifiedOn");
            show.Add("BeginTime");
            show.Add("OrderId");
            show.Add("EndTime");
            show.Add("orderItemList");

            var listCoupon = new List<CouponNewDTO>();

            foreach (var item in CouponList.RecordCollection.Skip(PageSize *(page - 1)).Take(PageSize))
            {
                var newCouponNew = new CouponNewDTO
                {
                    CouponId = item.Id,
                    Name = item.Name,
                    AppName = item.AppName,
                    CouponType = item.CouponType,
                    Cash = item.Cash,
                    CouponState = item.CouponStatus,
                    BindTime = item.BindTime,
                    ModifiedOn = item.ModifiedOn,
                    BeginTime = item.BeginTime,
                    OrderId = item.OrderId,
                    EndTime = item.EndTime,
                };

                //var list = new IBP.Facade.
                if (item.CouponStatus == Coupon.Deploy.Enum.CouponState.Used)
                {
                    var newOrderCode = new List<KeyValuePair<Guid, string>>();

                    var btCoupon = new IBP.Facade.CouponRefundFacade();
                    var listData = btCoupon.GetOrderInfoByCouponId(item.Id);

                    if (listData.isSuccess)
                    {
                        newOrderCode = listData.Data;
                        newCouponNew.orderItemList = JsonHelper.JsonSerializer(newOrderCode);
                    }
                }


                //newOrderCode.Add(new KeyValuePair<Guid, string>(Guid.Parse("BA96B833-E62A-43AE-BDAA-004ABE18285C"),"2018021504251"));                
                listCoupon.Add(newCouponNew);
            }

            var gridobj = new GridModel<Jinher.AMP.Coupon.Deploy.CustomDTO.CouponNewDTO>(show, listCoupon, rowCount, page, PageSize, string.Empty);
            return View(gridobj);
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetVoucher(string UserId, string AppId, int PageSize, int page)
        {
            AMP.YJB.ISV.Facade.UserYJCouponFacade cf = new YJB.ISV.Facade.UserYJCouponFacade();
            var coupon = new Jinher.AMP.YJB.Deploy.CustomDTO.UserYJCouponJouralSearchDTO();
            coupon.UserId = Guid.Parse(UserId);
            coupon.PageSize = PageSize;
            coupon.PageIndex = page;
            var CouponList = cf.GetUserYJCouponList(coupon);

            int rowCount = CouponList.Count;
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Name");
            show.Add("Price");
            show.Add("Status");
            show.Add("SubTime");
            show.Add("UseTime");
            show.Add("BeginTime");
            show.Add("OrderId");
            show.Add("EndTime");
            show.Add("OrderIdNumber");
            var gridobj = new GridModel<Jinher.AMP.YJB.Deploy.CustomDTO.UserYJCouponJouralDTO>(show, CouponList.Data, rowCount, page, PageSize, string.Empty);
            return View(gridobj);
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetCurrency(string UserId, string AppId, int PageSize, int page, int state)
        {
            if (state == 1)
            {
                state = 4;  //区别于前端
            }

            var data = YJBSV.GetUserYJBJournal(new Jinher.AMP.YJB.Deploy.CustomDTO.OrderYJBInfoInputDTO
            {
                UserId = Guid.Parse(UserId),
                Type = state,
                PageIndex = page,
                PageSize = 20,
            });

            IList<string> show = new List<string>();
            show.Add("ImgUrl");
            show.Add("Name");
            show.Add("Amount");
            show.Add("Type");
            show.Add("Count");
            show.Add("CreationTime");
            show.Add("ValidUntil");
            show.Add("OrderIdNumber");

            var bpfacade = new IBP.Facade.CommodityOrderFacade();

            foreach (var item in data.Data)
            {
                var orderId = Guid.Empty;
                if (!String.IsNullOrEmpty(item.OrderIdNumber) && Guid.TryParse(item.OrderIdNumber, out orderId))
                {
                    var orderInfo = bpfacade.GetCommodityOrderInfo(orderId);
                    if (orderInfo != null)
                    {
                        var keyInfo = new KeyValuePair<Guid, string>(orderId, orderInfo.Code);
                        item.OrderIdNumber = JsonHelper.JsonSerializer(keyInfo);
                    }
                    else
                    {
                        item.OrderIdNumber = "";
                    }
                }
                else
                {
                    item.OrderIdNumber = "";
                }
            }

            var gridobj = new GridModel<Jinher.AMP.YJB.Deploy.CustomDTO.UserYJBJournalDTO>(show, data.Data, data.Count, page, PageSize, string.Empty);
            return View(gridobj);
        }


        [HttpGet]
        public ActionResult GetMyYJB(string UserId)
        {
            Guid userId = Guid.Parse(UserId);
            var result = Jinher.AMP.BTP.TPS.YJBSV.GetUserYJB(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
