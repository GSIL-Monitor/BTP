
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/9/6 16:04:44
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
    public partial class YPKCommodityBP : BaseBP, IYPKCommodity
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYPKCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do();
            return this.AddYPKCommodityExt(input);
        }
        /// <summary>
        /// 导入易派客商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYPKCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do();
            return this.ImportYPKCommodityDataExt(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYPKCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.AutoSyncYPKCommodityInfoExt(AppId, Ids);
        }
        /// <summary>
        /// 全量同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoYPKSyncCommodity(System.Guid AppId)
        {
            base.Do();
            return this.AutoYPKSyncCommodityExt(AppId);
        }
    }
}