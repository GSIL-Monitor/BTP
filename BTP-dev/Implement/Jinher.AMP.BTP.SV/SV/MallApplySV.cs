
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/11 15:07:53
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
    public partial class MallApplySV : BaseSv, IMallApply
    {

        /// <summary>
        /// 获取易捷北京商家信息
        /// </summary>
        /// <param name="appIds">商家Id列表</param>
        /// <returns>商家信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO> GetYJSellerInfoes(System.Collections.Generic.List<System.Guid> appIds)
        {
            base.Do(false);
            return this.GetYJSellerInfoesExt(appIds);

        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(System.Guid appId)
        {
            base.Do(false);
            return this.GetMallApplyInfoListExt(appId);

        }
        /// <summary>
        /// 获取商品列表 轮播图片 直播列表      
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV3(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListSearchDTO search)
        {
            base.Do(false);
            return this.GetCommodityListV3Ext(search);

        }
        /// <summary>
        /// 获取App入驻类型
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallTypeDTO> GetMallTypeListByEsAppId(System.Guid esAppId)
        {
            base.Do(false);
            return this.GetMallTypeListByEsAppIdExt(esAppId);

        }
    }
}