
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/11 19:03:35
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CustomSV : BaseSv, ICustom
    {

        /// <summary>
        /// 根据userid获取用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.CBC.Deploy.CustomDTO.UserInfoWithAccountDTO> GetCustomInfo(System.Guid userId)
        {
            base.Do(false);
            return this.GetCustomInfoExt(userId);

        }
        /// <summary>
        /// 根据commodityId获取商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityInfoListDTO> GetCommodityInfo(System.Guid commodityId)
        {
            base.Do(false);
            return this.GetCommodityInfoExt(commodityId);

        }
        /// <summary>
        /// 根据orderId获取订单信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CommodityOrderDTO> GetCommodityOrder(System.Guid orderId)
        {
            base.Do(false);
            return this.GetCommodityOrderExt(orderId);
        }

        /// <summary>
        /// 商家的移动坐席数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSceneUserDTO>> GetAppSceneContent(int pageIndex)
        {
            base.Do(false);
            return this.GetAppSceneContentExt(pageIndex);

        }
        /// <summary>
        /// 获取易捷北京下所有店铺信息
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBJAppInfo>> GetYJAppInfo(int pageIndex)
        {
            base.Do(false);
            return this.GetYJAppInfoExt(pageIndex);
        }

        /// <summary>
        /// 获取消息未读量
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public int GetNoInfoCount(string guid)
        {
            base.Do(false);
            return this.GetNoInfoCountExt(guid);

        }
    }
}