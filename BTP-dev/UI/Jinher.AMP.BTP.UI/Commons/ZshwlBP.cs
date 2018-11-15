using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;
using Newtonsoft.Json;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Common.Loging;


namespace Jinher.AMP.BTP.UI.Commons
{
    public static class ZshwlBP
    {
        private static string GetWuliuJson(OrderExpressRouteDTO orderExpressRoute,Guid AppId)
        {
            string sappids = CustomConfig.SappIds;
            List<string> Sappidlist = null;
            if (!string.IsNullOrEmpty(sappids))
            {
                Sappidlist = sappids.Split(new char[] { ',' }).ToList();
            }
            string appkey = CustomConfig.zshappkey;
            string json = null;
            try
            {
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
                json = WebRequestHelper.SendGetRequest(url);
            }
            catch (Exception ex) { }
            return json.ToString();
        }


        public static ResultDTO GetWuliu(OrderExpressRouteDTO orderExpressRoute,Guid AppId)
        {
            LogHelper.Debug(string.Format("进入物流:运单号{0}物流类型:{1}AppId{2},", orderExpressRoute.ExpOrderNo, orderExpressRoute.ShipperCode, AppId));
            ResultDTO result = null;
            JObject obj = null;
            string json = null;
            try
            {
                OrderExpressRouteFacade orderExpressRoutefacade = new OrderExpressRouteFacade();
                ExpressTraceFacade expressTraceFacade = new ExpressTraceFacade();
                if (string.IsNullOrWhiteSpace(orderExpressRoute.Deliverystatus))
                {
                    #region 当没有物流信息时添加数据
                    json = GetWuliuJson(orderExpressRoute,AppId);
                    obj = JObject.Parse(json);
                    if (obj["status"].ToString() != null && obj["status"].ToString() == "0")
                    {
                        JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                        if (arr.Count() > 0)
                        {
                            List<ExpressTraceDTO> list = new List<ExpressTraceDTO>();
                            foreach (var item in arr)
                            {
                                ExpressTraceDTO model = new ExpressTraceDTO();
                                model.Id = Guid.NewGuid();
                                model.ExpRouteId = orderExpressRoute.Id;
                                model.AcceptTime = Convert.ToDateTime(item["time"].ToString());
                                model.AcceptStation = item["status"].ToString();
                                model.Remark = null;
                                list.Add(model);
                            }
                            expressTraceFacade.SaveExpressTraceList(list);
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
                        result = orderExpressRoutefacade.UpdateExpressRoute(orderExpressRoute);

                    }
                    #endregion
                }
                else
                {
                    //不存在下面两种情况时更新状态
                    if (!orderExpressRoute.Deliverystatus.Contains("已签收") && !orderExpressRoute.Deliverystatus.Contains("派送失败"))
                    {
                        #region 但物流信息存在时
                        json = GetWuliuJson(orderExpressRoute,AppId);
                        obj = JObject.Parse(json);
                        if (obj["status"].ToString() != null && obj["status"].ToString() == "0")
                        {
                            //先删除数据然后添加
                            var res = expressTraceFacade.DelExpressTrace(orderExpressRoute.Id);
                            if (res.isSuccess == true)
                            {
                                JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                                if (arr.Count() > 0)
                                {
                                    List<ExpressTraceDTO> list = new List<ExpressTraceDTO>();
                                    foreach (var item in arr)
                                    {
                                        ExpressTraceDTO model = new ExpressTraceDTO();
                                        model.Id = Guid.NewGuid();
                                        model.ExpRouteId = orderExpressRoute.Id;
                                        model.AcceptTime = Convert.ToDateTime(item["time"].ToString());
                                        model.AcceptStation = item["status"].ToString();
                                        model.Remark = null;
                                        list.Add(model);
                                    }
                                    expressTraceFacade.SaveExpressTraceList(list);
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
                                orderExpressRoutefacade.UpdateExpressRoute(orderExpressRoute);
                            }

                        }
                        #endregion
                    }
                }
                result = new ResultDTO() { ResultCode = 0, Message = "成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("物流快递异常信息:{0}", ex.Message));
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return result;
        }

    }

}
