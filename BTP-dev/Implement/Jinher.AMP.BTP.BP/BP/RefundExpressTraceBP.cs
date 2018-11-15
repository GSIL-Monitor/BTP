
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/5/22 13:10:24
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
    public partial class RefundExpressTraceBP : BaseBP, IRefundExpressTrace
    {

        /// <summary>
        /// 更新退货物流跟踪数据(物流相关时间)
        /// </summary>
        /// <param name="retd"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpressTrace(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd, System.Guid appId)
        {
            base.Do();
            return this.UpdateRefundExpressTraceExt(retd, appId);
        }
        /// <summary>
        /// 新增退货物流跟踪数据(商家确认退款时间)
        /// </summary>
        /// <param name="retd"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundExpressTrace(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            base.Do();
            return this.AddRefundExpressTraceExt(retd);
        }
    }
}