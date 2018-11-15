
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/28 19:39:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class CommodityOrderRefundFacade : BaseFacade<ICommodityOrderRefund>
    {

        /// <summary>
        /// 添加回款记录
        /// </summary>
        /// <param name="model">回款实体模型</param>
        public bool Insert(Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO model)
        {
            base.Do();
           return this.Command.Insert(model);
        }
        /// <summary>
        /// 根据CommodityOrderId 获取回款记录
        /// </summary>
        /// <param name="CommodityOrderId"></param>
        /// <returns>回款记录列表</returns>
        public List<CommodityOrderRefundDTO> GetListByCommodityOrderId(Guid CommodityOrderId)
        {
            base.Do();
            return this.Command.GetListByCommodityOrderId(CommodityOrderId);
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
            return this.Command.GetListByOther(RefundType, StartTime, EndTime);
        }
    }
}