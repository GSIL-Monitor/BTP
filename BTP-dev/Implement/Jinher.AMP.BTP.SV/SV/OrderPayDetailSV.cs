
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/9 15:11:30
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 获取已使用优惠卷信息
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class OrderPayDetailSV : BaseSv, IOrderPayDetail
    {
        /// <summary>
        /// 查询OrderPayDetail信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> GetOrderPayDetailList(Guid objectid)
        {
            base.Do();
            return this.GetOrderPayDetailListExt(objectid);
        }
       
    }
}