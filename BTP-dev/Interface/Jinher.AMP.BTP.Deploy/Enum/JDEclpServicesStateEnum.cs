using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 进销存-京东售后服务单状态
    /// </summary>
    [DataContract]
    public enum JDEclpServicesStateEnum
    {
        [Description("京东创建服务单接口调用失败")]
        AddEclpServiceInterfaceFail = 1000,
        /// <summary>
        /// 京东创建服务单接口调用成功但售后服务失败
        /// </summary>
        [Description("京东创建服务单接口调用成功但添加订单失败")]
        AddEclpServiceResultFail = 2000,
        /// <summary>
        /// 京东创建服务单接口调用成功且售后服务成功
        /// </summary>
        [Description("京东创建服务单接口调用成功且添加订单成功")]
        AddEclpServiceResultSuccess = 3000,
        /// <summary>
        /// 已审核待发运
        /// </summary>
        [Description("已审核待发运")]
        EclpServiceState1 = 1,
        /// <summary>
        /// 已发运待收货
        /// </summary>
        [Description("已发运待收货")]
        EclpServiceState6 = 6,
        /// <summary>
        /// 已收货待处理
        /// </summary>
        [Description("已收货待处理")]
        EclpServiceState2 = 2,
        /// <summary>
        /// 已处理待确认
        /// </summary>
        [Description("已处理待确认")]
        EclpServiceState3 = 3,
        /// <summary>
        /// 已完成并下发备件库
        /// </summary>
        [Description("已完成并下发备件库")]
        EclpServiceState20080 = 20080,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        EclpServiceState5 = 5,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        EclpServiceState20090 = 20090,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        EclpServiceState20100 = 20100,
    }
}
