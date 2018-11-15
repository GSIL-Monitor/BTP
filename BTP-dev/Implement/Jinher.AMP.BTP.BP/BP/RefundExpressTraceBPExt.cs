
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/5/10 13:53:01
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class RefundExpressTraceBP : BaseBP, IRefundExpressTrace
    {

        /// <summary>
        /// 更新退货物流跟踪数据
        /// </summary>
        /// <param name="retd"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpressTraceExt(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd, Guid appId)
        {
            try
            {
                var rets = RefundExpressTrace.ObjectSet().Where(t => t.OrderId == retd.OrderId);
                if (rets == null)
                {

                    LogHelper.Error(string.Format("更新退货物流跟踪数据不存在。UpdateRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "更新数据不存在" };
                }
                RefundExpressTrace ret = null;
                //整单退货
                if (retd.OrderItemId == null || retd.OrderItemId == Guid.Empty)
                {
                    ret = rets.FirstOrDefault();
                }
                //单商品退货
                else
                {
                    ret = rets.FirstOrDefault(r => r.OrderItemId == retd.OrderItemId);
                }
                if (ret == null)
                {

                    LogHelper.Error(string.Format("更新退货物流跟踪数据不存在。UpdateRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "更新数据不存在" };
                }

                //查询物流信息
                var expressInfo = GetWuliuJson(ret.RefundExpCo, ret.RefundExpOrderNo, appId);
                JObject obj = JObject.Parse(expressInfo);
                if (obj["status"].ToString() == null || obj["status"].ToString() != "0")
                {
                    LogHelper.Error(string.Format("更新退货物流跟踪数据时未查到物流信息。UpdateRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "更新退货物流跟踪数据时未查到物流信息" };

                }

                JArray arr = (JArray)JsonConvert.DeserializeObject(obj["result"]["list"].ToString());
                ret.ExpressDeliveryTime = arr.Last() != null ? Convert.ToDateTime(arr.Last()["time"]) : default(DateTime?); //快递收揽时间
                int deliverystatus = Convert.ToInt32(obj["result"]["deliverystatus"].ToString());      //快递状态
                ret.ExpressSDTime = deliverystatus != 3 ? default(DateTime?) : (arr.First() != null ? Convert.ToDateTime(arr.First()["time"]) : default(DateTime?));//快递送达时间
                ret.EntityState = System.Data.EntityState.Modified;
                ContextFactory.CurrentThreadContext.SaveObject(ret);
                var result = ContextFactory.CurrentThreadContext.SaveChanges();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, isSuccess = true, Message = "退货物流跟踪数据更新成功" };
                }

                LogHelper.Error(string.Format("更新退货物流跟踪数据失败。UpdateRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)));
                return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "退货物流跟踪数据更新失败" };

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("更新退货物流跟踪数据异常。UpdateRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)), ex);
                return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "退货物流跟踪数据更新失败" };
            }
        }


        /// <summary>
        /// 新增退货物流跟踪数据(商家确认退款时间)
        /// </summary>
        /// <param name="retd"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundExpressTraceExt(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            try
            {

                RefundExpressTrace ret = new RefundExpressTrace
                {
                    OrderId = retd.OrderId,
                    OrderItemId = retd.OrderItemId,
                    AgreeRefundTime = retd.AgreeRefundTime,
                    EntityState = System.Data.EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(ret);
                var result = ContextFactory.CurrentThreadContext.SaveChanges();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, isSuccess = true, Message = "退货物流跟踪数据添加成功(商家确认退款时间)" };
                }
                LogHelper.Error(string.Format("新增退货物流跟踪数据失败(商家确认退款时间)。AddRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)));
                return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "退货物流跟踪数据添加失败" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("新增退货物流跟踪数据异常(商家确认退款时间)。AddRefundExpressTraceExt：{0}", JsonHelper.JsonSerializer(retd)), ex);
                return new ResultDTO { ResultCode = 2, isSuccess = false, Message = "退货物流跟踪数据添加异常" };
            }


        }

        /// <summary>
        /// 根据退货物流单号获取物流信息
        /// </summary>
        /// <param name="refundExpCo">快递公司</param>
        /// <param name="refundExpOrderNo">快递单号</param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        private static string GetWuliuJson(string refundExpCo, string refundExpOrderNo, Guid AppId)
        {
            string sappids = CustomConfig.SappIds;
            List<string> Sappidlist = null;
            if (!string.IsNullOrEmpty(sappids))
            {
                Sappidlist = sappids.Split(new char[] { ',' }).ToList();
            }
            string appkey = CustomConfig.zshappkey;
            string json = null;
            //快递公司简称
            var shipperCode = ExpressCode.AllExpCodes.Where(ec => ec.ExpCompanyName.ToUpper().Contains(refundExpCo)).First();
            try
            {
                string url = null;
                if (Sappidlist.Contains(AppId.ToString().ToUpper()))
                {
                    //苏宁易购
                    url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, shipperCode, refundExpOrderNo);
                }
                else
                {
                    //中石化
                    url = string.Format("http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}", appkey, "auto", refundExpOrderNo);
                }
                json = WebRequestHelper.SendGetRequest(url);
            }
            catch (Exception ex) { }
            return json.ToString();
        }
    }
}