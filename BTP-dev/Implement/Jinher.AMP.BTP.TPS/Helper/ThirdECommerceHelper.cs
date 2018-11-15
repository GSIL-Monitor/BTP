using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 第三方电商接入帮助类
    /// </summary>
    public class ThirdECommerceHelper
    {
        /// <summary>
        /// 获取店铺(商家)对应的第三方电商类型
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static ThirdECommerceTypeEnum GetAppThirdECommerceType(Guid appId)
        {
            if (appId == Guid.Empty) return ThirdECommerceTypeEnum.NotThirdECommerce;
            if (CustomConfig.JdAppIdList.Contains(appId)) return ThirdECommerceTypeEnum.JingDongDaKeHu;
            else if (CustomConfig.YxAppIdList.Contains(appId)) return ThirdECommerceTypeEnum.WangYiYanXuan;
            else if (CustomConfig.SnAppIdList.Contains(appId)) return ThirdECommerceTypeEnum.SuNingYiGou;
            else if (CustomConfig.FZAppIdList.Contains(appId)) return ThirdECommerceTypeEnum.FangZheng;
            else if (CustomConfig.YPKAppIdList.Contains(appId)) return ThirdECommerceTypeEnum.YiPaiKe;
            else
            {
                var thirdECommerceId = MallApply.ObjectSet().Where(p => p.EsAppId == CustomConfig.YJAppId
                    && p.AppId == appId && p.ThirdECommerceId.HasValue).Select(p => p.ThirdECommerceId.Value).FirstOrDefault();
                if (thirdECommerceId != Guid.Empty)
                {
                    if (ThirdECommerce.ObjectSet().Any(p => p.Id == thirdECommerceId && p.Type == (int)ThirdECommerceTypeEnum.ByBiaoZhunJieKou))
                        return ThirdECommerceTypeEnum.ByBiaoZhunJieKou;
                }
                return ThirdECommerceTypeEnum.NotThirdECommerce;
            }
        }

        /// <summary>
        /// 店铺(商家)是否属于第三方电商(暂时京东除外)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsThirdECommerce(ThirdECommerceTypeEnum type)
        {
            return type != ThirdECommerceTypeEnum.NotThirdECommerce 
                && type != ThirdECommerceTypeEnum.JingDongDaKeHu
                && type != ThirdECommerceTypeEnum.SuNingYiGou
                && type != ThirdECommerceTypeEnum.FangZheng;
        }

        /// <summary>
        /// 店铺(商家)是否属于第三方电商(暂时京东除外)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsThirdECommerce(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return IsThirdECommerce(type);
        }

        /// <summary>
        /// 店铺(商家)是否属于京东大客户
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsJingDongDaKeHu(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.JingDongDaKeHu;
        }

        /// <summary>
        /// 店铺(商家)是否属于网易严选
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsWangYiYanXuan(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.WangYiYanXuan;
        }

        /// <summary>
        /// 店铺(商家)是否属于苏宁易购
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsSuNingYiGou(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.SuNingYiGou;
        }

        /// <summary>
        /// 店铺(商家)是否属于苏宁易购
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsFangZheng(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.FangZheng;
        }

        /// <summary>
        /// 店铺(商家)是否属于易派客
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsYiPaiKe(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.YiPaiKe;
        }
        /// <summary>
        /// 是否通过标准接口接入的电商
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsByBiaoZhunJieKou(Guid appId)
        {
            var type = GetAppThirdECommerceType(appId);
            return type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou;
        }

        /// <summary>
        /// 店铺(商家)是否混合
        /// </summary>
        /// <param name="appIdList"></param>
        /// <returns></returns>
        public static bool IsThirdMix(List<Guid> appIdList)
        {
            var flag = false;
            var listType = new List<ThirdECommerceTypeEnum>();
            for (int i = 0; i < appIdList.Count; i++)
            {
                var type = GetAppThirdECommerceType(appIdList[i]);
                if (type != ThirdECommerceTypeEnum.NotThirdECommerce && !listType.Contains(type))
                {
                    listType.Add(type);
                    if (listType.Count > 1)
                    {
                        flag = true;
                        break;
                    }
                }
                else continue;
            }
            return flag;
        }

        /// <summary>
        /// 判断商品是否填写备注编码（第三方电商商品skuid）
        /// </summary>
        /// <param name="commodityList"></param>
        /// <param name="commodityStockList"></param>
        /// <returns>未设置备注编码的商品名称集合</returns>
        public static List<string> CheckJDCodeExists(Guid appId, List<CommodityDTO> commodityList, List<BTP.Deploy.CommodityStockDTO> commodityStockList)
        {
            if (commodityList == null && commodityList.Count == 0) return new List<string>();
            var type = GetAppThirdECommerceType(appId);
            if (!IsThirdECommerce(type)) return new List<string>();
            var commodityIdList = new List<Guid>();
            commodityList.ForEach(p =>
            {
                if (commodityStockList != null && commodityStockList.Count > 0 && commodityStockList.Any(x => x.CommodityId == p.Id))
                {
                    commodityIdList.AddRange(commodityStockList.Where(x => x.CommodityId == p.Id && string.IsNullOrEmpty(x.JDCode)).Select(x => x.CommodityId).ToList());
                }
                else
                {
                    if (string.IsNullOrEmpty(p.JDCode))
                    {
                        commodityIdList.Add(p.Id);
                    }
                }
            });
            if (commodityIdList.Count > 0)
            {
                var commodityNameList = new List<string>();
                if (IsWangYiYanXuan(appId))
                {
                    var appName = APPSV.GetAppName(appId);
                    commodityNameList = commodityList.Where(p => commodityIdList.Contains(p.Id)).Select(p => p.Name).ToList();
                    var commodityNames = string.Join("、", commodityNameList);
                    YXOrderHelper.SaveErrorLog(new YXOrderErrorLogDTO
                    {
                        Content = string.Format("{0}App中{1}等商品的备注编码不存在，请尽快补充填写~", appName, commodityNames),
                        AppId = appId,
                        OrderId = Guid.Empty,
                        OrderCode = string.Empty,
                        AppName = appName,
                        CommodityNames = commodityNames,
                        Json = string.Empty
                    });
                }
                return commodityNameList;
            }
            return new List<string>();
        }

        /// <summary>
        /// 第三方电商定时获取未确认收货订单物流轨迹信息
        /// </summary>
        public static void GetOrderExpressForJob()
        {
            YXOrderHelper.GetExpressOrderForJob();
        }

        /// <summary>
        /// 是否第三方电商订单(暂时京东除外)
        /// </summary>
        /// <param name="appId">店铺Id</param>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        public static bool IsThirdECommerceOrder(Guid appId, Guid orderId)
        {
            var type = GetAppThirdECommerceType(appId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan) return YXOrderHelper.IsYXOrder(orderId);
            if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou) return ThirdECommerceOrderHelper.IsThirdECOrder(orderId);
            return false;
        }

        /// <summary>
        /// 获取第三方电商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static ThirdECommerceDTO GetThirdECommerce(Guid appId)
        {
            var thirdECommerceId = MallApply.ObjectSet().Where(p => p.EsAppId == CustomConfig.YJAppId
                    && p.AppId == appId && p.ThirdECommerceId.HasValue).Select(p => p.ThirdECommerceId.Value).FirstOrDefault();
            if (thirdECommerceId != Guid.Empty)
            {
                var thirdECommerce = ThirdECommerce.ObjectSet()
                    .Where(p => p.Id == thirdECommerceId && p.Type == (int)ThirdECommerceTypeEnum.ByBiaoZhunJieKou)
                    .FirstOrDefault();
                if (thirdECommerce != null) return thirdECommerce.ToEntityData();
            }
            return null;
        }

        /// <summary>
        /// 第三方电商授权认证
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="callerId"></param>
        /// <param name="time"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ThirdResponse CheckThirdECommerce(Guid appId, string callerId, string time, string code)
        {
            if (appId == Guid.Empty) return new ThirdResponse { Code = 10002, Msg = "缺少参数appId" };
            if (string.IsNullOrEmpty(callerId)) return new ThirdResponse { Code = 10003, Msg = "缺少参数callerId" };
            if (string.IsNullOrEmpty(time)) return new ThirdResponse { Code = 10004, Msg = "缺少参数time" };
            if (string.IsNullOrEmpty(code)) return new ThirdResponse { Code = 10005, Msg = "缺少参数code" };
            var thirdECommerce = GetThirdECommerce(appId);
            if (thirdECommerce == null) return new ThirdResponse { Code = 10006, Msg = "非法参数appId" };
            if (thirdECommerce.OpenApiCallerId != callerId) return new ThirdResponse { Code = 10008, Msg = "非法参数callerId" };
            var codeCheck = OAPISV.GetCode(thirdECommerce.OpenApiKey, thirdECommerce.OpenApiCallerId, time);
            if (code != codeCheck) return new ThirdResponse { Code = 10009, Msg = "认证失败" };
            return new ThirdResponse { Code = 200, Msg = "ok" };
        }

        /// <summary>
        /// 第三方电商取消订单申请
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static ResultDTO CancelOrder(CommodityOrder order)
        {
            var type = GetAppThirdECommerceType(order.AppId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan && YXOrderHelper.IsYXOrder(order.Id))
            {
                return YXOrderRefundHelper.CancelPaidOrder(order);
            }
            else if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou && ThirdECommerceOrderHelper.IsThirdECOrder(order.Id))
            {
                return ThirdECommerceOrderHelper.CancelOrder(order.Id);
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 第三方电商获取订单项物流
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<ThirdOrderItemExpress> GetOrderItemExpress(Guid appId, Guid orderId, ref bool isThirdECommerceOrder)
        {
            var type = GetAppThirdECommerceType(appId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan)
            {
                isThirdECommerceOrder = true;
                var list = YXOrderHelper.GetYXExpressDetailInfoSkuList(orderId);
                return list.Select(p => new ThirdOrderItemExpress { OrderItemId = p.OrderItemId, ExpressNo = p.ExpressNo, SubExpressNos = p.SubExpressNos }).ToList();
            }
            else if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou)
            {
                isThirdECommerceOrder = true;
                var list = ThirdECommerceOrderHelper.GetThirdECOrderPackageSkuList(orderId);
                return list.Select(p => new ThirdOrderItemExpress { OrderItemId = p.OrderItemId, ExpressNo = p.ExpressNo }).ToList();
            }
            else if (type == ThirdECommerceTypeEnum.SuNingYiGou)
            {
                isThirdECommerceOrder = true;
                return SuningSV.suning_govbus_rejection_getsnpackage(orderId);
            }
            else if (type == ThirdECommerceTypeEnum.FangZheng)
            {
                isThirdECommerceOrder = true;
                return FangZhengSV.FangZheng_Logistics_Package(orderId);
            }
            return new List<ThirdOrderItemExpress>();
        }

        /// <summary>
        /// 第三方电商获取订单项物流轨迹信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public static ThirdOrderPackageExpress GetOrderItemExpressTrace(Guid appId, Guid orderItemId)
        {
            var type = GetAppThirdECommerceType(appId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan)
            {
                var yxexpress = YXOrderHelper.GetExpressInfo(orderItemId);
                if (yxexpress == null) return null;
                var express = new ThirdOrderPackageExpress
                {
                    ExpressCompany = yxexpress.company,
                    ExpressNo = yxexpress.number,
                    ExpressTraceList = new List<ThirdExpressTrace>()
                };
                if (yxexpress.content != null && yxexpress.content.Count > 0)
                {
                    yxexpress.content.ForEach(p =>
                    {
                        express.ExpressTraceList.Add(new ThirdExpressTrace
                        {
                            Desc = p.desc,
                            Time = p.time
                        });
                    });
                }
                return express;
            }
            else if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou)
            {
                return ThirdECommerceOrderHelper.GetExpressInfo(orderItemId);
            }
            return null;
        }

        /// <summary>
        /// 第三方电商发起售后服务申请
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItem"></param>
        /// <param name="refund"></param>
        /// <returns></returns>
        public static ResultDTO CreateService(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            var type = GetAppThirdECommerceType(order.AppId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan)
            {
                return YXOrderRefundHelper.ApplyRefundOrderAfterSales(order, orderItem, refund);
            }
            else if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou)
            {
                return ThirdECommerceServiceHelper.CreateService(order, orderItem, refund);
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 第三方电商取消售后服务申请
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItem"></param>
        /// <param name="refund"></param>
        /// <returns></returns>
        public static ResultDTO CancelService(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            var type = GetAppThirdECommerceType(order.AppId);
            if (type == ThirdECommerceTypeEnum.WangYiYanXuan)
            {
                return YXSV.CancelRefundOrder(refund.ApplyId);
            }
            else if (type == ThirdECommerceTypeEnum.ByBiaoZhunJieKou)
            {
                return ThirdECommerceServiceHelper.CancelService(order.AppId, order.Id, refund.Id);
            }
            return new ResultDTO { isSuccess = true };
        }
    }
}