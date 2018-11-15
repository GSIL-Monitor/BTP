
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/5/22 13:11:11
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class RefundExpressTraceSV : BaseSv, IRefundExpressTrace
    {

        /// <summary>
        /// 更新退货物流跟踪数据（物流公司及物流单号）
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpress(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            base.Do();
            return this.UpdateRefundExpressExt(retd);

        }
        /// <summary>
        /// 获取退货物流跟踪列表数据
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO>> GetRefundExpressTrace()
        {
            base.Do();
            return this.GetRefundExpressTraceExt();

        }
    }
}