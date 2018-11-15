using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 快递模板属性类型
    /// </summary>
    [DataContract]
    public enum TemplatePropertyType
    {
        /// <summary>
        /// 发件人单位名称
        /// </summary>
        [EnumMemberAttribute]
        SenderCompany = 0,

        /// <summary>
        /// 发件人发件日期
        /// </summary>
        [EnumMemberAttribute]
        SendTime = 1,

        /// <summary>
        /// 发件人姓名
        /// </summary>
        [EnumMemberAttribute]
        SenderName = 2,

        /// <summary>
        /// 发件人手机
        /// </summary>
        [EnumMemberAttribute]
        SenderPhone = 3,

        /// <summary>
        /// 发件人电话
        /// </summary>
        [EnumMemberAttribute]
        SenderTel = 4,

        /// <summary>
        /// 发件人邮编
        /// </summary>
        [EnumMemberAttribute]
        SendPostCode = 5,

        /// <summary>
        /// 发件人地址：=Province+City+County+SenderDetailAddress
        /// </summary>
        [EnumMemberAttribute]
        SenderAddress = 6,

        /// <summary>
        /// 所在省
        /// </summary>
        [EnumMemberAttribute]
        SenderProvince = 7,

        /// <summary>
        /// 市
        /// </summary>
        [EnumMemberAttribute]
        SenderCity = 8,

        /// <summary>
        /// 区
        /// </summary>
        [EnumMemberAttribute]
        SenderCounty = 9,

        /// <summary>
        /// 发件人具体地址
        /// </summary>
        [EnumMemberAttribute]
        SenderDetailAddress = 10,


        /// <summary>
        /// 收件人单位名称
        /// </summary>
        [EnumMemberAttribute]
        ReceiptCompany = 11,

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [EnumMemberAttribute]
        ReceiptUserName =12,

        /// <summary>
        /// 收件人地址：=Province+City+County+ReceiptDetailAddress
        /// </summary>
        [EnumMemberAttribute]
        ReceiptAddress =13,

        /// <summary>
        /// 收件人所在省
        /// </summary>
        [EnumMemberAttribute]
        ReceiptProvince = 14,

        /// <summary>
        /// 收件人所在城市
        /// </summary>
        [EnumMemberAttribute]
        ReceiptCity = 15,

        /// <summary>
        /// 收件人所在区县
        /// </summary>
        [EnumMemberAttribute]
        ReceiptCounty = 16,

        /// <summary>
        /// 收件人具体地址
        /// </summary>
        [EnumMemberAttribute]
        ReceiptDetailAddress = 17,

        /// <summary>
        /// 收件人手机
        /// </summary>
        [EnumMemberAttribute]
        ReceiptPhone = 18,

        /// <summary>
        /// 收件人电话
        /// </summary>
        [EnumMemberAttribute]
        ReceiptTel = 19,

        /// <summary>
        /// 收件人邮编
        /// </summary>
        [EnumMemberAttribute]
        RecipientsZipCode =20,

        /// <summary>
        /// 订单编号
        /// </summary>
        [EnumMemberAttribute]
        OrderCode =21,

        /// <summary>
        /// 卖家备注
        /// </summary>
        [EnumMemberAttribute]
        SellersRemark = 22,

        /// <summary>
        /// 买家留言备注
        /// </summary>
        [EnumMemberAttribute]
        BuyersRemark =23,

        /// <summary>
        /// 买家昵称
        /// </summary>
        [EnumMemberAttribute]
        BuyersNickName =24,

        /// <summary>
        /// 宝贝名称/宝贝简称/商家编码/销售属性/数量/
        /// </summary>
        [EnumMemberAttribute]
        Other =25
    }
}
