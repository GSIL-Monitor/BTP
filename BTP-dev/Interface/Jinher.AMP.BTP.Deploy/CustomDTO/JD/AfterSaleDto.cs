using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 协议价价格
    /// </summary>
    public class AfterSaleDto
    {
        /// <summary>
        /// 订单号（必填）
        /// </summary>
        public string jdOrderId { get; set; }

        /// <summary>
        /// 客户预期（必填，退货(10)、换货(20)、维修(30)）
        /// </summary>
        public int customerExpect { get; set; }

        /// <summary>
        /// 产品问题描述(最多1000字符)
        /// </summary>
        public string questionDesc { get; set; }

        /// <summary>
        /// 是否需要检测报告
        /// </summary>
        public bool isNeedDetectionReport { get; set; }

        /// <summary>
        /// 问题描述图片(支持多张图片，用逗号分隔（英文逗号）)
        /// </summary>
        public string questionPic { get; set; }

        /// <summary>
        /// 是否有包装
        /// </summary>
        public bool isHasPackage { get; set; }

        /// <summary>
        /// 包装描述(0 无包装 10 包装完整 20 包装破损)
        /// </summary>
        public int packageDesc { get; set; }

        /// <summary>
        /// 客户信息实体
        /// </summary>
        public AfterSaleCustomerDto asCustomerDto { get; set; }

        /// <summary>
        /// 取件信息实体
        /// </summary>
        public AfterSalePickwareDto asPickwareDto { get; set; }

        /// <summary>
        /// 返件信息实体
        /// </summary>
        public AfterSaleReturnwareDto asReturnwareDto { get; set; }

        /// <summary>
        /// 申请单明细
        /// </summary>
        public AfterSaleDetailDto asDetailDto { get; set; }      
    }

    /// <summary>
    /// 客户信息实体
    /// </summary>
    public class AfterSaleCustomerDto
    {
        /// <summary>
        /// 联系人（必填）
        /// </summary>
        public string customerContactName { get; set; }

        /// <summary>
        /// 联系电话（必填）
        /// </summary>
        public string customerTel { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string jdOrderId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string customerMobilePhone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string customerEmail { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string customerPostcode { get; set; }       
    }

    /// <summary>
    /// 取件信息实体
    /// </summary>
    public class AfterSalePickwareDto
    {
        /// <summary>
        /// 取件方式(必填 4 上门取件 7 客户送货 40客户发货)
        /// </summary>
        public int pickwareType { get; set; }

        /// <summary>
        /// 取件省
        /// </summary>
        public int pickwareProvince { get; set; }

        /// <summary>
        /// 取件市
        /// </summary>
        public int pickwareCity { get; set; }

        /// <summary>
        /// 取件县
        /// </summary>
        public int pickwareCounty { get; set; }

        /// <summary>
        /// 取件乡镇
        /// </summary>
        public int pickwareVillage { get; set; }

        /// <summary>
        /// 取件街道地址
        /// </summary>
        public string pickwareAddress { get; set; }       
    }

    /// <summary>
    /// 返件信息实体
    /// </summary>
    public class AfterSaleReturnwareDto
    {
        /// <summary>
        /// 返件方式（自营配送(10),第三方配送(20);换、修这两种情况必填（默认值））
        /// </summary>
        public int returnwareType { get; set; }

        /// <summary>
        /// 返件省
        /// </summary>
        public int returnwareProvince { get; set; }

        /// <summary>
        /// 返件市
        /// </summary>
        public int returnwareCity { get; set; }

        /// <summary>
        /// 返件县
        /// </summary>
        public int returnwareCounty { get; set; }

        /// <summary>
        /// 返件乡镇
        /// </summary>
        public int returnwareVillage { get; set; }

        /// <summary>
        /// 返件街道地址
        /// </summary>
        public string returnwareAddress { get; set; }       
    }

    /// <summary>
    /// 申请单明细
    /// </summary>
    public class AfterSaleDetailDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string skuId { get; set; }

        /// <summary>
        /// 商品申请数量
        /// </summary>
        public int skuNum { get; set; }   
    }
}
