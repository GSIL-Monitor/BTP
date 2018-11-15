
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/8 15:20:00
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class PaymentBP : BaseBP, IPayment
    {

        /// <summary>
        /// 获取所有支付方式
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<PaymentsForEditDTO> GetAllPaymentExt(Guid appId)
        {
            List<AllPayment> allPaymentDTO = AllPayment.ObjectSet().OrderBy(p => p.SubTime).ToList();
            var onusePayments = Payments.ObjectSet().Where(p => p.AppId == appId).ToDictionary(p => p.PaymentId, p => p);

            //onusePayments 如果没有数据表示是首次查询加载
            bool isInitLoad = !(onusePayments.Count > 0);

            List<PaymentsForEditDTO> paymentsForEditDTOList = new List<PaymentsForEditDTO>();
            foreach (var payment in allPaymentDTO)
            {
                PaymentsForEditDTO paymentsForEditDTO = new PaymentsForEditDTO();
                paymentsForEditDTO.Id = payment.Id;
                paymentsForEditDTO.PaymentName = payment.PaymentName;
                if (onusePayments.ContainsKey(payment.Id))
                {
                    var appPayment = onusePayments[payment.Id];
                    if (appPayment.PaymentName == "支付宝")
                    {
                        paymentsForEditDTO.AliPayPartnerId = appPayment.AliPayPartnerId;
                        paymentsForEditDTO.AliPaySeller = appPayment.AliPaySeller;
                        paymentsForEditDTO.AliPayPrivateKey = appPayment.AliPayPrivateKey;
                        paymentsForEditDTO.AliPayPublicKey = appPayment.AliPayPublicKey;
                        paymentsForEditDTO.AliPayVerifyCode = appPayment.AliPayVerifyCode;
                    }
                    paymentsForEditDTO.IsOnuse = appPayment.IsOnuse;
                }
                paymentsForEditDTO.isInitLoad = isInitLoad;
                paymentsForEditDTOList.Add(paymentsForEditDTO);
            }

            return paymentsForEditDTOList;

        }

        /// <summary>
        /// 修改用户支付方式
        /// </summary>
        /// <param name="paymentsDTO">支付方式实体</param>
        /// <returns></returns>
        public ResultDTO UpdatePaymentExt(PaymentsVM paymentsVM)
        {
            if (paymentsVM == null || paymentsVM.PaymentIds == null || !paymentsVM.PaymentIds.Any())
                return new ResultDTO { ResultCode = 1, Message = "支付方式不能为空" };
            if (paymentsVM.AppScoreSetting == null)
                paymentsVM.AppScoreSetting = new AppScoreSettingDTO();
            //app是否有积分功能项
            var isMyIntegral = BACBP.CheckMyIntegral(paymentsVM.AppId);
            if (isMyIntegral && paymentsVM.AppScoreSetting.IsCashForScore && CustomConfig.ScoreCostList.All(c => c != paymentsVM.AppScoreSetting.ScoreCost))
            {
                return new ResultDTO { ResultCode = 2, Message = "积分设置不合法" };
            }
            Payments p = new Payments();
            List<Payments> payments = Payments.ObjectSet().Where(n => n.AppId == paymentsVM.AppId).ToList();

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            try
            {
                foreach (Payments item in payments)
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(item);
                }

                if (paymentsVM.PaymentIds != null && paymentsVM.PaymentIds.Count() > 0)
                {
                    foreach (var item in paymentsVM.PaymentIds)
                    {
                        Guid payId;
                        if (!Guid.TryParse(item, out payId))
                        {
                            continue;
                        }
                        string name = AllPayment.ObjectSet().Where(n => n.Id == payId).Select(n => n.PaymentName).FirstOrDefault();
                        DateTime dt = DateTime.Now;
                        PaymentsDTO paymentsDTO = new PaymentsDTO();
                        paymentsDTO.Id = Guid.NewGuid();
                        paymentsDTO.Name = name;
                        paymentsDTO.AppId = paymentsVM.AppId;
                        paymentsDTO.Code = dt.ToFileTime().ToString();
                        paymentsDTO.SubTime = DateTime.Now;
                        paymentsDTO.SubId = paymentsVM.AppId;
                        paymentsDTO.PaymentId = new Guid(item);
                        paymentsDTO.IsOnuse = paymentsVM.IsOnuse;
                        paymentsDTO.PaymentName = name;

                        if (name == "支付宝")
                        {
                            paymentsDTO.AliPayPartnerId = paymentsVM.AliPayPartnerId;
                            paymentsDTO.AliPayPrivateKey = paymentsVM.AliPayPrivateKey;
                            paymentsDTO.AliPayPublicKey = paymentsVM.AliPayPublicKey;
                            paymentsDTO.AliPaySeller = paymentsVM.AliPaySeller;
                            paymentsDTO.AliPayVerifyCode = paymentsVM.AliPayVerifyCode;
                        }

                        paymentsDTO.EntityState = System.Data.EntityState.Added;
                        Payments payment = new Payments().FromEntityData(paymentsDTO);
                        contextSession.SaveObject(payment);
                    }
                }
                //保存积分抵现设置
                if (isMyIntegral)
                {
                    var appExt = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == paymentsVM.AppId);
                    if (appExt != null)
                    {
                        appExt.IsCashForScore = paymentsVM.AppScoreSetting.IsCashForScore;
                        appExt.ModifiedOn = DateTime.Now;
                        appExt.EntityState = EntityState.Modified;

                    }
                    else
                    {
                        appExt = AppExtension.CreateAppExtension();
                        appExt.Id = paymentsVM.AppId;
                        appExt.IsCashForScore = paymentsVM.AppScoreSetting.IsCashForScore;
                        contextSession.SaveObject(appExt);
                    }
                    if (paymentsVM.AppScoreSetting.IsCashForScore)
                    {
                        //差异判断，设置不变，不重新保存
                        var lastScoreSetting = ScoreSetting.ObjectSet().Where(c => c.AppId == paymentsVM.AppId).OrderByDescending(c => c.SubTime).FirstOrDefault();
                        if (lastScoreSetting == null || lastScoreSetting.ScoreCost != paymentsVM.AppScoreSetting.ScoreCost)
                        {
                            ScoreSetting scoreSetting = ScoreSetting.CreateScoreSetting();
                            scoreSetting.ScoreCost = paymentsVM.AppScoreSetting.ScoreCost;
                            scoreSetting.AppId = paymentsVM.AppId;
                            contextSession.SaveObject(scoreSetting);
                        }

                    }
                }
                contextSession.SaveChange();

                //删除缓存
                Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_PaymentInfo", paymentsVM.AppId.ToString(), "BTPCache");
                Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_AliPayInfo", paymentsVM.AppId.ToString(), "BTPCache");

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改用户支付方式服务异常。paymentsVM：{0}", JsonHelper.JsonSerializer(paymentsVM)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 是否可以取消积分 (平台启用了分销并且设置了值，或启用了众销且设置了值，就不能取消。)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool IsEnableCancelScoreExt(Guid appId)
        {
            ////app是否有积分功能项
            //var isMyIntegral = BACBP.CheckMyIntegral(appId);
            //if (!isMyIntegral)
            //{
            //    return true;
            //}

            //获取全局配置
            var appExtention = AppExtension.ObjectSet().Where(t => t.Id == appId).FirstOrDefault();

            if (appExtention == null)
            {
                return true;
            }

            //是否app启用分成推广,即众销
            var isEnableShare = BACBP.CheckSharePromotion(appId);
            if (isEnableShare)
            {
                //如果启用，并且有值，就返回false，即不能修改
                //全局
                if (appExtention.IsDividendAll == true && appExtention.SharePercent >0)
                {
                    return false;
                }

               //每个商品
               if (appExtention.IsDividendAll == false)
               {
                   var sharePercent = Commodity.ObjectSet().Where(t => t.AppId == appId).Select(t => t.SharePercent).ToList().Sum();
                   if (sharePercent > 0)
                   {
                       return false;
                   } 
               }
            }

            //是否启用三级分销功能
            var isEnableDistribute = BACBP.CheckAppDistribute(appId);
            if (isEnableDistribute)
            {
                //如果启用，并且有值，就返回false，即不能修改
                //全局
                if (appExtention.DistributeL1Percent > 0 || appExtention.DistributeL2Percent > 0 ||
                    appExtention.DistributeL3Percent > 0)
                {
                    return false;
                }
                //每个商品
                var list = (from c in Commodity.ObjectSet()
                            join cd in CommodityDistribution.ObjectSet() on c.Id equals cd.Id
                            where c.AppId == appId
                            select cd).ToList();
                if (list.Count > 0)
                {
                    var count = list.Select(t => t.L1Percent + t.L2Percent + t.L3Percent).ToList().Sum();
                    if (count > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}