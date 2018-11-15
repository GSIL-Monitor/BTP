
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/12/6 16:55:07
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
    public partial class CateringSettingBP : BaseBP, ICateringSetting
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            base.Do();
            return this.AddCateringSettingExt(settingDTO);
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            base.Do();
            return this.UpdateCateringSettingExt(settingDTO);
        }

        public Deploy.CustomDTO.FCYSettingCDTO GetCateringSetting(Guid storeId)
        {
            base.Do();
            return this.GetCateringSettingExt(storeId);
        }

        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetCateringSettingByAppIdExt(appId);
        }


        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Deploy.CustomDTO.FCYSettingCDTO> GetCateringSettingByStoreIds(List<Guid> storeIds)
        {
            base.Do();
            return this.GetCateringSettingByStoreIdsExt(storeIds);
        }
    }
}