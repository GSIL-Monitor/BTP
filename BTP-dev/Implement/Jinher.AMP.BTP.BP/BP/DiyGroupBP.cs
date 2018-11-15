
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/14 16:43:19
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
    /// 拼团
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DiyGroupBP : BaseBP, IDiyGroup
    {

        /// <summary>
        /// 获取拼团信息（必传参数AppId、PageIndex、PageSize、State，可选参数ComNameSub）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> GetDiyGroups(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.GetDiyGroupsExt(search);
        }
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmDiyGroup(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.ConfirmDiyGroupExt(search);
        }
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Refund(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.RefundExt(search);
        }

        /// <summary>
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.UnfinishedDiyGroupOutputDTO>> UnfinishedDiyGrouplist(Deploy.CustomDTO.UnfinishedDiyGroupInputDTO inputDTO)
        {
            base.Do(false);
            return this.UnfinishedDiyGrouplistExt(inputDTO);
        }
    }
}