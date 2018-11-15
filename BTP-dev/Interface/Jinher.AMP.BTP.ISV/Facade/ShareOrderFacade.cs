using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ShareOrderFacade : BaseFacade<IShareOrder>
    {
        /// <summary>
        /// 获取众销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO GetShareOrderMoneySumInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search)
        {
            base.Do();
            return this.Command.GetShareOrderMoneySumInfo(search);
        }

        /// <summary>
        /// 获取众销入账信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO GetShareOrderMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search)
        {
            base.Do();
            return this.Command.GetShareOrderMoneyInfo(search);
        }
    }
}
