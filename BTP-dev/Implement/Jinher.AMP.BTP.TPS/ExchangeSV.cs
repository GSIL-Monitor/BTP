using System;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */
    public class ExchangeSV : OutSideServiceBase<ExchangeSVFacade>
    {
        /// <summary>
        /// 获取绑定的汇款充值银行账户。
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static ResultDTO<ChargeAccountDTO> GetChargeAccounts(Guid ownerId)
        {
            return Instance.GetChargeAccounts(ownerId);
        }
    }

    public class ExchangeSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取绑定的汇款充值银行账户。
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<ChargeAccountDTO> GetChargeAccounts(Guid ownerId)
        {
            ResultDTO<ChargeAccountDTO> result;
            try
            {
                Jinher.AMP.FSP.ISV.Facade.ExchangeFacade facade = new FSP.ISV.Facade.ExchangeFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                facade.ContextDTO.LoginOrg = Guid.Empty;
                var chargeInfo = facade.GetChargeAccounts(ownerId);
                if (chargeInfo.Code != 0)
                {
                    result = new ResultDTO<ChargeAccountDTO> { isSuccess = false, Message = chargeInfo.Message };
                }
                else if (chargeInfo.Data.Count == 0)
                {
                    result = new ResultDTO<ChargeAccountDTO> { isSuccess = false, Message = "请商家到“金币管理-充值-网银充值”页面绑定银行账号	" };
                }
                else
                {
                    result = new ResultDTO<ChargeAccountDTO>
                        {
                            isSuccess = true,
                            Data = new ChargeAccountDTO()
                            {
                                AccountName = chargeInfo.Data[0].Item3,
                                BankAccount = chargeInfo.Data[0].Item4,
                                BankName = chargeInfo.Data[0].Item2
                            }
                        };
                }
            }
            catch (Exception ex)
            {
                result = new ResultDTO<ChargeAccountDTO> { isSuccess = false, Message = ex.Message };
                LogHelper.Error(string.Format("ExchangeSV.GetChargeAccounts服务异常:获取应用信息异常。 ownerId：{0}", ownerId), ex);
            }
            return result;
        }
    }
}
