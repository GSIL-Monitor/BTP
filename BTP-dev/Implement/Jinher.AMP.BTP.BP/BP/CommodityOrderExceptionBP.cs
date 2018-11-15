
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/7/28 11:43:16
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
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderExceptionBP : BaseBP, ICommodityOrderException
    {

        /// <summary>
        ///  按条件获取订单异常日志
        ///  </summary>
        /// <param name="dto">参数实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> GetOrderExceptionByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderExceptionParamDTO dto)
        {
            base.Do();
            return this.GetOrderExceptionByAppIdExt(dto);
        }
        /// <summary>
        ///  更新订单异常日志状态
        ///  </summary>
        /// <param name="dto">参数实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderException(Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO dto)
        {
            base.Do();
            return this.UpdateOrderExceptionExt(dto);
        }
    }
}