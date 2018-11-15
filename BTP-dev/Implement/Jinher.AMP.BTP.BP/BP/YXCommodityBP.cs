
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/5/19 12:55:50
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
    public partial class YXCommodityBP : BaseBP, IYXCommodity
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYXCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do(false);
            return this.AddYXCommodityExt(input);
        }
        /// <summary>
        /// 导入严选商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYXCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do(false);
            return this.ImportYXCommodityDataExt(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步严选商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYXCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do(false);
            return this.AutoSyncYXCommodityInfoExt(AppId, Ids);
        }
    }
}