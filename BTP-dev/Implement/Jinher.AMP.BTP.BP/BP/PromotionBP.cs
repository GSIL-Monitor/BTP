
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/17 14:51:03
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
    public partial class PromotionBP : BaseBP, IPromotion
    {

        /// <summary>
        /// 添加折扣
        /// </summary>
        /// <param name="discountsDTO">自定义折扣属性</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddPromotion(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            base.Do();
            return this.AddPromotionExt(discountsDTO);
        }
        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="id">促销ID</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelPromotion(System.Guid id)
        {
            base.Do();
            return this.DelPromotionExt(id);
        }
        /// <summary>
        /// 修改折扣
        /// </summary>
        /// <param name="discountsDTO">自定义属性</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            base.Do();
            return this.UpdatePromotionExt(discountsDTO);
        }
        /// <summary>
        /// 查询所有折扣
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM> GetAllPromotion(System.Guid sellerID, int pageSize, int pageIndex, string startTime, string endTime, string sintensity, string eintensity, string commodityName, string state, out int rowCount)
        {
            base.Do();
            return this.GetAllPromotionExt(sellerID, pageSize, pageIndex, startTime, endTime, sintensity, eintensity, commodityName, state, out rowCount);
        }
        /// <summary>
        /// 根据促销ID查询促销商品
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM> GetPromotionItemsByPromotionID(System.Guid promotionID, Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search, int pageSize, int pageIndex, out int rownum)
        {
            base.Do();
            return this.GetPromotionItemsByPromotionIDExt(promotionID, search, pageSize, pageIndex, out rownum);
        }
        /// <summary>
        /// 根据促销ID查询促销详情
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM GetPromotionByPromotionID(System.Guid promotionID)
        {
            base.Do();
            return this.GetPromotionByPromotionIDExt(promotionID);
        }
        /// <summary>
        /// 查询所有能打折所有商品ID
        /// </summary>
        /// <param name="appID">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetCommodityID(System.Guid appID, System.DateTime startTime, System.DateTime endTime)
        {
            base.Do();
            return this.GetCommodityIDExt(appID, startTime, endTime);
        }
        /// <summary>
        /// 查询所有折扣商品的编号
        /// </summary>
        /// <param name="appid">appID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<string> GetCommodityCodeByPromotion(System.Guid appid, System.DateTime startTime, System.DateTime endTime)
        {
            base.Do();
            return this.GetCommodityCodeByPromotionExt(appid, startTime, endTime);
        }
        /// <summary>
        /// 得到促销下所有的促销商品
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PromotionItemsDTO> GetAllPromotionItems(System.Guid promotionId)
        {
            base.Do();
            return this.GetAllPromotionItemsExt(promotionId);
        }
        /// <summary>
        /// 删除促销商品
        /// </summary>
        /// <param name="promotionId"></param>
        public void DeletePromotionItems(System.Guid promotionId)
        {
            base.Do();
            this.DeletePromotionItemsExt(promotionId);
        }
        /// <summary>
        /// 判断同一时期不能超过5个促销
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public bool IsAddPromotion(System.DateTime starTime, System.DateTime endTime, System.Guid appId)
        {
            base.Do();
            return this.IsAddPromotionExt(starTime, endTime, appId);
        }
        /// <summary>
        /// 同一促销时期不能添加的商品
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> IsCommodityCan(System.DateTime starTime, System.DateTime endTime, System.Guid appId)
        {
            base.Do();
            return this.IsCommodityCanExt(starTime, endTime, appId);
        }

        public System.Collections.Generic.List<CommodityDTO> GetCommodityByPromotionID(Guid promotionID)
        {
            base.Do();
            return this.GetCommodityByPromotionIDExt(promotionID);
        }

        public int GetAllCommodityNum(System.DateTime starTime, System.DateTime endTime, Guid appId, Guid? promotionid)
        {
            base.Do();
            return this.GetAllCommodityNumExt(starTime, endTime, appId, promotionid);
        }

        public List<string> GetCodes(List<string> commCodes, Guid appId)
        {
            base.Do();
            return this.GetCodesExt(commCodes, appId);
        }

        public List<Guid> GetCommodityIds(Guid promotionId)
        {
            base.Do();
            return this.GetCommodityIdsExt(promotionId);
        }

        public int GetCommodityNum(Guid promotionId)
        {
            base.Do();
            return this.GetCommodityNumExt(promotionId);
        }
    }
}