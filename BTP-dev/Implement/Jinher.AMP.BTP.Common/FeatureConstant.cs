using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 权限项
    /// </summary>
    public class FeatureConstant
    {
        /// <summary>
        /// 出售商品下架
        /// </summary>
        public const string BTPOffShelfCom = "BTPOffShelfCom";
        /// <summary>
        /// 出售商品修改
        /// </summary>
        public const string BTPUpdSaleCom = "BTPUpdSaleCom";
        /// <summary>
        /// 出售商品删除
        /// </summary>
        public const string BTPDelSaleCom = "BTPDelSaleCom";       
        /// <summary>
        /// 仓库商品添加
        /// </summary>
        public const string BTPCreateStockCom = "BTPCreateStockCom";
        /// <summary>
        /// 仓库商品上架
        /// </summary>
        public const string BTPShelfCom = "BTPShelfCom";
        /// <summary>
        /// 仓库商品修改
        /// </summary>
        public const string BTPUpdStockCom = "BTPUpdStockCom";
        /// <summary>
        /// 仓库商品删除
        /// </summary>
        public const string BTPDelStockCom = "BTPDelStockCom";
        /// <summary>
        /// 订单状态修改
        /// </summary>
        public const string BTPOrderStateUpd = "BTPOrderStateUpd";
        /// <summary>
        /// 订单价格修改
        /// </summary>
        public const string BTPOrderPriceUpd = "BTPOrderPriceUpd";
        /// <summary>
        /// 三级分销分销商品设置
        /// </summary>
        public const string BTPDistributeCom = "BTPDistributeCom";
         /// <summary>
        /// 三级分销佣金设置
        /// </summary>
        public const string BTPDistributeDivi = "BTPDistributeDivi";
         /// <summary>
        /// 三级分销分销商管理
        /// </summary>
        public const string BTPDistributeManage = "BTPDistributeManage";
    }
}
