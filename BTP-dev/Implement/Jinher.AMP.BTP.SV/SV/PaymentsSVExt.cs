using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.App.IBP.Facade;
using Jinher.AMP.App.Deploy;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 支付方式接口类
    /// </summary>
    public partial class PaymentsSV : BaseSv, IPayments
    {
        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetPaymentsExt(System.Guid appId)
        {
            try
            {
                //从缓存读取
                var payments = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_PaymentInfo", appId.ToString(), "BTPCache") as List<PaymentsSDTO>;
                //缓存中不存在
                if (payments == null || payments.Count == 0)
                {
                    //从数据库查询
                    payments = Payments.ObjectSet().Where(n => n.AppId == appId).Select(
                                    n => new PaymentsSDTO
                                    {
                                        IsOnUse = n.IsOnuse,
                                        PaymentsName = n.Name
                                    }).ToList();
                    //添加缓存
                    if (payments.Count > 0)
                    {
                        Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_PaymentInfo", appId.ToString(), payments, "BTPCache");
                    }
                }
                //如果支付方式为空，返回默认金币支付
                if (payments.Count == 0)
                {
                    payments.Add(new PaymentsSDTO
                                {
                                    IsOnUse = true,
                                    PaymentsName = "金币"
                                });
                }
                return payments;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取支付方式异常。appId：{0}", appId), ex);
                return null;
            }
        }

        /// <summary>
        /// 获取支付方式 --- 厂家直销
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetSetPaymentsExt()
        {
            try
            {
                //从数据库查询
                var payments = Payments.ObjectSet().Select(
                                n => new PaymentsSDTO
                                {
                                    IsOnUse = n.IsOnuse,
                                    PaymentsName = n.Name
                                }).ToList();

                //如果支付方式为空，返回默认金币支付
                if (payments.Count == 0)
                {
                    payments.Add(new PaymentsSDTO
                    {
                        IsOnUse = true,
                        PaymentsName = "金币"
                    });
                }
                return payments;
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取支付方式异常", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取商家收款ID
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Guid GetPayeeIdExt(System.Guid appId)
        {
            try
            {
                AppManagerFacade appManagerFacade = new AppManagerFacade();
                ApplicationDTO applicationDTO = appManagerFacade.GetAppById(appId);
                if (applicationDTO != null && applicationDTO.OwnerId != Guid.Empty)
                {
                    return applicationDTO.OwnerId ?? Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商家收款Id异常。appId：{0}", appId), ex);
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 获取支付宝信息
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AlipayDTO GetAlipayInfoExt(System.Guid appId)
        {
            //从缓存读取
            var payments = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_AliPayInfo", appId.ToString(), "BTPCache") as AlipayDTO;
            //缓存中不存在
            if (payments == null)
            {
                //从数据库查询
                payments = Payments.ObjectSet().Where(n => n.AppId == appId && n.Name == "支付宝").Select(
                              n => new AlipayDTO
                              {
                                  AliPayPartnerId = n.AliPayPartnerId,
                                  AliPayPrivateKey = n.AliPayPrivateKey,
                                  AliPayPublicKey = n.AliPayPublicKey,
                                  AliPaySeller = n.AliPaySeller,
                                  AliPayVerifyCode = n.AliPayVerifyCode
                              }).FirstOrDefault();

                if (payments != null)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_AliPayInfo", appId.ToString(), payments, "BTPCache");
                }
            }
            return payments;

        }

        /// <summary>
        /// 是不是所有店铺app都支持“货到付款”。
        /// </summary>
        /// <param name="appIds">店铺appId</param>
        /// <returns>是不是所有店铺app都支持“货到付款”</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> IsAllAppSupportCODExt(System.Collections.Generic.List<System.Guid> appIds)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> result = new ResultDTO<bool>();
            result.Data = false;
            try
            {
                if (appIds == null || (!appIds.Any()))
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                //“货到付款” 支付方式Id => 79016C71-4785-4D79-85EB-4AE0096F0969
                var codPaymentId = new Guid("79016C71-4785-4D79-85EB-4AE0096F0969");
                var count = (from p in Payments.ObjectSet()
                             where appIds.Contains(p.AppId.HasValue ? p.AppId.Value : Guid.Empty) 
                             && p.PaymentId == codPaymentId && p.IsOnuse
                             select p.AppId).Distinct().Count();
                if (count == appIds.Count)
                {
                    result.Data = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("IsAllAppSupportCODExt异常，异常信息：", ex);
                result.Message = "服务异常";
                result.ResultCode = -1;
            }
            return result;
        }
    }
}