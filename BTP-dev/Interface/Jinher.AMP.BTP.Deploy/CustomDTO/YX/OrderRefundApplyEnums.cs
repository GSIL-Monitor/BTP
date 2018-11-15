using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 售后申请单状态
    /// </summary>
    public enum OrderRefundApplyResponseStatusEnum
    {
        None = 0, 待审核, 待用户寄回, 用户已寄送, 退货成功, 客服审核不通过, 用户取消, 系统取消, 客服取消, 待客服确认, 客服拒绝 = 11
    }

    /// <summary>
    /// sku的售后信息的状态
    /// </summary>
    public enum OrderRefundApplySkuOperateStatusEnum
    {
        待审核 = 0, 已取消 = 1, 已拒绝 = 2, 审核通过 = 3
    }

    /// <summary>
    /// sku的售后信息的异常类型
    /// </summary>
    public enum OrderRefundApplySkuOperateExceptionTypeEnum
    {
        正常 = 0, 未匹配售后单, 匹配多个售后单, 商品多退, 商品少退, 非严选商品 = 5
    }

    /// <summary>
    /// 处理方式
    /// </summary>
    public enum OrderRefundApplySkuQualityTreatmentMethodEnum
    {
        损毁 = 0, 退回供应商, 内部微瑕品销售, 换包装, 可直接二次销售 = 4
    }
}
