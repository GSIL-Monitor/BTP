using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 进销存-京东订单状态
    /// </summary>
    [DataContract]
    public enum JDEclpOrderStateEnum
    {
        /// <summary>
        /// 京东销售出库单接口调用失败
        /// </summary>
        [Description("京东销售出库单接口调用失败")]
        AddEclpOrderInterfaceFail = 1000,
        /// <summary>
        /// 京东销售出库单接口调用成功但添加订单失败
        /// </summary>
        [Description("京东销售出库单接口调用成功但添加订单失败")]
        AddEclpOrderResultFail = 2000,
        /// <summary>
        /// 京东销售出库单接口调用成功且添加订单成功
        /// </summary>
        [Description("京东销售出库单接口调用成功且添加订单成功")]
        AddEclpOrderResultSuccess = 3000,
        /// <summary>
        /// 修改金和订单状态为已发货失败
        /// </summary>
        [Description("修改金和订单状态为已发货失败")]
        UpdateOrderStateFail = 4000,
        /// <summary>
        /// 已下发库房
        /// </summary>
        [Description("已下发库房")]
        EclpOrderState10014 = 10014,
        /// <summary>
        /// 复核
        /// </summary>
        [Description("复核")]
        EclpOrderState10017 = 10017,
        /// <summary>
        /// 货品已打包
        /// </summary>
        [Description("货品已打包")]
        EclpOrderState10018 = 10018,
        /// <summary>
        /// 交接发货
        /// </summary>
        [Description("交接发货")]
        EclpOrderState10019 = 10019,
        /// <summary>
        /// 取消成功
        /// </summary>
        [Description("取消成功")]
        EclpOrderState10028 = 10028,
        /// <summary>
        /// 站点验收
        /// </summary>
        [Description("站点验收")]
        EclpOrderState10033 = 10033,
        /// <summary>
        /// 妥投
        /// </summary>
        [Description("妥投")]
        EclpOrderState10034 = 10034,
        /// <summary>
        /// 拒收
        /// </summary>
        [Description("拒收")]
        EclpOrderState10035 = 10035,
        /// <summary>
        /// 逆向完成
        /// </summary>
        [Description("逆向完成")]
        EclpOrderState10038 = 10038
    }
}
