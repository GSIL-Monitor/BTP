using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.SV
{
    public partial class OrderExpressRouteSV : BaseSv, IOrderExpressRoute
    {
        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<OrderExpressRouteExtendDTO> GetExpressRouteByExpNoExt(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO express)
        {
            ResultDTO<OrderExpressRouteExtendDTO> result = new ResultDTO<OrderExpressRouteExtendDTO>();
            try
            {
                if (express == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空!";
                    return result;
                }
                else if (string.IsNullOrWhiteSpace(express.ShipExpCo))
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，物流公司名称不能为空!";
                    return result;
                }
                else if (string.IsNullOrWhiteSpace(express.ExpOrderNo))
                {
                    result.ResultCode = 3;
                    result.Message = "参数错误，运单号不能为空!";
                    return result;
                }

                var cname = express.ShipExpCo.Replace("快递", "").Replace("速递", "").Replace("物流", "").Replace("配送", "").Replace("货运", "").Replace("快运", "").Replace("速运", "").ToUpper();
                var ocInfo = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(cname));
                if (!ocInfo.Any())
                {
                    result.ResultCode = 4;
                    result.Message = "暂不支持该物流公司的快递单号查询!";
                    return result;
                }
                express.ShipperCode = ocInfo.First().ExpCode;

                var oerQuery = (from oer in OrderExpressRoute.ObjectSet()
                                where oer.ShipperCode == express.ShipperCode && oer.ExpOrderNo == express.ExpOrderNo
                                select oer).FirstOrDefault();
                if (oerQuery == null)
                {
                    return result;
                }
                OrderExpressRouteExtendDTO oerDto = new OrderExpressRouteExtendDTO();
                oerDto.FillWith(oerQuery);
                var etQuery = (from et in ExpressTrace.ObjectSet()
                               where et.ExpRouteId == oerQuery.Id
                               orderby et.AcceptTime descending
                               select et).ToList();
                if (etQuery.Any())
                {
                    List<ExpressTraceDTO> oerList = etQuery.ConvertAll<ExpressTraceDTO>(exp => exp.ToEntityData());
                    oerDto.Traces = oerList;
                }
                result.Data = oerDto;

            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.Message = string.Format("按快递单号获取快递路由信息异常，异常信息：{0}", ex);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 向快递鸟订阅需要推送的快递单号(和bp中SubscribeOrderExpress实现相同)
        /// </summary>
        /// <returns></returns>
        public ResultDTO SubscribeOneOrderExpressExt(OrderExpressRoute oer)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            if (oer == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空!";
                return result;
            }
            if (string.IsNullOrWhiteSpace(oer.ShipExpCo))
            {
                result.ResultCode = 2;
                result.Message = "参数错误，物流公司名称不能为空!";
                return result;
            }
            //校验快递公司简称(按快递公司名称找“快递鸟”编码) 
            oer.ShipperCode = "";
            var cname = oer.ShipExpCo.Replace("快递", "").Replace("速递", "").Replace("物流", "").Replace("配送", "").Replace("货运", "").Replace("快运", "").Replace("速运", "").ToUpper();
            var ocInfo = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(cname));
            if (!ocInfo.Any())
            {
                result.ResultCode = 3;
                result.Message = "第三方“快递鸟”暂不支持该物流公司!";
                return result;
            }
            oer.ShipperCode = ocInfo.First().ExpCode;

            //检查 运单号 是否存在
            var oerReady = from op in OrderExpressRoute.ObjectSet()
                           where op.ShipperCode == oer.ShipperCode && op.ExpOrderNo == oer.ExpOrderNo
                           select op;
            if (oerReady.Any())
            {
                result.ResultCode = 4;
                result.Message = "运单号已存在!";
                return result;
            }

            oer.Id = Guid.NewGuid();
            oer.State = 1;
            oer.SubTime = DateTime.Now;
            oer.ModifiedOn = DateTime.Now;
            oer.EntityState = System.Data.EntityState.Added;

            ContextFactory.CurrentThreadContext.SaveObject(oer);
            ContextFactory.CurrentThreadContext.SaveChanges();

            return result;
        }




        /// <summary>
        /// 向快递鸟订阅需要推送的快递单号
        /// </summary>
        /// <returns></returns>
        private ResultDTO SubscribeOrderExpressMultiple(List<OrderExpressRoute> expressList)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };

            try
            {
                if (expressList == null || !expressList.Any())
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空!";
                    return result;
                }

                List<KdSubscribeDTO> kdSubList = new List<KdSubscribeDTO>();
                var ecompany = (from exp in expressList select exp.ShipperCode).Distinct();
                foreach (string g in ecompany)
                {
                    KdSubscribeDTO kdSub = new KdSubscribeDTO();
                    kdSub.Code = g;
                    kdSub.Item = new List<SubItem>();

                    var expNos = from exp in expressList where exp.ShipperCode == g select exp;
                    foreach (var expNo in expNos)
                    {
                        //非法物流单号过滤掉。
                        if (string.IsNullOrWhiteSpace(expNo.ExpOrderNo)
                            || expNo.ExpOrderNo.Length < 6
                            || expNo.ExpOrderNo.Length > 50)
                        {
                            expNo.State = 3;
                            expNo.ModifiedOn = DateTime.Now;
                            expNo.EntityState = System.Data.EntityState.Modified;
                            continue;
                        }
                        kdSub.Item.Add(new SubItem() { No = expNo.ExpOrderNo, Bk = "" });
                    }
                    kdSubList.Add(kdSub);
                }

                //发布订阅。
                KdSubscribeResultDTO kdResult = KdApiSubscribeKit.Instance.orderTracesSubByJson(kdSubList);

                //发布订阅失败。
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //发布订阅完成。
                if (!kdResult.Success)
                {
                    //发布订阅失败，记录下来，job重发。
                    result.ResultCode = 2;
                    result.Message = "订阅物流信息失败！";

                    List<string> errorExpNos = new List<string>();
                    if (!string.IsNullOrWhiteSpace(kdResult.Reason))
                    {
                        string[] errorKD = kdResult.Reason.Split("；".ToCharArray());
                        foreach (string mi in errorKD)
                        {
                            int ind = mi.IndexOf("快递单号");
                            if (ind < 0)
                            {
                                continue;
                            }
                            string expNo = mi.Substring(0, ind).Trim();
                            errorExpNos.Add(expNo);
                        }
                    }
                    if (errorExpNos != null && errorExpNos.Count > 0)
                    {
                        foreach (string eNo in errorExpNos)
                        {
                            var expList = from exp in expressList
                                          where exp.ExpOrderNo == eNo
                                          select exp;
                            if (!expList.Any())
                            {
                                continue;
                            }
                            foreach (OrderExpressRoute oer in expList)
                            {
                                oer.State = 3;
                                oer.ModifiedOn = DateTime.Now;
                                oer.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                    }
                }
                else
                {
                    foreach (OrderExpressRoute oer in expressList)
                    {
                        if (oer.State == 1)
                        {
                            oer.State = 2;
                        }
                        oer.ModifiedOn = DateTime.Now;
                        oer.EntityState = System.Data.EntityState.Modified;
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.Message = string.Format("向快递鸟订阅需要推送的快递单号异常，异常信息：{0}", ex);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 使用job重新订阅快递鸟物流信息（对订阅失败的）。 base.Do(false);
        /// </summary>
        private void SubscribeOrderExpressForJobExt()
        {
            //非正式环境不执行此job
            if (!string.IsNullOrEmpty(CustomConfig.Environment) && CustomConfig.Environment.ToLower() != "release" && CustomConfig.Environment.ToLower() != "pre")
                return;
            LogHelper.Info(string.Format("使用job重新订阅快递鸟物流信息开始"));
            //处理订单状态为已退款
            try
            {
                int pageSize = 20;
                int errorCount = 0;
                while (true)
                {
                    List<OrderExpressRoute> oerList = (from oerp in OrderExpressRoute.ObjectSet()
                                                       where oerp.State == 1
                                                       select oerp).Take(pageSize).ToList();
                    if (!oerList.Any())
                    {
                        return;
                    }
                    if (errorCount > CustomConfig.KdniaoSubscribeErrorCount)
                    {
                        LogHelper.Error("使用job订阅快递鸟物流信息，订阅出错已超过" + CustomConfig.KdniaoSubscribeErrorCount + "次的上限。");
                        return;
                    }
                    var result = SubscribeOrderExpressMultiple(oerList);
                    if (result.ResultCode == 0)
                    {
                        errorCount = 0;
                    }
                    else
                    {
                        errorCount++;
                    }

                }
            }
            catch (Exception ex)
            {
                string s = string.Format("使用job重新订阅快递鸟物流信息异常，异常信息：{0}", ex);
                LogHelper.Error(s);
            }
        }





        /// <summary>
        ///  获取物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJobExt()
        {

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {

                Guid EsAppId = Guid.Parse("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                var commodityOrder = CommodityOrder.ObjectSet().Where(p => (p.State == 1 || p.State == 2) && p.EsAppId == EsAppId).AsQueryable();
                if (commodityOrder.Count() > 0)
                {
                    LogHelper.Info(string.Format("commodityOrder中的Count:{0}", commodityOrder));
                    foreach (var item in commodityOrder)
                    {
                        //当表中存在这个订单式撤销
                        var exEntity = LongisticsTrack.ObjectSet().FirstOrDefault(p => p.Code == item.Code);
                        if (exEntity == null)
                        {
                            LongisticsTrack model = new LongisticsTrack();
                            model.Id = Guid.NewGuid();
                            model.Code = item.Code;
                            model.AppName = item.AppName;
                            model.SupplierName = item.SupplierName;
                            model.SupplierCode = item.SupplierCode;
                            model.AppId = item.AppId;
                            model.EsAppId = item.EsAppId ?? Guid.Empty;
                            model.SupplierType = item.SupplierType.ToString();
                            var orderitem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == item.Id);
                            if (orderitem != null)
                            {
                                model.CommodityDetail = orderitem.Name + "(数量:" + orderitem.Number + ",属性:" + orderitem.CommodityAttributes + ")";
                            }
                            //下订单时间
                            model.Ordertime = item.SubTime;
                            //上传订单时间
                            model.UploadExpresstime = item.ShipmentsTime;

                            var commodityOrderId = item.Id.ToString();
                            var jdOrderitem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId.Contains(commodityOrderId));
                            if (jdOrderitem != null)
                            {
                                //查询京东物流的订单信息
                                var jdwuliu = JdHelper.orderTrack(jdOrderitem.JdOrderId);
                                if (jdwuliu != null)
                                {
                                    JArray objson = JArray.Parse(jdwuliu);
                                    LogHelper.Info(string.Format("obj京东信息:{0}", objson.ToString()));
                                    int count = 0;
                                    foreach (var _item in objson)
                                    {
                                        if (count == 0)
                                        {
                                            model.Expressdeliverytime = DateTime.Parse(_item["msgTime"].ToString());

                                        }
                                        if (count == 1)
                                        {
                                            model.ExpressSdtime = DateTime.Parse(_item["msgTime"].ToString());
                                        }
                                        count++;
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.ExpOrderNo))
                                {
                                    var result = (from a in ExpressTrace.ObjectSet()
                                                  join b in OrderExpressRoute.ObjectSet()
                                                  on a.ExpRouteId equals b.Id
                                                  where b.ExpOrderNo == item.ExpOrderNo
                                                  select a).ToList();

                                    if (result.Count() == 1)
                                    {
                                        model.Expressdeliverytime = result[0].AcceptTime;
                                        model.ExpressSdtime = result[0].AcceptTime;
                                    }
                                    if (result.Count() > 2)
                                    {
                                        model.Expressdeliverytime = result[0].AcceptTime;
                                        model.ExpressSdtime = result[1].AcceptTime;
                                    }

                                }
                            }
                            model.Confirmtime = item.ConfirmTime;
                            model.SubTime = DateTime.Now;
                            model.IsDel = false;
                            model.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(model);
                        }
                    }
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("OTMSJob错误信息:{0}", ex.Message));
            }

        }

        /// <summary>
        /// 获取用户最新的订单物流信息
        /// 查询第一条数据
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpressExt(System.Guid AppId, System.Guid Userid)
        {
            LogHelper.Info("【一条物流】====AppId-->[" + AppId + "]--->UserId=[" + Userid + "]");
            ComOrderExpressNew ExpressNews = new ComOrderExpressNew();
            ResultDTO<ComOrderExpressNew> result = new ResultDTO<ComOrderExpressNew>()
            {
                ResultCode = 1,
                isSuccess = false,
                Data = ExpressNews
            };
            string json = null;
            JObject obj = null;
            try
            {
                //获取已发货的订单信息
                var ComOrder = CommodityOrder.ObjectSet().Where(p => p.State == 2 && p.UserId == Userid && p.EsAppId == AppId).OrderByDescending(p => p.ModifiedOn).FirstOrDefault();

                if (ComOrder != null)
                {
                    List<ExpressTrace> ExpressList = new List<ExpressTrace>();
                    var OrderItemInfo = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == ComOrder.Id).FirstOrDefault();
                    ExpressNews.CommodityOrderId = ComOrder.Id; //订单id
                    ExpressNews.Pic = OrderItemInfo.PicturesPath;//首条订单图片
                    //京东信息
                    var commodityOrderId = ComOrder.Id;

                    /*
                    var jdOrderitem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId.Contains(commodityOrderId));
                    LogHelper.Debug(string.Format("物流信息(订单信息),订单id{0}：", ComOrder.Id));
                    //急速数据信息
                    if (!string.IsNullOrWhiteSpace(ComOrder.ExpOrderNo) && jdOrderitem == null)
                    {
                        //根据快递编号获取急速数据信息
                        json = GetWuliuJson(ComOrder.ExpOrderNo);
                        obj = JObject.Parse(json);
                        LogHelper.Info(string.Format("极速物流信息：订单Id:{0},物流信息:{1}", ComOrder.Id, obj));
                        if (Convert.ToInt32(obj["status"]) != 0)
                        {
                            return result;
                        }
                        JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                        foreach (var item in arr)
                        {
                            ExpressTrace Express = new ExpressTrace();
                            Express.AcceptTime = Convert.ToDateTime(item["time"].ToString());
                            Express.AcceptStation = item["status"].ToString().Trim();
                            ExpressList.Add(Express);
                        }

                        if (Convert.ToInt32(obj["result"]["deliverystatus"]) == 1)
                        {
                            ExpressNews.shipmentsTime = ExpressList.Min(p => p.AcceptTime);
                            ExpressNews.state = "已发货";
                            ExpressNews.Remarked = "您的订单已发货，由" + ComOrder.ShipExpCo + "快递配送，请注意物流信息~";
                            result.ResultCode = 0;
                            result.isSuccess = true;
                        }
                        else if (Convert.ToInt32(obj["result"]["deliverystatus"]) == 2)
                        {
                            ExpressNews.shipmentsTime = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault().AcceptTime;
                            ExpressNews.state = "派件中";
                            ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                            result.ResultCode = 0;
                            result.isSuccess = true;
                        }
                        else
                        {
                            result.ResultCode = 1;
                            result.isSuccess = false;
                        }
                    }
                    else
                    {
                        if (jdOrderitem != null)
                        {
                            var jdwuliu = JdHelper.orderTrack(jdOrderitem.JdOrderId);
                            if (jdwuliu != null)
                            {
                                JArray objson = JArray.Parse(jdwuliu);
                                foreach (var item in objson)
                                {
                                    ExpressTrace Express = new ExpressTrace();
                                    Express.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                                    Express.AcceptStation = item["content"].ToString().Trim();
                                    ExpressList.Add(Express);
                                }
                                var NewExpress = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault();
                                if (NewExpress.AcceptStation.Contains("正在配送") || NewExpress.AcceptStation.Contains("自提"))
                                {
                                    ExpressNews.shipmentsTime = NewExpress.AcceptTime;
                                    ExpressNews.state = "派件中";
                                    ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                                    result.ResultCode = 0;
                                    result.isSuccess = true;
                                }
                                else if (NewExpress.AcceptStation.Contains("已签收"))
                                {
                                    result.ResultCode = 1;
                                    result.isSuccess = false;
                                }
                                else if (NewExpress.AcceptStation.Contains("出库") || NewExpress != null)
                                {
                                    ExpressNews.shipmentsTime = ExpressList.Min(p => p.AcceptTime);
                                    ExpressNews.state = "已发货";
                                    ExpressNews.Remarked = "您的订单已发货，由京东快递配送，请注意物流信息~";
                                    result.ResultCode = 0;
                                    result.isSuccess = true;
                                }
                            }
                        }
                    }*/




                    if (ThirdECommerceHelper.IsJingDongDaKeHu(ComOrder.AppId))
                    {
                        string orderId = commodityOrderId.ToString().ToLower();
                        LogHelper.Info("IsJingDongDaKeHu1--->" + orderId);




                        var jdOrderitem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId.ToLower() == orderId).FirstOrDefault();
                        LogHelper.Info("IsJingDongDaKeHu2--->" + orderId);
                        if (jdOrderitem != null)
                        {
                            LogHelper.Info("IsJingDongDaKeHu3--->" + orderId);
                            var jdwuliu = JdHelper.orderTrack(jdOrderitem.JdOrderId);

                            LogHelper.Info("【一条---京东】京东物流[" + JsonHelper.JsonSerializer(jdOrderitem) + "]====返回-->[" + jdwuliu + "]");
                            if (jdwuliu != null)
                            {
                                JArray objson = JArray.Parse(jdwuliu);
                                foreach (var item in objson)
                                {
                                    ExpressTrace Express = new ExpressTrace();
                                    Express.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                                    Express.AcceptStation = item["content"].ToString().Trim();
                                    ExpressList.Add(Express);
                                }
                                AddExpressNew(ExpressList, ExpressNews, result);
                            }
                        }
                    }
                    else if (ThirdECommerceHelper.IsWangYiYanXuan(ComOrder.AppId))//网易严选或第三方电商
                    {
                        var yxOrder = YXOrder.ObjectSet().FirstOrDefault(p => p.OrderId == commodityOrderId);
                        if (yxOrder != null)
                        {
                            var express = ThirdECommerceHelper.GetOrderItemExpressTrace(AppId, yxOrder.Id);

                            LogHelper.Info("【一条---严选】网易严选物流[" + JsonHelper.JsonSerializer(yxOrder) + "]===返回-->[" + JsonHelper.JsonSerializer(express) + "]");
                            if (express != null)
                            {
                                if (express.ExpressTraceList != null && express.ExpressTraceList.Count > 0)
                                {
                                    express.ExpressTraceList.ForEach(p =>
                                    {
                                        ExpressTrace Express = new ExpressTrace();
                                        Express.AcceptTime = p.Time == null ? DateTime.Now : DateTime.Parse(p.Time);
                                        Express.AcceptStation = p.Desc;
                                        ExpressList.Add(Express);
                                    });
                                    AddExpressNew(ExpressList, ExpressNews, result);
                                }
                            }
                        }
                    }
                    else if (ThirdECommerceHelper.IsSuNingYiGou(ComOrder.AppId))//苏宁店铺
                    {
                        var snOrderitem = SNOrderItem.ObjectSet().FirstOrDefault(p => p.OrderId == commodityOrderId);
                        if (snOrderitem != null)
                        {
                            SNExpressTraceFacade snExpress = new SNExpressTraceFacade();
                            var SuningWuliu = snExpress.GetExpressTrace(commodityOrderId.ToString(), snOrderitem.Id.ToString());

                            LogHelper.Info("【一条---苏宁】苏宁物流[" + JsonHelper.JsonSerializer(snOrderitem) + "]===返回-->[" + JsonHelper.JsonSerializer(SuningWuliu) + "]");
                            if (SuningWuliu != null)
                            {
                                SuningWuliu.ForEach(p =>
                                {
                                    ExpressTrace Express = new ExpressTrace();
                                    Express.AcceptTime = p.OperateTime == null ? DateTime.Now : p.OperateTime.Value;
                                    Express.AcceptStation = p.OperateState;
                                    ExpressList.Add(Express);
                                });
                                AddExpressNew(ExpressList, ExpressNews, result);
                            }
                        }
                    }
                    else if (ThirdECommerceHelper.IsFangZheng(ComOrder.AppId))//方正店铺
                    {
                        var FangZhengWuliu = FangZhengSV.FangZheng_Logistics_InfoList(commodityOrderId);
                        LogHelper.Info("【一条---方正】方正物流[" + commodityOrderId + "]===返回-->[" + JsonHelper.JsonSerializer(FangZhengWuliu) + "]");
                        if (FangZhengWuliu != null)
                        {
                            FangZhengWuliu.ExpressTraceList.ForEach(p =>
                            {
                                ExpressTrace Express = new ExpressTrace();
                                Express.AcceptTime = DateTime.Parse(p.Time);
                                Express.AcceptStation = p.Desc;
                                ExpressList.Add(Express);
                            });
                            AddExpressNew(ExpressList, ExpressNews, result);
                        }
                    }
                    else
                    {
                        json = Express100SV.GetExpressFromKD100(ComOrder.ExpOrderNo, ComOrder.ShipExpCo);
                        obj = JObject.Parse(json);
                        LogHelper.Info(string.Format("【一条---快递100】快递100：订单Id:{0},物流信息:{1}", commodityOrderId, obj));
                        if (obj["message"].ToString().ToLower().Equals("ok"))
                        {
                            JArray arr = (JArray)JsonConvert.DeserializeObject(obj["data"].ToString());
                            foreach (var item in arr)
                            {
                                ExpressTrace Express = new ExpressTrace();
                                Express.AcceptTime = Convert.ToDateTime(item["ftime"].ToString());
                                Express.AcceptStation = item["context"].ToString().Trim();

                                ExpressList.Add(Express);
                            }
                            //快递单当前签收状态，包括0在途中、1已揽收、2疑难、3已签收、4退签、5同城派送中、6退回、7转单等7个状态，其中4-7需要另外开通才有效
                            int deliverystatus = Convert.ToInt32(obj["state"].ToString());
                            if (deliverystatus == 0)
                            {
                                ExpressNews.shipmentsTime = ExpressList.Min(p => p.AcceptTime);
                                ExpressNews.state = "已发货";
                                ExpressNews.Remarked = "您的订单已发货，由" + ComOrder.ShipExpCo + "快递配送，请注意物流信息~";
                                result.ResultCode = 0;
                                result.isSuccess = true;
                            }
                            else if (deliverystatus == 5)
                            {
                                ExpressNews.shipmentsTime = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault().AcceptTime;
                                ExpressNews.state = "派件中";
                                ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                                result.ResultCode = 0;
                                result.isSuccess = true;
                            }
                            else if (deliverystatus == 3)
                            {
                                ExpressNews.shipmentsTime = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault().AcceptTime;
                                ExpressNews.state = "已签收";
                                ExpressNews.Remarked = "您的订单已签收";
                                result.ResultCode = 0;
                                result.isSuccess = true;
                            }
                        }

                    }



                    result.Data = ExpressNews;
                    LogHelper.Info("Express100--->result[" + JsonHelper.JsonSerializer(result) + "]");
                    return result;
                }
                else
                {
                    return result;
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户最新的订单物流信息信息异常，异常信息：{0}", ex));
                return result;
            }
        }
        private void AddExpressNew(List<ExpressTrace> ExpressList, ComOrderExpressNew ExpressNews, ResultDTO<ComOrderExpressNew> result)
        {
            var NewExpress = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault();
            if (NewExpress.AcceptStation.Contains("正在配送") || NewExpress.AcceptStation.Contains("自提"))
            {
                ExpressNews.shipmentsTime = NewExpress.AcceptTime;
                ExpressNews.state = "派件中";
                ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                result.ResultCode = 0;
                result.isSuccess = true;
            }
            else if (NewExpress.AcceptStation.Contains("已签收"))
            {
                result.ResultCode = 1;
                result.isSuccess = false;
            }
            else if (NewExpress.AcceptStation.Contains("出库") || NewExpress != null)
            {
                ExpressNews.shipmentsTime = ExpressList.Min(p => p.AcceptTime);
                ExpressNews.state = "已发货";
                ExpressNews.Remarked = "您的订单已发货，由京东快递配送，请注意物流信息~";
                result.ResultCode = 0;
                result.isSuccess = true;
            }
        }


        /// <summary>
        /// 获取用户最新的所有订单的物流信息
        /// 查询前三条数据
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<ComOrderExpressNew>> GetUserAllNewOrderExpressExt(Guid AppId, Guid Userid)
        {
            ListResult<ComOrderExpressNew> expressNews = new ListResult<ComOrderExpressNew>();
            expressNews.List = new List<ComOrderExpressNew>();
            ResultDTO<ListResult<ComOrderExpressNew>> result = new ResultDTO<ListResult<ComOrderExpressNew>>()
            {
                isSuccess = false,
                ResultCode = 1,
                Data = expressNews
            };
            string json = string.Empty;
            JObject obj = null;
            try
            {
                LogHelper.Info("【三条---京东】京东物流[开始调用]AppId=" + AppId + "  Userid=" + Userid + "");
                //获取已发货的订单信息
                //var comOrders = CommodityOrder.ObjectSet().Where(p => p.State == 2 && p.UserId == Userid && p.EsAppId == AppId).OrderByDescending(p => p.ModifiedOn).ToList();
                var comOrders = CommodityOrder.ObjectSet().Where(p => p.State != 0 && p.UserId == Userid && p.EsAppId == AppId).OrderByDescending(p => p.ModifiedOn).Take(3).ToList();
                LogHelper.Info("【三条---京东】京东物流comOrders-->[" + comOrders.Count() + "]");
                if (comOrders != null)
                {

                    List<ExpressTrace> expressList = null;
                    ComOrderExpressNew expressNew = null;
                    foreach (var order in comOrders.ToList())
                    {
                        expressList = new List<ExpressTrace>();
                        expressNew = new ComOrderExpressNew();
                        var OrderItemInfo = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == order.Id).FirstOrDefault();
                        expressNew.CommodityOrderId = order.Id; //订单id
                        expressNew.Pic = OrderItemInfo.PicturesPath;//首条订单图片
                        //京东信息
                        var commodityOrderId = order.Id.ToString();
                        if (ThirdECommerceHelper.IsJingDongDaKeHu(order.AppId))
                        {
                            //var jdOrderitem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId.Contains(commodityOrderId));
                            var query = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId.Contains(commodityOrderId));

                            //去重后的父单集合
                            var jdOrderList = query.Select(p => new { p.JdOrderId }).GroupBy(p => p.JdOrderId).ToList();

                            if (jdOrderList.Any())
                            {
                                jdOrderList.ForEach(p =>
                                {
                                    var jdwuliu = JdHelper.orderTrack(p.Key);

                                    LogHelper.Info("【三条---京东】京东物流====返回-->[" + jdwuliu + "]");
                                    if (jdwuliu != null)
                                    {
                                        JArray objson = JArray.Parse(jdwuliu);
                                        foreach (var item in objson)
                                        {
                                            ExpressTrace Express = new ExpressTrace();
                                            Express.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                                            Express.AcceptStation = item["content"].ToString().Trim();
                                            expressList.Add(Express);
                                        }
                                        AddExpressNewAll(expressNews, expressList, expressNew, "京东");
                                    }
                                });
                            }

                        }
                        else if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))//网易严选或第三方电商
                        {
                            var orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == order.Id);
                            var express = ThirdECommerceHelper.GetOrderItemExpressTrace(order.AppId, orderItem.Id);
                            LogHelper.Info("【三条---严选】网易严选物流-->[" + JsonHelper.JsonSerializer(express) + "]");
                            if (express != null)
                            {
                                if (express.ExpressTraceList != null && express.ExpressTraceList.Count > 0)
                                {
                                    express.ExpressTraceList.ForEach(p =>
                                    {
                                        ExpressTrace Express = new ExpressTrace();
                                        Express.AcceptTime = p.Time == null ? DateTime.Now : DateTime.Parse(p.Time);
                                        Express.AcceptStation = p.Desc;
                                        expressList.Add(Express);
                                    });

                                    string name = "严选";
                                    var yxExpress = YXExpressTrace.ObjectSet().Where(p=>p.OrderId==order.Id).FirstOrDefault();
                                    if (yxExpress != null)
                                    {
                                        name = yxExpress.ExpressCompany;
                                    }
                                    AddExpressNewAll(expressNews, expressList, expressNew, name);
                                }
                            }
                        }
                        else if (ThirdECommerceHelper.IsSuNingYiGou(order.AppId))//苏宁店铺
                        {
                            var snOrderitem = SNOrderItem.ObjectSet().FirstOrDefault(p => p.OrderId == order.Id);

                            SNExpressTraceFacade snExpress = new SNExpressTraceFacade();
                            var SuningWuliu = snExpress.GetExpressTrace(order.Id.ToString(), snOrderitem.OrderItemId.ToString());
                            LogHelper.Info("【三条---苏宁】苏宁物流[" + order.Id.ToString() + "," + snOrderitem.OrderItemId.ToString() + "]===返回-->[" + JsonHelper.JsonSerializer(SuningWuliu) + "]");
                            if (SuningWuliu != null)
                            {
                                SuningWuliu.ForEach(p =>
                                {
                                    ExpressTrace Express = new ExpressTrace();
                                    Express.AcceptTime = p.OperateTime == null ? DateTime.Now : p.OperateTime.Value;
                                    Express.AcceptStation = p.OperateState;
                                    expressList.Add(Express);
                                });
                                AddExpressNewAll(expressNews, expressList, expressNew, "苏宁");
                            }
                        }
                        else if (ThirdECommerceHelper.IsFangZheng(order.AppId))//方正店铺
                        {
                            var FangZhengWuliu = FangZhengSV.FangZheng_Logistics_InfoList(order.Id);
                            LogHelper.Info("【三条---方正】方正物流[" + commodityOrderId + "]===返回-->[" + JsonHelper.JsonSerializer(FangZhengWuliu) + "]");
                            if (FangZhengWuliu != null)
                            {
                                FangZhengWuliu.ExpressTraceList.ForEach(p =>
                                {
                                    ExpressTrace Express = new ExpressTrace();
                                    Express.AcceptTime = DateTime.Parse(p.Time);
                                    Express.AcceptStation = p.Desc;
                                    expressList.Add(Express);
                                });
                                AddExpressNewAll(expressNews, expressList, expressNew, "方正");
                            }
                        }
                        else
                        {
                            if (order.ExpOrderNo != null)
                            {
                                json = Express100SV.GetExpressFromKD100(order.ExpOrderNo, order.ShipExpCo);
                                obj = JObject.Parse(json);
                                LogHelper.Info(string.Format("【三条---快递100】快递100：订单Id:{0},物流信息:{1}", order.Id, obj));
                                if (obj["message"].ToString().ToLower().Equals("ok"))
                                {
                                    JToken arr = obj["data"];
                                    foreach (var item in arr)
                                    {
                                        ExpressTrace Express = new ExpressTrace();
                                        Express.AcceptTime = Convert.ToDateTime(item["ftime"].ToString());
                                        Express.AcceptStation = item["context"].ToString().Trim();
                                        expressList.Add(Express);
                                    }
                                    LogHelper.Info(string.Format("【三条---快递100】arr[{0}]--->expressList[{1}]", arr.Count(), expressList.Count));



                                    //快递单当前签收状态，包括0在途中、1已揽收、2疑难、3已签收、4退签、5同城派送中、6退回、7转单等7个状态，其中4-7需要另外开通才有效
                                    int deliverystatus = Convert.ToInt32(obj["state"].ToString());
                                    LogHelper.Info("【三条---快递100】deliverystatus-->[" + deliverystatus + "]");
                                    if (deliverystatus == 0)
                                    {
                                        expressNew.shipmentsTime = expressList.Min(p => p.AcceptTime);
                                        expressNew.state = "已发货";
                                        expressNew.Remarked = "您的订单已发货，由" + order.ShipExpCo + "快递配送，请注意物流信息~";
                                        expressNews.List.Add(expressNew);
                                    }
                                    else if (deliverystatus == 5)
                                    {
                                        expressNew.shipmentsTime = expressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault().AcceptTime;
                                        expressNew.state = "派件中";
                                        expressNew.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                                        expressNews.List.Add(expressNew);
                                    }
                                    else if (deliverystatus == 3)
                                    {

                                        expressNew.shipmentsTime = expressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault().AcceptTime;
                                        expressNew.state = "已签收";
                                        expressNew.Remarked = "您的订单已签收";
                                        expressNews.List.Add(expressNew);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                            }
                        }
                    }
                    expressNews.Count = expressNews.List == null ? 0 : expressNews.List.Count();
                    if (expressNews.List.Any())
                    {
                        ListResult<ComOrderExpressNew> express = new ListResult<ComOrderExpressNew>();
                        express.List = new List<ComOrderExpressNew>();
                        express.List = expressNews.List.Take(3).ToList();
                        //取前三条
                        result.Data = express;
                        result.isSuccess = true;
                        result.ResultCode = 0;
                    }
                    else
                    {
                        result.Data = expressNews;
                        result.isSuccess = false;
                        result.ResultCode = 1;
                    }
                    LogHelper.Info(string.Format("获取用户最新的所有订单物流信息返回数据：{0}", JsonHelper.JsonSerializer(result)));
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("获取用户最新的所有订单物流信息信息异常，异常信息：{0}", ex));
                return result;
            }
        }

        private void AddExpressNewAll(ListResult<ComOrderExpressNew> expressNews, List<ExpressTrace> expressList, ComOrderExpressNew expressNew,string name)
        {
            var NewExpress = expressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault();
            if (NewExpress.AcceptStation.Contains("正在配送") || NewExpress.AcceptStation.Contains("自提"))
            {
                expressNew.shipmentsTime = NewExpress.AcceptTime;
                expressNew.state = "派件中";
                expressNew.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                expressNews.List.Add(expressNew);
            }
            else if (NewExpress.AcceptStation.Contains("出库") || NewExpress != null)
            {
                expressNew.shipmentsTime = expressList.Min(p => p.AcceptTime);
                expressNew.state = "已发货";
                expressNew.Remarked = "您的订单已发货，由"+ name + "快递配送，请注意物流信息~";
                expressNews.List.Add(expressNew);
            }
        }


        /// <summary>
        /// 获取极光数据
        /// </summary>
        /// <param name="orderExpressRoute"></param>
        /// <returns></returns>
        private string GetWuliuJson(string ExpOrderNo)
        {
            string sappids = CustomConfig.SappIds;
            List<string> Sappidlist = null;
            if (!string.IsNullOrEmpty(sappids))
            {
                Sappidlist = sappids.Split(new char[] { ',' }).ToList();
            }
            string appkey = CustomConfig.zshappkey;
            string json = null;
            string url = null;
            Guid AppId = Guid.Empty;
            var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.ExpOrderNo == ExpOrderNo);
            if (commodityOrder != null)
            {
                AppId = commodityOrder.AppId;
            }

            if (Sappidlist.Contains(AppId.ToString().ToUpper()))
            {
                var orderExpressRoute = OrderExpressRoute.ObjectSet().FirstOrDefault(p => p.ExpOrderNo == ExpOrderNo);
                //苏宁易购
                url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, orderExpressRoute.ShipperCode, ExpOrderNo);
            }
            else
            {
                //中石化
                url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, "auto", ExpOrderNo);
            }
            json = WebRequestHelper.SendGetRequest(url);
            return json.ToString();
        }

    }
}
