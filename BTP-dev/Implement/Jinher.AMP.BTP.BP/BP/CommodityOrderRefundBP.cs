
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/28 19:39:20
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
    public partial class CommodityOrderRefundBP : BaseBP, ICommodityOrderRefund
    {

        /// <summary>
        /// 添加回款记录
        /// </summary>
        /// <param name="model">回款实体模型</param>
        public bool Insert(Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO model)
        {
            base.Do(false);
           return  this.InsertExt(model);
        }
        /// <summary>
        /// 根据CommodityOrderId 获取回款记录
        /// </summary>
        /// <param name="CommodityOrderId"></param>
        /// <returns>回款记录列表</returns>
        public List<CommodityOrderRefundDTO> GetListByCommodityOrderId(Guid CommodityOrderId)
        {
            base.Do(false);
            return this.GetListByCommodityOrderIdExt(CommodityOrderId);
        }
        /// <summary>
        /// 根据类型，时间获取回款记录
        /// </summary>
        /// <param name="RefundType">类型0电汇1支票2内部挂帐</param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public List<CommodityOrderRefundDTO> GetListByOther(Jinher.AMP.BTP.Deploy.Enum.RefundTypeEnum RefundType, System.DateTime StartTime, System.DateTime EndTime)
        {
            base.Do();
            return this.GetListByOtherExt(RefundType, StartTime, EndTime);
        }
    }
}