
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/11/23 10:08:06
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
    public partial class YJBJCardBP : BaseBP, IYJBJCard
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Create(System.Guid orderId)
        {
            base.Do();
            return this.CreateExt(orderId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJBJCardDTO> Get(System.Guid orderId)
        {
            base.Do();
            return this.GetExt(orderId);
        }
    }
}