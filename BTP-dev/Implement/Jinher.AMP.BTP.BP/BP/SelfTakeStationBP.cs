
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/8/7 14:47:16
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 自提点
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SelfTakeStationBP : BaseBP, ISelfTakeStation
    {

        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            base.Do();
            return this.SaveSelfTakeStationExt(selfTakeStationDTO);
        }

        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            base.Do();
            return this.UpdateSelfTakeStationExt(selfTakeStationDTO);
        }

        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStations(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.DeleteSelfTakeStationsExt(ids);
        }

        /// <summary>
        /// 查询自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult> GetAllSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchSDTO selfTakeStationSearch, out int rowCount)
        {
            base.Do();
            return this.GetAllSelfTakeStationExt(selfTakeStationSearch, out rowCount);
        }

        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult GetSelfTakeStationById(System.Guid id)
        {
            base.Do();
            return this.GetSelfTakeStationByIdExt(id);
        }
        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSelfTakeStationManagerByUserId(System.Guid userId)
        {
            base.Do();
            return this.CheckSelfTakeStationManagerByUserIdExt(userId);
        }
        /// <summary>
        /// 删除负责人信息
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStationManagerById(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.DeleteSelfTakeStationManagerByIdExt(ids);
        }
        /// <summary>
        /// 获取所有自提点
        /// </summary>
        /// <param name="AppId">卖家id</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<AppSelfTakeStationResultDTO> GetAllAppSelfTakeStation(Guid AppId, int pageSize, int pageIndex, out int rowCount)
        {
            base.Do(false);
            return this.GetAllAppSelfTakeStationExt(AppId, pageSize, pageIndex,out rowCount);
        }
        /// <summary>
        /// 删除自提点
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStation(Guid id)
        {
            base.Do(false);
            return this.DeleteAppSelfTakeStationExt(id);
        }
        /// <summary>
        /// 根据条件查询所有自提点
        /// </summary>
        /// <param name="AppId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="Name"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> GetAllAppSelfTakeStationByWhere(Guid appId, int pageSize, int pageIndex,out int rowCount, string Name, string province, string city, string district)
        {
            base.Do(false);
            return this.GetAllAppSelfTakeStationByWhereExt(appId, pageSize, pageIndex, out rowCount, Name, province, city, district);
        }
    }
}