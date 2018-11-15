
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:06
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
    public partial class OrderFieldBP : BaseBP, IOrderField
    {

        /// <summary>
        /// 获取订单设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.OrderFieldDTO GetOrderSet(Guid AppId)
        {
            base.Do();
            return this.GetOrderSetExt(AppId);
        }
        
    }
}