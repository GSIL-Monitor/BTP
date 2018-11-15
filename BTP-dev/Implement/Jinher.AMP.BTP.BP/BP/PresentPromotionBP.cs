
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/12/1 14:13:44
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
    public partial class PresentPromotionBP : BaseBP, IPresentPromotion
    {

        /// <summary>
        /// 查询活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionSearchResultDTO>> GetPromotions(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionSearchDTO input)
        {
            base.Do();
            return this.GetPromotionsExt(input);
        }
        /// <summary>
        /// 结束活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EndPromotion(System.Guid id)
        {
            base.Do();
            return this.EndPromotionExt(id);
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePromotion(System.Guid id)
        {
            base.Do();
            return this.DeletePromotionExt(id);
        }
        /// <summary>
        /// 获取活动详细信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO> GetPromotionDetails(System.Guid id)
        {
            base.Do();
            return this.GetPromotionDetailsExt(id);
        }
        /// <summary>
        /// 发布活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO input)
        {
            base.Do();
            return this.CreatePromotionExt(input);
        }
        /// <summary>
        /// 更新活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO input)
        {
            base.Do();
            return this.UpdatePromotionExt(input);
        }
        /// <summary>
        /// 查询商品
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCommoditySearchResultDTO>> GetCommodities(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCommoditySearchDTO input)
        {
            base.Do(false);
            return this.GetCommoditiesExt(input);
        }
    }
}