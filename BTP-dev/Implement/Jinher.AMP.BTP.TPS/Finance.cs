using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.Finance.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Finance.ISV.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class Finance : OutSideServiceBase<FinanceFacade>
    {

    }

    public class FinanceFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 多收款人分账交易,需要付款人密码
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO MultiPayeeTradeByPassword(MultiPayeeTradeByPasswordArg arg)
        {
            ReturnInfoDTO reT = new ReturnInfoDTO();
            try
            {
                GoldDealerFacade goldDealerFacade = new GoldDealerFacade();
                goldDealerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                goldDealerFacade.ContextDTO.LoginOrg = Guid.Empty;
                reT = goldDealerFacade.MultiPayeeTradeByPassword(arg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Finance.MultiPayeeTradeByPassword服务异常:获取应用信息异常。 arg：{0}", arg), ex);
            }
            return reT;
        }

        /// <summary>
        /// 获取直接到账支付信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<Jinher.AMP.Finance.Deploy.PayTransactionDTO> GetPayTransaction(Guid orderId)
        {
            ReturnInfoDTO<AMP.Finance.Deploy.PayTransactionDTO> reT = new ReturnInfoDTO<AMP.Finance.Deploy.PayTransactionDTO>();
            try
            {
                Jinher.AMP.Finance.ISV.Facade.PayFacade payFacade = new PayFacade();
                payFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                payFacade.ContextDTO.LoginOrg = Guid.Empty;
                reT = payFacade.GetPayTransaction(orderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Finance.GetPayTransaction:获取应用信息异常。 arg：{0}", orderId), ex);
            }
            return reT;
        }

        /// <summary>
        /// 
        /// </summary>
        [BTPAopLogMethod]
        public bool GetIsPay(Guid orderId)
        {
            bool isPay = false;
            try
            {
                var ret = GetPayTransaction(orderId);
                if (ret != null && ret.Data != null)
                {
                    if (!string.IsNullOrEmpty(ret.Data.TradeNum))
                    {
                        isPay = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Finance.GetIsPay:获取应用信息异常。 arg：{0}", orderId), ex);
                isPay = true;
            }
            return isPay;
        }
    }

}
