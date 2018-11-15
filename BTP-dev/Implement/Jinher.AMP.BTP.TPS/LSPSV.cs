using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class LSPSV : OutSideServiceBase<LSPSVFacade>
    {

    }

    public class LSPSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 订单状态同步lsp.
        /// </summary>
        /// <param name="commodityOrderId">电商订单id</param>
        /// <param name="stateTo">目标状态</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool SynchronizeOrderFromBTP(Guid commodityOrderId, int stateTo, Guid serviceId, Jinher.JAP.BF.BE.Deploy.Base.ContextDTO contextDTO)
        {
            try
            {

                Jinher.AMP.LSP.Deploy.CustomDTO.Order.SynchronizeOrderFromBTPDTO arg = new LSP.Deploy.CustomDTO.Order.SynchronizeOrderFromBTPDTO();
                arg.OrderId = commodityOrderId;
                arg.OrderStatus = stateTo;
                arg.TempOrderId = serviceId;

                LSP.ISV.Facade.OrderFacade lspOrderFacade = new LSP.ISV.Facade.OrderFacade();
                lspOrderFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.LSP.Deploy.CustomDTO.ReturnInfoDTO<int> lspResult = lspOrderFacade.SynchronizeOrderFromBTP(arg);
                return lspResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("LSPSV.SynchronizeOrderFromBTP异常，异常信息：", ex);
            }
            return false;
        }
    }

}
