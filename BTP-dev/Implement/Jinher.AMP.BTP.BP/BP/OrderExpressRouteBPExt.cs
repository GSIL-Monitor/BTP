
/***************
功能描述: BTP-EngineBP
作    者: 
创建时间: 2015/12/25 10:12:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Common;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;
using Jinher.AMP.BTP.TPS.Helper;
using System.Threading;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class OrderExpressRouteBP : BaseBP, IOrderExpressRoute
    {

        /// <summary>
        /// 接收快递鸟推送的物流路由信息。  base.Do(false);
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReceiveKdniaoExpressRouteExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> oerList)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            try
            {
                if (oerList == null || !oerList.Any())
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，物流路由信息不能为空!";
                    return result;
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<OrderExpressRoute> oerReadyList = new List<OrderExpressRoute>();
                var distinctOEList = (from oe in oerList select new { oe.ShipperCode, oe.ExpOrderNo }).Distinct();
                foreach (var oe in distinctOEList)
                {

                    var oerQuery = (from oer in OrderExpressRoute.ObjectSet()
                                    where oer.ShipperCode == oe.ShipperCode && oer.ExpOrderNo == oe.ExpOrderNo
                                    select oer).ToList();
                    if (!oerQuery.Any())
                    {
                        continue;
                    }
                    oerReadyList.AddRange(oerQuery);

                    //清理同一运单已有路由信息。
                    var erids = (from oer in oerQuery select oer.Id).Distinct();
                    var etQuery = (from et in ExpressTrace.ObjectSet()
                                   where erids.Contains(et.ExpRouteId)
                                   select et).ToList();
                    foreach (var etq in etQuery)
                    {
                        etq.EntityState = System.Data.EntityState.Deleted;
                    }
                }
                //将运单路由信息保存到db.

                foreach (var oere in oerList)
                {
                    OrderExpressRoute oerNew = (from oerr in oerReadyList
                                                where oerr.ShipperCode == oere.ShipperCode && oerr.ExpOrderNo == oere.ExpOrderNo
                                                select oerr).FirstOrDefault();
                    if (oerNew == null)
                    {
                        continue;
                    }
                    oerNew.ModifiedOn = DateTime.Now;
                    oerNew.EntityState = System.Data.EntityState.Modified;

                    if (oere.Traces != null && oere.Traces.Any())
                    {
                        foreach (ExpressTraceDTO etDto in oere.Traces)
                        {
                            ExpressTrace et = ExpressTrace.CreateExpressTrace();
                            et.FillWith(etDto);
                            et.ExpRouteId = oerNew.Id;
                            et.Id = Guid.NewGuid();
                            et.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(et);
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = string.Format("保存快递鸟推送的物流路由信息异常,异常信息：{0}", ex);
                LogHelper.Error(s);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return result;
        }
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
                var ocDebug = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(cname));
                if (!ocDebug.Any())
                {
                    result.ResultCode = 4;
                    result.Message = "暂不支持该物流公司的快递单号查询!";
                    return result;
                }
                express.ShipperCode = ocDebug.First().ExpCode;

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
            Guid EsappId = Guid.Parse("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
            //判断是否是中石化的订订单
            var commoidtyorder = CommodityOrder.ObjectSet().Where(p => p.ExpOrderNo == oer.ExpOrderNo && p.EsAppId == EsappId).FirstOrDefault();
            if (oer == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空!";
                return result;
            }

            if (string.IsNullOrWhiteSpace(oer.ShipExpCo))
            {
                //判断是否是中石化的订订单
                if (commoidtyorder != null)
                {
                    oer.ShipExpCo = "auto";
                }
                else
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，物流公司名称不能为空!";
                    return result;
                }
            }
            //校验快递公司简称(按快递公司名称找“快递鸟”编码) 
            oer.ShipperCode = "";
            var cname = oer.ShipExpCo.Replace("快递", "").Replace("速递", "").Replace("物流", "").Replace("配送", "").Replace("货运", "").Replace("快运", "").Replace("速运", "").ToUpper();
            var ocDebug = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(cname));
            if (!ocDebug.Any())
            {
                //判断是否是中石化的订订单
                if (commoidtyorder != null)
                {
                    oer.ShipperCode = "auto";
                }
                else
                {
                    result.ResultCode = 3;
                    result.Message = "第三方“快递鸟”暂不支持该物流公司!";
                    return result;
                }
            }
            else
            {
                oer.ShipperCode = ocDebug.First().ExpCode;
            }


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
        /// 批量向快递鸟订阅需要推送的快递单号
        /// </summary>
        /// <returns></returns>
        public ResultDTO BatchSubscribeOneOrderExpressExt(List<OrderExpressRoute> oers)
        {
            try
            {
                if (oers == null || oers.Count == 0) return new ResultDTO() { ResultCode = 1, Message = "参数为空" };
                var shipExpCo = oers.Select(r => r.ShipExpCo).Distinct().ToList()[0];
                var cname = shipExpCo.Replace("快递", "").Replace("速递", "").Replace("物流", "").Replace("配送", "").Replace("货运", "").Replace("快运", "").Replace("速运", "").ToUpper();
                var ocDebug = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(cname));
                if (!ocDebug.Any())
                {
                    return new ResultDTO() { ResultCode = 1, Message = "第三方“快递鸟”暂不支持该物流公司!" };
                }

                string shipperCode = ocDebug.First().ExpCode;
                oers.ForEach(oer =>
                {
                    oer.Id = Guid.NewGuid();
                    oer.ShipperCode = shipperCode;
                });

                var ships = oers.Where(r => !string.IsNullOrEmpty(r.ShipperCode)).Select(r => new { Id = r.Id, ShipperCode = r.ShipperCode, ExpOrderNo = r.ExpOrderNo }).ToList();

                var expOrderNos = ships.Select(r => r.ExpOrderNo).ToList();

                //检查 运单号 是否存在
                var tOrderExpressRoutes = (from op in OrderExpressRoute.ObjectSet()
                                           where expOrderNos.Contains(op.ExpOrderNo)
                                           select op).ToList();

                //检查 运单号 是否存在
                var oerReady = (from s in ships
                                join op in tOrderExpressRoutes on new { ShipperCode = s.ShipperCode, ExpOrderNo = s.ExpOrderNo } equals new { ShipperCode = op.ShipperCode, ExpOrderNo = op.ExpOrderNo }
                                select new
                                {
                                    Id = s.Id,
                                    OrderExpressRoute = op
                                }).ToList();

                oers.ForEach(oer =>
                {
                    if (string.IsNullOrEmpty(oer.ShipperCode)) return;//是否有支持快递鸟
                    if (oerReady.Exists(r => r.Id == oer.Id)) return; //检查 运单号 是否存在

                    oer.Id = Guid.NewGuid();
                    oer.State = 1;
                    oer.SubTime = DateTime.Now;
                    oer.ModifiedOn = DateTime.Now;
                    oer.EntityState = System.Data.EntityState.Added;
                    ContextFactory.CurrentThreadContext.SaveObject(oer);

                });
                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                {
                    return new ResultDTO() { ResultCode = 0, Message = "更新成功" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("批量向快递鸟订阅需要推送的快递单号。BatchSubscribeOneOrderExpressExt"), ex);
            }
            return new ResultDTO() { ResultCode = 1, Message = "更新失败" };
        }


        /// <summary>
        /// 根据快递单号获取快递信息
        /// </summary>
        /// <returns></returns>
        public OrderExpressRouteDTO GetExpressRouteByExpOrderNoExt(string expOrderNo)
        {
            var orderExpressRoute = OrderExpressRoute.ObjectSet().FirstOrDefault(p => p.ExpOrderNo == expOrderNo);
            OrderExpressRouteDTO model = new OrderExpressRouteDTO();
            if (orderExpressRoute != null)
            {
                model.Id = orderExpressRoute.Id;
                model.SubTime = orderExpressRoute.SubTime;
                model.SubId = orderExpressRoute.SubId;
                model.ModifiedOn = orderExpressRoute.ModifiedOn;
                model.ShipExpCo = orderExpressRoute.ShipExpCo;
                model.ExpOrderNo = orderExpressRoute.ExpOrderNo;
                model.ShipperCode = orderExpressRoute.ShipperCode;
                model.State = orderExpressRoute.State;
                model.Deliverystatus = orderExpressRoute.Deliverystatus;
            }
            return model;
        }


        /// <summary>
        /// 修改快递信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO UpdateExpressRouteExt(OrderExpressRouteDTO model)
        {

            ResultDTO dto = null;
            try
            {
                LogHelper.Debug(string.Format("物流快递信息:Id{0}State{1}Deliverystatus{2},", model.Id, model.State, model.Deliverystatus));
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var orderExpressRoute = OrderExpressRoute.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (orderExpressRoute != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Deliverystatus))
                    {
                        orderExpressRoute.Deliverystatus = model.Deliverystatus;
                    }
                    if (!string.IsNullOrEmpty(model.State.ToString()))
                    {
                        orderExpressRoute.State = model.State;
                    }
                    orderExpressRoute.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 0, Message = "快递信息不存在", isSuccess = true };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("快递信息修改异常。OrderExpressRoute：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        ///  获取京东物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJdJobExt()
        {
            try
            {

                //为了避免接口重复调用我只同步30天内的数据
                DateTime startTime = DateTime.Now.AddDays(-30);
                DateTime endTime = DateTime.Now.AddDays(1);
                var jdOrderitem = JdOrderItem.ObjectSet().Where(p => p.SubTime > startTime && p.SubTime < endTime && p.State != 1).GroupBy(p => p.JdOrderId).Select(p => p.FirstOrDefault()).ToList();
                if (jdOrderitem.Any())
                {
                    foreach (var _item in jdOrderitem)
                    {
                        //查询京东物流信息是否存在
                        var jdExpresstrace = JdExpressTrace.ObjectSet().Where(p => p.ExpRouteId == _item.Id).ToList();
                        if (jdExpresstrace.Count() == 0)
                        {
                            //添加京东物流信息
                            AddJdwuliu(_item);
                        }
                        else
                        {
                            //先删除在增加
                            var result = DeleteJdwuliu(jdExpresstrace);
                            if (result.isSuccess == true)
                            {
                                AddJdwuliu(_item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetOrderExpressForJdJobExt错误信息:{0}", ex.Message), ex);
            }

        }


        ///// <summary>
        /////  获取急速数据物流跟踪信息
        ///// </summary>
        public void GetOrderExpressForJsJobExt()
        {
            try
            {
                LogHelper.Info("GetOrderExpressForJsJobExt********开始执行");
                //为了避免接口重复调用我只同步30天内的数据
                DateTime startTime = DateTime.Now.AddDays(-30);
                DateTime endTime = DateTime.Now.AddDays(1);
                Guid EsAppId = Guid.Parse("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                var commodityOrder = CommodityOrder.ObjectSet().Where(p => p.EsAppId == EsAppId && (p.State == 3) && p.SubTime > startTime && p.SubTime < endTime && !string.IsNullOrEmpty(p.ExpOrderNo)).OrderByDescending(p=>p.ModifiedOn).Select(p => new { p.ExpOrderNo, p.Id, p.AppId, p.ShipExpCo }).ToList();
                //var commodityOrder = CommodityOrder.ObjectSet().Where(p => p.EsAppId == EsAppId && (p.State != 0 || p.State != 1) && p.SubTime > startTime && p.SubTime < endTime && !string.IsNullOrEmpty(p.ExpOrderNo)).Select(p => new { p.ExpOrderNo, p.Id, p.AppId }).ToList();
                List<string> expOrdernolist = new List<string>();
                commodityOrder.ForEach(p =>
                {
                    expOrdernolist.Add(p.ExpOrderNo);
                });
                var orderexpressroutelist = OrderExpressRoute.ObjectSet().Where(p => expOrdernolist.Contains(p.ExpOrderNo)&&string.IsNullOrEmpty(p.Deliverystatus)).ToList();
                LogHelper.Info("GetOrderExpressForJsJobExt********开始执行***orderexpressroutelist=["+ orderexpressroutelist.Count+ "]");
                if (orderexpressroutelist.Any())
                {
                    foreach (var item in orderexpressroutelist)
                    {
                        var order = commodityOrder.FirstOrDefault(p => p.ExpOrderNo == item.ExpOrderNo);
                        Guid AppId = order.AppId;// commodityOrder.FirstOrDefault(p => p.ExpOrderNo == item.ExpOrderNo).AppId;
                        string ShipExpCo = order.ShipExpCo;
                        //添加快递100信息
                        GetExpress100Wuliu(item, AppId, ShipExpCo);
                        Thread.Sleep(200);

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetOrderExpressForJsJobExt错误信息:{0}", ex.Message), ex);
            }

            LogHelper.Info("GetOrderExpressForJsJobExt********执行完成");
        }


        /// <summary>
        /// 添加京东物流信息
        /// </summary>
        private void AddJdwuliu(JdOrderItem jdOrderitem)
        {
            try
            {
                //查询京东物流的订单信息
                var jdwuliu = JdHelper.orderTrack(jdOrderitem.JdOrderId);
                if (jdwuliu != null)
                {
                    JArray objson = JArray.Parse(jdwuliu);
                    LogHelper.Info(string.Format("OTMSJobobj京东信息:{0}", objson.ToString()));
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into JdExpressTrace (Id,ExpRouteId,AcceptTime,AcceptStation,Remark) values");
                    foreach (var _item in objson)
                    {
                        sb.Append("(");
                        sb.Append("'" + Guid.NewGuid() + "','" + jdOrderitem.Id + "','" + DateTime.Parse(_item["msgTime"].ToString()) + "','" + _item["content"].ToString().Trim() + "',''");
                        sb.Append("),");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    JdExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("OTMSJob添加京东物流异常信息:{0}", ex));
            }


        }

        /// <summary>
        /// 删除京东物流信息
        /// </summary>
        private ResultDTO DeleteJdwuliu(List<JdExpressTrace> jdExpresstrace)
        {
            ResultDTO dto = null;
            try
            {
                Guid ExpRouteId = jdExpresstrace.FirstOrDefault().ExpRouteId;
                string sql = " delete from JdExpressTrace where ExpRouteId='" + ExpRouteId + "'";
                JdExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sql);
                dto = new ResultDTO() { ResultCode = 0, Message = "删除成功", isSuccess = true };

            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("OTMSJob删除京东物流异常信息:{0}", ex));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;

        }

        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpressExt(System.Guid AppId, System.Guid Userid)
        {
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
                    var OrderItemDebug = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == ComOrder.Id).FirstOrDefault();
                    ExpressNews.CommodityOrderId = ComOrder.Id; //订单id
                    ExpressNews.Pic = OrderItemDebug.PicturesPath;//首条订单图片
                    ExpressTrace Express = new ExpressTrace();
                    //急速数据信息
                    if (!string.IsNullOrWhiteSpace(ComOrder.ExpOrderNo))
                    {
                        var orderexpressroute = OrderExpressRoute.ObjectSet().FirstOrDefault(p => p.ExpOrderNo == ComOrder.ExpOrderNo);
                        //根据快递编号获取急速数据信息
                        json = GetWuliuJson(orderexpressroute, ComOrder.AppId);
                        LogHelper.Debug(string.Format("极速快递物流信息：{0}", JsonHelper.JsonSerializer(json)));
                        obj = JObject.Parse(json);
                        if (Convert.ToInt32(obj["status"]) != 0)
                        {
                            return result;
                        }
                        JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                        foreach (var item in arr)
                        {
                            Express.AcceptTime = Convert.ToDateTime(item["time"].ToString());
                            Express.AcceptStation = item["status"].ToString().Trim();
                            ExpressList.Add(Express);
                        }
                        var NewExpress = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault();
                        if (Convert.ToInt32(obj["result"]["deliverystatus"]) == 1)
                        {
                            result.ResultCode = 0;
                            result.isSuccess = true;
                            ExpressNews.shipmentsTime = ComOrder.ModifiedOn;
                            ExpressNews.state = "已发货";
                            ExpressNews.Remarked = "您的订单已发货，由" + ComOrder.ShipExpCo + "快递配送，请注意物流信息~";

                        }
                        else if (Convert.ToInt32(obj["result"]["deliverystatus"]) == 2)
                        {
                            result.ResultCode = 0;
                            result.isSuccess = true;
                            ExpressNews.shipmentsTime = NewExpress.AcceptTime;
                            ExpressNews.state = "派件中";
                            ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                        }
                        else
                        {
                            result.ResultCode = 1;
                            result.isSuccess = false;
                        }
                    }
                    else
                    {
                        //京东信息
                        var commodityOrderId = ComOrder.Id.ToString();
                        var jdOrderitem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId.Contains(commodityOrderId));
                        if (jdOrderitem != null)
                        {
                            var jdwuliu = JdHelper.orderTrack(jdOrderitem.JdOrderId);
                            LogHelper.Debug(string.Format("京东物流信息：{0}", JsonHelper.JsonSerializer(jdwuliu)));
                            if (jdwuliu != null)
                            {

                                JArray objson = JArray.Parse(jdwuliu);
                                foreach (var item in objson)
                                {
                                    Express.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                                    Express.AcceptStation = item["content"].ToString().Trim();
                                    ExpressList.Add(Express);
                                }
                                var NewExpress = ExpressList.OrderByDescending(p => p.AcceptTime).FirstOrDefault();
                                if (NewExpress.AcceptStation.Contains("正在配送"))
                                {
                                    result.ResultCode = 0;
                                    result.isSuccess = true;
                                    ExpressNews.shipmentsTime = NewExpress.AcceptTime;
                                    ExpressNews.state = "派件中";
                                    ExpressNews.Remarked = "您的订单已开始派送，请保持电话畅通，确认商品外包装完好后签收~";
                                }
                                else if (NewExpress.AcceptStation.Contains("订单已签收"))
                                {
                                    result.ResultCode = 1;
                                    result.isSuccess = false;
                                }
                                else
                                {
                                    result.ResultCode = 0;
                                    result.isSuccess = true;
                                    ExpressNews.shipmentsTime = ComOrder.ModifiedOn;
                                    ExpressNews.state = "已发货";
                                    ExpressNews.Remarked = "您的订单已发货，由京东快递配送，请注意物流信息~";
                                }
                            }
                        }
                    }
                    result.Data = ExpressNews;
                    LogHelper.Debug(string.Format("获取用户最新的订单物流信息返回数据：{0}", JsonHelper.JsonSerializer(result)));
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

        /// <summary>
        /// 获取极光数据
        /// </summary>
        /// <param name="orderExpressRoute"></param>
        /// <returns></returns>
        private string GetWuliuJson(OrderExpressRoute orderExpressRoute, Guid AppId)
        {
            string json = null;
            try
            {
                string sappids = CustomConfig.SappIds;
                List<string> Sappidlist = null;
                if (!string.IsNullOrEmpty(sappids))
                {
                    Sappidlist = sappids.Split(new char[] { ',' }).ToList();
                }
                string appkey = CustomConfig.zshappkey;
                string url = null;
                if (Sappidlist.Contains(AppId.ToString().ToUpper()))
                {
                    //苏宁易购
                    url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, orderExpressRoute.ShipperCode, orderExpressRoute.ExpOrderNo);
                }
                else
                {
                    //中石化
                    url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, "auto", orderExpressRoute.ExpOrderNo);
                }
                LogHelper.Debug(string.Format("获取Geturl:{0}", url));
                json = WebRequestHelper.SendGetRequest(url);
                LogHelper.Debug(string.Format("获取GetWuliuJsonjson:{0}", json));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取GetWuliuJson:{0}", ex));
            }
            return json.ToString();
        }

        /// <summary>
        /// 删除极光物流数据信息
        /// </summary>
        private ResultDTO DeleteExpressTrace(Guid orderExpressRouteId)
        {
            ResultDTO dto = null;
            try
            {
                string sql = " delete from ExpressTrace where ExpRouteId='" + orderExpressRouteId + "'";
                ExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sql);
                dto = new ResultDTO() { ResultCode = 0, Message = "删除成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("OTMSJob删除急速物流异常信息:{0}", ex));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;

        }

        /*
        private void GetWuliu(OrderExpressRoute orderExpressRoute,Guid AppId)
        {
            JObject obj = null;
            string json = null;
            try
            {
                if (string.IsNullOrWhiteSpace(orderExpressRoute.Deliverystatus))
                {
                    #region 当没有物流信息时添加数据
                    json = GetWuliuJson(orderExpressRoute, AppId);
                    obj = JObject.Parse(json);
                    if (obj["status"].ToString() != null && obj["status"].ToString() == "0")
                    {
                        //先删除数据然后添加
                        var res = DeleteExpressTrace(orderExpressRoute.Id);
                        if (res.isSuccess==true)
                        {
                            JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                            if (arr.Count() > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("insert into ExpressTrace (Id,ExpRouteId,AcceptTime,AcceptStation,Remark) values");
                                foreach (var item in arr)
                                {
                                    sb.Append("(");
                                    sb.Append("'" + Guid.NewGuid() + "','" + orderExpressRoute.Id + "','" + Convert.ToDateTime(item["time"].ToString()) + "','" + item["status"].ToString().Trim() + "',''");
                                    sb.Append("),");
                                }
                                sb.Remove(sb.Length - 1, 1);
                                ExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sb.ToString());
                            }
                            int deliverystatus = Convert.ToInt32(obj["result"]["deliverystatus"].ToString());
                            if (deliverystatus == 1)
                            {
                                orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.ZTZ);
                            }
                            if (deliverystatus == 2)
                            {
                                orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.PJZ);
                            }
                            if (deliverystatus == 3)
                            {
                                orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.YQS);
                            }
                            if (deliverystatus == 4)
                            {
                                orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.PSSB);
                            }
                            string sql = "update OrderExpressRoute set Deliverystatus='" + orderExpressRoute.Deliverystatus + "' where Id='" + orderExpressRoute.Id + "'";
                            OrderExpressRoute.ObjectSet().Context.ExecuteStoreCommand(sql);
                        }
                        
                    }

                    #endregion
                }
                else
                {
                    //不存在下面两种情况时更新状态
                    if (!orderExpressRoute.Deliverystatus.Contains("已签收") && !orderExpressRoute.Deliverystatus.Contains("派送失败"))
                    {
                        #region 当物流信息存在时
                        json = GetWuliuJson(orderExpressRoute, AppId);
                        obj = JObject.Parse(json);
                        if (obj["status"].ToString() != null && obj["status"].ToString() == "0")
                        {
                            //先删除数据然后添加
                            var res = DeleteExpressTrace(orderExpressRoute.Id);
                            if (res.isSuccess == true)
                            {
                                JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                                if (arr.Count() > 0)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("insert into ExpressTrace (Id,ExpRouteId,AcceptTime,AcceptStation,Remark) values");

                                    foreach (var item in arr)
                                    {
                                        sb.Append("(");
                                        sb.Append("'" + Guid.NewGuid() + "','" + orderExpressRoute.Id + "','" + Convert.ToDateTime(item["time"].ToString()) + "','" + item["status"].ToString() + "',''");
                                        sb.Append("),");
                                    }
                                    sb.Remove(sb.Length - 1, 1);
                                    ExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sb.ToString());
                                }
                                int deliverystatus = Convert.ToInt32(obj["result"]["deliverystatus"].ToString());
                                if (deliverystatus == 1)
                                {
                                    orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.ZTZ);
                                }
                                if (deliverystatus == 2)
                                {
                                    orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.PJZ);
                                }
                                if (deliverystatus == 3)
                                {
                                    orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.YQS);
                                }
                                if (deliverystatus == 4)
                                {
                                    orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.PSSB);
                                }
                                string sql = "update OrderExpressRoute set Deliverystatus='" + orderExpressRoute.Deliverystatus + "' where Id='" + orderExpressRoute.Id + "'";
                                OrderExpressRoute.ObjectSet().Context.ExecuteStoreCommand(sql);
                            }

                        }
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("OTMSJob急速数据物流快递异常信息:{0}", ex.Message));
            }

        }
        */
        /// <summary>
        /// 获取快递100物流信息
        /// </summary>
        /// <param name="orderExpressRoute"></param>
        /// <param name="AppId"></param>
        /// <param name="shipExpCo"></param>
        private void GetExpress100Wuliu(OrderExpressRoute orderExpressRoute, Guid AppId, string shipExpCo)
        {
            JObject obj = null;
            string json = null;
            try
            {

                #region 当没有物流信息时添加数据


                json = Express100SV.GetExpressFromKD100(orderExpressRoute.ExpOrderNo, shipExpCo);
                //LogHelper.Info("【获取快递100】AppId=[" + AppId + "]shipExpCo=[" + shipExpCo + "]====返回-->[" + json + "]");
                obj = JObject.Parse(json);
                if (obj["message"].ToString().ToLower().Equals("ok"))
                {
                    //先删除数据然后添加
                    var res = DeleteExpressTrace(orderExpressRoute.Id);
                    if (res.isSuccess == true)
                    {
                        string data = obj["data"].ToString();
                        JArray arr = JArray.Parse(data);
                        //JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                        if (arr.Count() > 0)
                        {
                            JToken first = arr[arr.Count() - 1];
                            JToken end = arr[0];


                            StringBuilder sb = new StringBuilder();
                            sb.Append("insert into ExpressTrace (Id,ExpRouteId,AcceptTime,AcceptStation,Remark) values");

                            if (first != null)
                            {
                                sb.Append("(");
                                sb.Append("'" + Guid.NewGuid() + "','" + orderExpressRoute.Id + "','" + Convert.ToDateTime(first["ftime"].ToString()) + "','" + first["context"].ToString().Trim() + "',''");
                                sb.Append("),");
                            }
                            if (end != null)
                            {
                                sb.Append("(");
                                sb.Append("'" + Guid.NewGuid() + "','" + orderExpressRoute.Id + "','" + Convert.ToDateTime(end["ftime"].ToString()) + "','" + end["context"].ToString().Trim() + "',''");
                                sb.Append("),");
                            }

                            sb.Remove(sb.Length - 1, 1);
                            ExpressTrace.ObjectSet().Context.ExecuteStoreCommand(sb.ToString());
                            //LogHelper.Info("【获取快递100】Job 同步成功Sql--->[" + sb.ToString() + "]");
                        }
                        //快递单当前签收状态，包括0在途中、1已揽收、2疑难、3已签收、4退签、5同城派送中、6退回、7转单等7个状态，其中4-7需要另外开通才有效
                        int deliverystatus = Convert.ToInt32(obj["state"].ToString());
                        if (deliverystatus == 3)
                        {
                            orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.YQS);

                            string sql = "update OrderExpressRoute set Deliverystatus='" + orderExpressRoute.Deliverystatus + "' where Id='" + orderExpressRoute.Id + "'";
                            OrderExpressRoute.ObjectSet().Context.ExecuteStoreCommand(sql);
                        }
                       else if (deliverystatus == 6)
                        {
                            orderExpressRoute.Deliverystatus = new EnumHelper().GetDescription(DeliverystatusEnum.PSSB);

                            string sql = "update OrderExpressRoute set Deliverystatus='" + orderExpressRoute.Deliverystatus + "' where Id='" + orderExpressRoute.Id + "'";
                            OrderExpressRoute.ObjectSet().Context.ExecuteStoreCommand(sql);
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetExpress100Wuliu 快递100接口调用错误:{0}", ex.Message));
            }

        }


    }
}
