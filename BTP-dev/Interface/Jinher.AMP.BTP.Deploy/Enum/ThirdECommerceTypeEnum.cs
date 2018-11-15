using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 第三方电商类型
    /// </summary>
    [DataContract]
    public enum ThirdECommerceTypeEnum
    {
        /// <summary>
        /// 非第三方电商
        /// </summary>
        [Description("非第三方电商")]
        NotThirdECommerce = 0,
        /// <summary>
        /// 京东大客户
        /// </summary>
        [Description("京东大客户")]
        JingDongDaKeHu = 1,
        /// <summary>
        /// 网易严选
        /// </summary>
        [Description("网易严选")]
        WangYiYanXuan = 2,
        /// <summary>
        /// 苏宁易购
        /// </summary>
        [Description("苏宁易购")]
        SuNingYiGou = 3,
        /// <summary>
        /// 通过标准接口接入的电商
        /// </summary>
        [Description("通过标准接口接入的电商")]
        ByBiaoZhunJieKou = 4,
        /// <summary>
        /// 苏宁易购
        /// </summary>
        [Description("方正电商Apple")]
        FangZheng = 5,
        /// <summary>
        /// 易派客
        /// </summary>
        [Description("易派客")]
        YiPaiKe = 6
    }
}
