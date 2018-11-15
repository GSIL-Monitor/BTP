using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 添加购物车请求实体
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShoppingCartItemSDTO
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 实际购买价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 尺寸颜色ID
        /// </summary>
        [DataMemberAttribute()]
        public string SizeAndColorId { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? CommodityStockId { get; set; }

        /// <summary>
        /// 促销ID（是促销商品时添加）
        /// </summary>
        [DataMemberAttribute()]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }

        /// <summary>
        /// 购物车实体id
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShopCartItemId { get; set; }


        /// <summary>
        /// 订单项Id
        /// </summary>
        [DataMember]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 优惠价 
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// 下单商品分类
        /// </summary>
        [DataMember]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// 积分抵用金额
        /// </summary>
        [DataMember]
        public decimal ScorePrice { get; set; }

        /// <summary>
        /// 关税单价
        /// </summary>
        public decimal Duty { get; set; }

        /// <summary>
        /// 关税总价
        /// </summary>
        public decimal DutyAmount
        {
            get { return decimal.Round(Duty * CommodityNumber, 2, MidpointRounding.AwayFromZero); }
        }

        /// <summary>
        /// 销项税
        /// </summary>
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// 进项税
        /// </summary>
        public decimal? InputRax { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal? MarkPrice { get; set; }

        /// <summary>
        /// 进货价
        /// </summary>
        public decimal? CostPrice { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 京东商品编码
        /// </summary>
        public string JDCode { get; set; }

        /// <summary>
        /// 商城品类Ids
        /// </summary>
        public string InnerCatetoryIds { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 订单项赠品列表
        /// </summary>
        [DataMember]
        public List<ShoppingCartItemPresentDTO> Presents { get; set; }

        /// <summary>
        /// 石化二期系统商品编号
        /// 规格设置
        /// </summary>
        public string ErQiCode { get; set; }
        [DataMember]
        public int Specifications { get; set; }


        /// <summary>
        /// 这个商品  跨店满减券分配到的优惠金额
        /// </summary>
        [DataMember]
        public decimal StoreCouponDivide { get; set; }
    }

    /// <summary>
    /// 订单项赠品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShoppingCartItemPresentDTO
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityStockId { get; set; }
        public int Number { get; set; }
    }
}
