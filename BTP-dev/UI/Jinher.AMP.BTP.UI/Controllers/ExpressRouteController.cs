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
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
//using Jinher.AMP.BTP.Deploy.ExpressTraceDTO;
namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 接收物流信息异步通知。
    /// </summary>
    public class ExpressRouteController : BaseController
    {

        /// <summary>
        /// 手机端物流信息展示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string shipExpCo, string expOrderNo, string SubExpressNos, string CommodityOrderId, string JdOrderId)
        {
            var appId = Guid.Empty;
            Guid.TryParse(Request["OrderAppId"], out appId);
            var orderId = Guid.Empty;
            Guid.TryParse(Request["CommodityOrderId"], out orderId);
            var orderItemId = Guid.Empty;
            Guid.TryParse(Request["OrderItemId"], out orderItemId);

            var result = new ResultDTO<OrderExpressRouteExtendDTO>()
            {
                Data = new OrderExpressRouteExtendDTO { Traces = new List<ExpressTraceDTO>() }
            };

            if (ThirdECommerceHelper.IsWangYiYanXuan(appId))//网易严选或第三方电商
            {
                var expressCompany = string.Empty;
                var expressNo = string.Empty;
                var express = ThirdECommerceHelper.GetOrderItemExpressTrace(appId, orderItemId);
                if (express != null)
                {
                    expressCompany = express.ExpressCompany;
                    expressNo = express.ExpressNo;
                    if (express.ExpressTraceList != null && express.ExpressTraceList.Count > 0)
                    {
                        express.ExpressTraceList.ForEach(p =>
                        {
                            result.Data.Traces.Add(new ExpressTraceDTO
                            {
                                Id = Guid.NewGuid(),
                                ExpRouteId = Guid.NewGuid(),
                                AcceptTime = DateTime.Parse(p.Time),
                                AcceptStation = p.Desc,
                                Remark = string.Empty
                            });
                        });
                    }
                }
                ViewBag.ExpressRoute = result;
                ViewBag.ShipperCode = expressCompany;
                ViewBag.ExpOrderNo = expressNo;
                ViewBag.SubExpressNos = SubExpressNos;
            }
            else if (ThirdECommerceHelper.IsJingDongDaKeHu(appId))//京东大客户
            {
                //查询京东物流的订单信息
                var jdwuliu = JdHelper.orderTrack(JdOrderId);
                if (jdwuliu != null)
                {
                    List<ExpressTraceDTO> data = new List<ExpressTraceDTO>();
                    JArray objson = JArray.Parse(jdwuliu);
                    foreach (var item in objson)
                    {
                        ExpressTraceDTO entity = new ExpressTraceDTO();
                        entity.Id = Guid.NewGuid();
                        entity.ExpRouteId = Guid.NewGuid();
                        entity.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                        entity.AcceptStation = item["content"].ToString();
                        entity.Remark = null;
                        data.Add(entity);
                    }
                    result.Data.Traces = data;
                }
                ViewBag.ExpressRoute = result;
                ViewBag.ShipperCode = "京东快递";
                ViewBag.ExpOrderNo = JdOrderId;
            }
            else if (ThirdECommerceHelper.IsSuNingYiGou(appId))//苏宁店铺
            {
               
                SNExpressTraceFacade snExpress = new SNExpressTraceFacade();
                var SuningWuliu = snExpress.GetExpressTrace(orderId.ToString(), orderItemId.ToString());
                //LogHelper.Info(string.Format("苏宁物流2:{0}",Newtonsoft.Json.JsonConvert.SerializeObject(SuningWuliu)));
                if (SuningWuliu != null)
                {
                    result.Data.Traces = SuningWuliu.Select(s => new ExpressTraceDTO
                    {
                        Id = Guid.NewGuid(),
                        ExpRouteId = Guid.NewGuid(),
                        AcceptTime = s.OperateTime == null ? DateTime.MinValue : (DateTime)s.OperateTime,
                        AcceptStation = s.OperateState,
                        Remark = null,
                    }).ToList();
                    ViewBag.ExpressRoute = result;
                    ViewBag.ShipperCode = "苏宁易购";
                    ViewBag.ExpOrderNo = SuningWuliu[0].OrderId;
                }
                else
                {
                    ViewBag.ExpressRoute = null;
                    ViewBag.ShipperCode = "无物流";
                    ViewBag.ExpOrderNo = "";
                }
            }
            else if (ThirdECommerceHelper.IsFangZheng(appId))//方正店铺
            {
                var FangZhengWuliu = FangZhengSV.FangZheng_Logistics_InfoList(orderId);
                if (FangZhengWuliu != null)
                {
                    result.Data.Traces = FangZhengWuliu.ExpressTraceList.Select(s => 
                    new ExpressTraceDTO
                    {
                        Id = Guid.NewGuid(),
                        ExpRouteId = Guid.NewGuid(),
                        AcceptTime = DateTime.Parse(s.Time),
                        AcceptStation = s.Desc,
                        Remark = string.Empty
                    }).ToList();
                    ViewBag.ExpressRoute = result;
                    ViewBag.ShipperCode = FangZhengWuliu.ExpressCompany;
                    ViewBag.ExpOrderNo = FangZhengWuliu.ExpressNo;
                }
                else
                {
                    ViewBag.ExpressRoute = null;
                    ViewBag.ShipperCode = "无物流";
                    ViewBag.ExpOrderNo = "";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(shipExpCo) && !string.IsNullOrWhiteSpace(expOrderNo))
                {
                    OrderExpressRouteDTO oerDto = new OrderExpressRouteDTO();
                    oerDto.ExpOrderNo = expOrderNo;
                    oerDto.ShipExpCo = shipExpCo;
                    result=Express100SV.GetExpressFromKD100DTO(expOrderNo, shipExpCo);
                    ViewBag.ExpressRoute = result;
                    ViewBag.ShipperCode = shipExpCo;
                    ViewBag.ExpOrderNo = expOrderNo;

                    /*
                    BTP.ISV.Facade.OrderExpressRouteFacade oerFacade = new BTP.ISV.Facade.OrderExpressRouteFacade();
                    result = oerFacade.GetExpressRouteByExpNo(oerDto);
                    if (result.Data != null)
                    {
                        ViewBag.ExpressRoute = result;
                        ViewBag.ShipperCode = shipExpCo;
                        ViewBag.ExpOrderNo = result.Data.ExpOrderNo;
                    } 
                    else
                    {
                        ViewBag.ExpressRoute = null;
                        ViewBag.ShipperCode = "无物流";
                        ViewBag.ExpOrderNo = "";
                    }
                    */
                }
                else
                {
                    ViewBag.ExpressRoute = null;
                    ViewBag.ShipperCode = "无物流";
                    ViewBag.ExpOrderNo = "";
                }

            }
            //return Redirect("https://m.kuaidi100.com/index_all.html?type=&postid=" + ViewBag.ExpOrderNo + "&callbackurl=" + System.Web.HttpUtility.UrlEncode(Request["backUrl"])); 
            return View();
        }
        /// <summary>
        /// 接收物流信息异步通知
        /// </summary>
        [HttpPost]
        public void Receive()
        {
            string reqdata = Request["RequestData"];
            LogHelper.Debug(string.Format("进入ExpressRouteController.Receive，RequestData：{0}", reqdata));

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            KdNotifyResult kdnResult = new KdNotifyResult();
            kdnResult.Success = false;
            kdnResult.UpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            kdnResult.EBusinessID = BTP.Common.CustomConfig.KdniaoEBusinessID;

            StringBuilder sbResult = new StringBuilder();
            jsSerializer.Serialize(kdnResult, sbResult);



            KdNotifyDTO notifyModel = jsSerializer.Deserialize<KdNotifyDTO>(reqdata);


            if (notifyModel.Count <= 0 || notifyModel.Data.Count <= 0)
            {
                Response.Write(sbResult.ToString());
                return;
            }
            if (notifyModel.EBusinessID != BTP.Common.CustomConfig.KdniaoEBusinessID)
            {
                Response.Write(sbResult.ToString());
                return;
            }

            List<OrderExpressRouteExtendDTO> oerList = new List<OrderExpressRouteExtendDTO>();
            foreach (KDRouteData kdr in notifyModel.Data)
            {
                OrderExpressRouteExtendDTO oerExtend = new OrderExpressRouteExtendDTO();
                oerExtend.ExpOrderNo = kdr.LogisticCode;
                oerExtend.ShipExpCo = kdr.ShipperCode;
                oerExtend.ShipperCode = kdr.ShipperCode;
                oerExtend.State = 2;
                oerExtend.Traces = new List<Deploy.ExpressTraceDTO>();
                oerList.Add(oerExtend);

                if (!kdr.Traces.Any())
                {
                    continue;
                }

                foreach (RouteDetail rd in kdr.Traces)
                {
                    ExpressTraceDTO expTrace = new ExpressTraceDTO();
                    expTrace.AcceptStation = rd.AcceptStation;
                    expTrace.AcceptTime = rd.AcceptTime;
                    expTrace.ExpRouteId = Guid.Empty;
                    expTrace.Remark = rd.Remark;
                    if (string.IsNullOrEmpty(expTrace.AcceptStation))
                        continue;
                    oerExtend.Traces.Add(expTrace);
                }
            }
            OrderExpressRouteFacade oerFacade = new OrderExpressRouteFacade();
            ResultDTO result = oerFacade.ReceiveKdniaoExpressRoute(oerList);

            kdnResult.UpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            kdnResult.Success = result.ResultCode == 0 ? true : false;
            sbResult.Clear();
            jsSerializer.Serialize(kdnResult, sbResult);
            Response.Write(sbResult.ToString());
        }
        /// <summary>
        /// 根据子订单信息返回物流信息。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SubExpressNos()
        {
            string shipExpCo = "";
            string expOrderNo = "";
            string SubExpressNos = "";
            string SubExpressNo = Request["SubExpressNo"];
            string CommodityOrderId = "";
            string JdOrderId = "";


            var appId = Guid.Empty;
            Guid.TryParse(Request["OrderAppId"], out appId);
            var orderId = Guid.Empty;
            Guid.TryParse(Request["CommodityOrderId"], out orderId);
            var orderItemId = Guid.Empty;
            Guid.TryParse(Request["OrderItemId"], out orderItemId);

            var result = new ResultDTO<OrderExpressRouteExtendDTO>()
            {
                Data = new OrderExpressRouteExtendDTO { Traces = new List<ExpressTraceDTO>() }
            };

            if (ThirdECommerceHelper.IsWangYiYanXuan(appId))//网易严选
            {
                var expressCompany = string.Empty;
                var expressNo = string.Empty;
                var deliveryInfo = YXOrderHelper.GetExpressInfoSub(orderItemId, SubExpressNo);
                if (deliveryInfo != null)
                {
                    expressCompany = deliveryInfo.company;
                    expressNo = deliveryInfo.number;
                    if (deliveryInfo.content != null && deliveryInfo.content.Count > 0)
                    {
                        deliveryInfo.content.ForEach(p =>
                        {
                            result.Data.Traces.Add(new ExpressTraceDTO
                            {
                                Id = Guid.NewGuid(),
                                ExpRouteId = Guid.NewGuid(),
                                AcceptTime = DateTime.Parse(DateTime.Parse(p.time).ToString("yyyy-MM-dd HH:mm:ss")),
                                AcceptStation = p.desc,
                                Remark = string.Empty
                            });
                        });
                    }
                }

            }
            else
                return Json("不是严选的数据！", JsonRequestBehavior.AllowGet);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}

