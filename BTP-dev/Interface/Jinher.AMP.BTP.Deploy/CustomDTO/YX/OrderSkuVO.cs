
namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    public class OrderSkuVO
    {
        /***
           * SKU ID
           */
        public string skuId { get; set; }

        /***
         * 商品名称
         */
        public string productName { get; set; }

        /***
         * 商品数量
         */
        public int saleCount { get; set; }

        /***
         * 商品单价
         */
        public decimal originPrice { get; set; }

        /***
         * 金额小计
         */
        public decimal subtotalAmount { get; set; }

        /***
         * 优惠卷金额，可省略
         */
        public decimal couponTotalAmount { get; set; }

        /***
         * 活动优惠金额，可省略
         */
        public decimal activityTotalAmount { get; set; }
    }
}
