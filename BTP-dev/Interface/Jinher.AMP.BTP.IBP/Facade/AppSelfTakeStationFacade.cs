
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/9/12 14:48:01
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
    public class AppSelfTakeStationFacade : BaseFacade<IAppSelfTakeStation>
    {

        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            base.Do();
            return this.Command.SaveAppSelfTakeStation(model);
        }
        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            base.Do();
            return this.Command.UpdateAppSelfTakeStation(model);
        }
        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStations(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.DeleteAppSelfTakeStations(ids);
        }
        /// <summary>
        /// 查询自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchResultSDTO GetAppSelfTakeStationList(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchSDTO search)
        {
            base.Do();
            return this.Command.GetAppSelfTakeStationList(search);
        }
        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO GetAppSelfTakeStationById(System.Guid id)
        {
            base.Do();
            return this.Command.GetAppSelfTakeStationById(id);
        }
        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckUserIdExists(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.CheckUserIdExists(userId, appId);
        }
    }
}