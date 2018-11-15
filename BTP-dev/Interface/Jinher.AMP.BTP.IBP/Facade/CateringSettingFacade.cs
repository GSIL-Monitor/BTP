
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/12/6 16:55:06
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
    public class CateringSettingFacade : BaseFacade<ICateringSetting>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            base.Do();
            return this.Command.AddCateringSetting(settingDTO);
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            base.Do();
            return this.Command.UpdateCateringSetting(settingDTO);
        }

        public Deploy.CustomDTO.FCYSettingCDTO GetCateringSetting(Guid storeId)
        {
            base.Do();
            return this.Command.GetCateringSetting(storeId);
        }


        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetCateringSettingByAppId(appId);
        }


        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Deploy.CustomDTO.FCYSettingCDTO> GetCateringSettingByStoreIds(List<Guid> storeIds)
        {
            base.Do();
            return this.Command.GetCateringSettingByStoreIds(storeIds);
        }
    }
}