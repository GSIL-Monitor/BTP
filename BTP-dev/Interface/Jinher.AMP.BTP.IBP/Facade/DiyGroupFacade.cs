
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/5/14 16:43:17
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class DiyGroupFacade : BaseFacade<IDiyGroup>
    {

        /// <summary>
        /// 获取拼团信息（必传参数AppId、PageIndex、PageSize、State，可选参数ComNameSub）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> GetDiyGroups(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.Command.GetDiyGroups(search);
        }
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmDiyGroup(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.Command.ConfirmDiyGroup(search);
        }
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Refund(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.Command.Refund(search);
        }

        /// <summary>
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public ResultDTO<List<UnfinishedDiyGroupOutputDTO>> UnfinishedDiyGrouplist(UnfinishedDiyGroupInputDTO inputDTO)
        {
            base.Do();
            return this.Command.UnfinishedDiyGrouplist(inputDTO);
        }
    }
}