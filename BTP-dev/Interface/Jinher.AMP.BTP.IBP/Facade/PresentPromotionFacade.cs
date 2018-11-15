
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/12/1 14:13:43
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
    public class PresentPromotionFacade : BaseFacade<IPresentPromotion>
    {

        /// <summary>
        /// 查询活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionSearchResultDTO>> GetPromotions(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionSearchDTO input)
        {
            base.Do();
            return this.Command.GetPromotions(input);
        }
        /// <summary>
        /// 结束活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EndPromotion(System.Guid id)
        {
            base.Do();
            return this.Command.EndPromotion(id);
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePromotion(System.Guid id)
        {
            base.Do();
            return this.Command.DeletePromotion(id);
        }
        /// <summary>
        /// 获取活动详细信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO> GetPromotionDetails(System.Guid id)
        {
            base.Do();
            return this.Command.GetPromotionDetails(id);
        }
        /// <summary>
        /// 发布活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO input)
        {
            base.Do();
            return this.Command.CreatePromotion(input);
        }
        /// <summary>
        /// 更新活动
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCreateDTO input)
        {
            base.Do();
            return this.Command.UpdatePromotion(input);
        }
        /// <summary>
        /// 查询商品
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCommoditySearchResultDTO>> GetCommodities(Jinher.AMP.BTP.Deploy.CustomDTO.PresentPromotionCommoditySearchDTO input)
        {
            base.Do();
            return this.Command.GetCommodities(input);
        }
    }
}