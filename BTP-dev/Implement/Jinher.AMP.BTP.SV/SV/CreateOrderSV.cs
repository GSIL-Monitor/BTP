
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/10/28 18:04:21
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
    public partial class CreateOrderSV : BaseSv, ICreateOrder
    {

        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderNeedDTO> GetCreateOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderSearchDTO search)
        {
            base.Do();
            return this.GetCreateOrderInfoExt(search);

        }
    }
}