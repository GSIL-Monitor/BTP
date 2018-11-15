using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.AMP.FSP.ISV.Facade;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */


    public class FSPSV : OutSideServiceBase<FSPSVFacade>
    {
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="tradeType">交易类型</param>
        /// <returns></returns>
        public static ReturnInfoDTO CancelPay(CancelPayDTO dto, BTP.Deploy.Enum.TradeTypeEnum tradeType)
        {
            switch (tradeType)
            {
                case TradeTypeEnum.Direct:
                    return Instance.DirectCancelPay(dto);
                case TradeTypeEnum.SecTrans:
                    return Instance.GoldCancelPay(dto);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取应用的交易方式设置
        /// </summary>
        /// <param name="appId">应用id</param>
        ///<returns>0:担保交易;1：非担保交易（直接到账）</returns>
        public static int GetTradeSettingInfo(Guid appId)
        {
            var settings = Instance.GetTradeSettingInfoFsp(appId);
            return settings.TradeType;
        }
    }

    public class FSPSVFacade : OutSideFacadeBase
    {/// <summary>
        /// 获取指定所有者的金币账户余额。
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ulong GetBalance(Guid? ownerId)
        {
            ulong result = 0;
            try
            {
                GoldAccountFacade goldAccountFacade = new GoldAccountFacade();
                goldAccountFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                goldAccountFacade.ContextDTO.LoginOrg = Guid.Empty;
                var returnInfo = goldAccountFacade.GetBalance(ownerId);
                if (returnInfo != null)
                    result = returnInfo.Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.GetBalance服务异常:获取应用信息异常。 ownerId：{0}", ownerId), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取指定所有者的金币账户是否设置了交易密码
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO HasPassword(Guid? ownerId)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldAccountFacade goldAccountFacade = new GoldAccountFacade();
                goldAccountFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldAccountFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldAccountFacade.HasPassword(ownerId);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.HasPassword服务异常:获取应用信息异常。 ownerId：{0}", ownerId), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO ChangePassword(string oldPwd, string newPwd, Guid? userId)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldAccountFacade goldAccountFacade = new GoldAccountFacade();
                goldAccountFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldAccountFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldAccountFacade.ChangePassword(oldPwd, newPwd, userId);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.ChangePassword服务异常:获取应用信息异常。 userId：{0}", userId), ex);
            }
            return returnInfoDTO;
        }

        /// <summary>
        /// 解冻金币
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <returns></returns>
        public ReturnInfoDTO UnFreezeGold(Guid orderId)
        {
            FSP.Deploy.CustomDTO.UnFreezeGoldDTO unFreezeGoldDTO = new FSP.Deploy.CustomDTO.UnFreezeGoldDTO()
            {
                BizId = orderId,
                Sign = CustomConfig.PaySing
            };
            Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO fspResult = FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO, "取消订单");
            LogHelper.Debug(string.Format("UnFreezeGold，订单:{0},结果：{1}", orderId, JsonConvert.SerializeObject(fspResult)));
            return fspResult;
        }
        /// <summary>
        /// 解冻金币
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO UnFreezeGold(UnFreezeGoldDTO dto, string src = "")
        {
            ReturnInfoDTO returnInfoDTO = null;
            GoldPayFacade goldPayFacade = new GoldPayFacade();
            try
            {
                goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.UnFreezeGold(dto);
                if (returnInfoDTO == null || returnInfoDTO.Code != 0)
                {
                    LogHelper.Error(string.Format("{0} 解冻金币错误,unFreezeGoldResult={1}", src, returnInfoDTO));
                    //return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0} FSPSV.UnFreezeGold服务异常:获取应用信息异常。 dto：{1}，ContextDTO：{2}", src, JsonHelper.JsonSerializer(dto), JsonHelper.JsonSerializer(goldPayFacade.ContextDTO)), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 担保交易退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO GoldCancelPay(CancelPayDTO dto)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.CancelPay(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.CancelPay服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 直接到账退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO DirectCancelPay(CancelPayDTO dto)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {

                DirectPayFacade facade = new DirectPayFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                facade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = facade.CancelPay(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.DirectCancelPay服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;
        }

        /// <summary>
        /// 金币支付（批量）
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="contextDTO">上下文，UI层调用必传</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<long> PayByPayeeIdBatch(PayOrderGoldDTO dto, ContextDTO contextDTO = null)
        {
            ReturnInfoDTO<long> returnInfoDTO = null;
            contextDTO = contextDTO ?? AuthorizeHelper.CoinInitAuthorizeInfo();
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = contextDTO;
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                goldPayFacade.Do();
                returnInfoDTO = goldPayFacade.PayByPayeeIdBatch(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.PayByPayeeIdBatch服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;
        }


        /// <summary>
        /// 按收款人Id支付
        /// </summary>
        /// <param name="outTradeId">外部交易单号</param>
        /// <param name="payeeId">收款人用户ID</param>
        /// <param name="gold">交易金币数</param>
        /// <param name="payorComment">付款方备注信息</param>
        /// <param name="payeeComment">收款方备注信息</param>
        /// <param name="password">交易密码</param>
        /// <param name="appId">调用方AppId</param>
        /// <param name="notifyUrl">交易成功通知URL</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<long> PayByPayeeId(Guid outTradeId, Guid payeeId, ulong gold, string payorComment, string payeeComment, string password, Guid appId, string notifyUrl)
        {
            ReturnInfoDTO<long> returnInfoDTO = null;
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.PayByPayeeId(outTradeId, payeeId, gold, payorComment, payeeComment, password, appId, notifyUrl);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.PayByPayeeId服务异常:获取应用信息异常。 outTradeId：{0},payeeId:{1},gold{2},payorComment{3},payeeComment{4},password{5},appId{6},notifyUrl{7}", outTradeId, payeeId, gold, payorComment, payeeComment, password, appId, notifyUrl), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 检查交易密码是否正确
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<string> CheckPassword(string password, ContextDTO contextDTO)
        {
            ReturnInfoDTO<string> returnInfoDTO = null;
            try
            {
                PasswordProtectionFacade passwordProtectionFacade = new PasswordProtectionFacade();
                passwordProtectionFacade.ContextDTO = contextDTO;
                passwordProtectionFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = passwordProtectionFacade.CheckPassword(password);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.CheckPassword服务异常:获取应用信息异常。 password：{0}", password), ex);
            }
            return returnInfoDTO;
        }

        /// <summary>
        /// fsp支付设置
        /// </summary>
        /// <param name="appId">应用id</param>
        ///<returns></returns>
        [BTPAopLogMethod]
        public FspTradeSettingDTO GetTradeSettingInfoFsp(Guid appId)
        {
            if (YXSV.IsProduction && appId == CustomConfig.YJAppId) return new FspTradeSettingDTO { TradeType = 1, IsSetWeixinPay = true };
            FspTradeSettingDTO result;
            try
            {
                GoldSafeFacade gsFacade = new GoldSafeFacade();
                gsFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                gsFacade.ContextDTO.LoginOrg = Guid.Empty;
                Dictionary<string, string> settings = gsFacade.GetTradeSettingInfo(appId);

                result = new FspTradeSettingDTO(settings);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.GetTradeSettingInfoFsp获取应用的交易方式设置异常。异常信息：{0}", ex));
                result = new FspTradeSettingDTO();
            }
            return result;
        }
        ///  <summary>
        ///  保存BTP合并支付订单信息
        ///  </summary>
        /// <param name="preDto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO<List<ChildTransactionStatusDTO>> PreDirectPayBatch(PrePayBatchDTO preDto)
        {
            ReturnInfoDTO<List<ChildTransactionStatusDTO>> result = new ReturnInfoDTO<List<ChildTransactionStatusDTO>>();
            try
            {
                DirectPayFacade dirpayFacade = new Jinher.AMP.FSP.ISV.Facade.DirectPayFacade();
                dirpayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                dirpayFacade.ContextDTO.LoginOrg = Guid.Empty;
                result = dirpayFacade.PreDirectPayBatch(preDto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.PreDirectPayBatch保存BTP合并支付订单信息异常。异常信息：{0}", ex));
            }
            return result;
        }
        /// <summary>
        /// 批量支付预处理
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<List<ChildTransactionStatusDTO>> PrePayBatch(PrePayBatchDTO dto)
        {
            ReturnInfoDTO<List<ChildTransactionStatusDTO>> result = new ReturnInfoDTO<List<ChildTransactionStatusDTO>>();
            try
            {
                Jinher.AMP.FSP.ISV.Facade.GoldPayFacade facadeGoldPay = new FSP.ISV.Facade.GoldPayFacade();
                facadeGoldPay.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                facadeGoldPay.ContextDTO.LoginOrg = Guid.Empty;
                result = facadeGoldPay.PrePayBatch(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.PrePayBatch批量支付预处理信息异常。异常信息：{0}", ex));
            }
            return result;
        }
        /// <summary>
        /// 确认付款(解冻商家和收款人收到的金币和代金券)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO ConfirmPayUnFreeze(ConfirmPayDTO dto)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.ConfirmPayUnFreeze(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.ConfirmPayUnFreeze服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 确认付款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO ConfirmPay(ConfirmPayDTO dto)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.ConfirmPay(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.ConfirmPay服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 确认付款(冻结商家和收款人收到的金币和代金券)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO ConfirmPayFreeze(ConfirmPayDTO dto)
        {
            ReturnInfoDTO returnInfoDTO = null;
            try
            {
                GoldPayFacade goldPayFacade = new GoldPayFacade();
                goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
                returnInfoDTO = goldPayFacade.ConfirmPayFreeze(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FSPSV.ConfirmPayFreeze服务异常:获取应用信息异常。 dto：{0}", dto), ex);
            }
            return returnInfoDTO;

        }
    }

}
