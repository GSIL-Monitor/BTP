
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/10/12 11:09:20
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
    public partial class OrderRefundBP : BaseBP, IOrderRefund
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundCompareDTO>> GetOrderRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundSearchDTO search)
        {
            base.Do(false);
            return this.GetOrderRefundExt(search);
        }
    }
}