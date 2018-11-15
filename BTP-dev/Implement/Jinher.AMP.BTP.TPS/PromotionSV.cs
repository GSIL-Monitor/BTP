using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{

    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class PromotionSV : OutSideServiceBase<PromotionSVFacade>
    {

    }

    public class PromotionSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 根据用户ID获取它的代金券条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public int GetUsersVoucherCount(Guid userId)
        {
            int result = 0;
            if (userId == Guid.Empty)
                return result;
            Jinher.AMP.Promotion.ISV.Facade.VoucherFacade facade = new Jinher.AMP.Promotion.ISV.Facade.VoucherFacade();
            facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            try
            {
                result = facade.GetUsersVoucherCount(userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("PromotionSV.GetUsersVoucherCount服务异常。userId:{0} ", userId), ex);
            }
            return result;
        }
    }
}
