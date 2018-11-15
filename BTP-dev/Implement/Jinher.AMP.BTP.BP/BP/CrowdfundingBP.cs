
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/12/29 15:18:43
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CrowdfundingBP : BaseBP, ICrowdfunding
    {

        /// <summary>
        /// 添加众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        public ResultDTO AddCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {

            base.Do();
            return this.AddCrowdfundingExt(crowdfundingDTO);

        }
        /// <summary>
        /// 更新众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        public ResultDTO UpdateCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            base.Do();
            return this.UpdateCrowdfundingExt(crowdfundingDTO);
        }
        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="id">众筹Id</param>
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfunding(System.Guid id)
        {
            base.Do();
            return this.GetCrowdfundingExt(id);
        }
        /// <summary>
        /// 获取众筹列表
        /// </summary>
        /// <param name="appName">app名称</param>
        /// <param name="cfState">众筹状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public GetCrowdfundingsDTO GetCrowdfundings(string appName, int cfState, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetCrowdfundingsExt(appName, cfState, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取众筹股东列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="userCode">用户账号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public GetUserCrowdfundingsDTO GetUserCrowdfundings(System.Guid crowdfundingId, string userName, string userCode, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetUserCrowdfundingsExt(crowdfundingId, userName, userCode, pageIndex, pageSize);
        }
        /// <summary>
        /// 众筹股东订单列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public CommodityOrderVMDTO GetUserCrowdfundingOrders(System.Guid crowdfundingId, System.Guid userId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetUserCrowdfundingOrdersExt(crowdfundingId, userId, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据appId找appName
        /// </summary>
        /// <returns></returns>
        public AppNameDTO GetAppNameByAppId(Guid appId)
        {
            base.Do();
            return this.GetAppNameByAppIdExt(appId);
        }
    }
}