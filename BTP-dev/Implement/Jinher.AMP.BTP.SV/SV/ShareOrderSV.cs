using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ShareOrderSV : BaseSv, IShareOrder
    {
        /// <summary>
        /// 获取众销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO GetShareOrderMoneySumInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search)
        {
            base.Do(false);
            return this.GetShareOrderMoneySumInfoExt(search);
        }

        /// <summary>
        /// 获取众销入账信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO GetShareOrderMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search)
        {
            base.Do(false);
            return this.GetShareOrderMoneyInfoExt(search);
        }
    }
}
