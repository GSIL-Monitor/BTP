
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/5/22 13:11:10
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class RefundExpressTraceFacade : BaseFacade<IRefundExpressTrace>
    {

        /// <summary>
        /// 更新退货物流跟踪数据（物流公司及物流单号）
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpress(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            base.Do();
            return this.Command.UpdateRefundExpress(retd);
        }
        /// <summary>
        /// 获取退货物流跟踪列表数据
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO>> GetRefundExpressTrace()
        {
            base.Do();
            return this.Command.GetRefundExpressTrace();
        }
    }
}