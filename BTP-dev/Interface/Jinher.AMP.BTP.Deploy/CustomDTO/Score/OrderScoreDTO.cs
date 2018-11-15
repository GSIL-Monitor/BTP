using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class OrderScoreCheckDTO
    {
        /// <summary>
        /// 商城Id
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 下单商品
        /// </summary>
        [DataMember]
        public List<ComScoreCheckDTO> Coms { get; set; }
        /// <summary>
        /// 订单减免
        /// </summary>
        [DataMember]
        public List<OrderReduction> Reductions { get; set; }


    }
    [Serializable]
    [DataContract]
    public class OrderReduction
    {
        /// <summary>
        /// 店铺id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 订单减免
        /// </summary>
        [DataMember]
        public decimal Reduction { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ComScoreCheckDTO
    {
        /// <summary>
        /// 购物车项Id，单品下单id为Guid.Empty
        /// </summary>
        [DataMember]
        public Guid ItemId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 购买价格
        /// </summary>
        [DataMember]
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 属性组合
        /// </summary>
        [DataMember]
        public string ColorAndSize { get; set; }
        /// <summary>
        /// 商品库存Id
        /// </summary>
        [DataMember]
        public Guid? CommodityStockId { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        [DataMember]
        public int Num { get; set; }
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否促销互动（目前判断标准是Price != RealPrice）
        /// </summary>
        [DataMember]
        public bool IsPromotion { get; set; }

        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMember]
        public Guid? OutPromotionId { get; set; }

        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMember()]
        public int Type { get; set; }
    }

    /// <summary>
    /// 订单校验积分抵现
    /// </summary>
    [Serializable]
    [DataContract]
    public class OrderScoreCheckResultDTO
    {
        /// <summary>
        /// 是否可以积分抵现
        /// </summary>
        [DataMember]
        public bool IsCashForScore { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// 积分类型0店铺积分，1通用积分
        /// </summary>
        [DataMember]
        public ScoreTypeEnum ScoreType { get; set; }


        /// <summary>
        /// 兑换比例:积分/ScoreCost=人民币
        /// </summary>
        [DataMember]
        public int ScoreCost { get; set; }

        /// <summary>
        /// 积分可兑换的钱数（单位：元）
        /// </summary>
        [DataMember]
        public decimal Money { get; set; }
        /// <summary>
        /// 积分抵用列表
        /// </summary>
        [DataMember]
        public List<AppScoreDTO> List { get; set; }

        public OrderScoreCheckResultDTO()
        {
            List = new List<AppScoreDTO>();
        }

    }
    [Serializable]
    [DataContract]
    public class AppScoreDTO
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 店铺可抵用积分金额
        /// </summary>
        [DataMember]
        public decimal Money { get; set; }
        /// <summary>
        /// 积分抵现商品明细
        /// </summary>
        [DataMember]
        public List<ComScoreCheckReDTO> Coms { get; set; }

        public AppScoreDTO()
        {
            Coms = new List<ComScoreCheckReDTO>();
        }
    }
    [Serializable]
    [DataContract]
    public class ComScoreCheckReDTO : ComScoreCheckDTO
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        public static ComScoreCheckReDTO FromRequest(ComScoreCheckDTO request, Guid appId)
        {
            ComScoreCheckReDTO result = new ComScoreCheckReDTO();
            result.ItemId = request.ItemId;
            result.CommodityId = request.CommodityId;
            result.RealPrice = request.RealPrice;
            result.ColorAndSize = request.ColorAndSize;
            result.CommodityStockId = request.CommodityStockId;
            result.Num = request.Num;
            result.AppId = appId;
            return result;
        }



        /// <summary>
        /// 积分可兑换的钱数（单位：元）
        /// </summary>
        [DataMember]
        public decimal Money { get; set; }
        ///// <summary>
        ///// 可抵用比例
        ///// </summary>
        //public decimal ScorePercent { get; set; }
        /// <summary>
        /// 可积分抵现金额
        /// </summary>
        public decimal CanScoreMoney { get; set; }
    }
}
