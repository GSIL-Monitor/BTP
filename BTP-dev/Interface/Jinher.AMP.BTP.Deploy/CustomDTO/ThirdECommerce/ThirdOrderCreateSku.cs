
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商订单提交Sku信息
    /// </summary>
    public class ThirdOrderCreateSku
    {
        /// <summary>
        /// skuId
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 商品进货价
        /// </summary>
        public decimal Price { get; set; }      
    }
}
