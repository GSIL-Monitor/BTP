using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class CompatibleServiceDetailDTO
    {
        /// <summary>
        /// 服务单号
        /// </summary>
        public string afsServiceId { get; set; }

        /// <summary>
        /// 服务类型码（退货(10)、换货(20)、维修(20)）
        /// </summary>
        public string customerExpect { get; set; }

        /// <summary>
        /// 服务单申请时间
        /// </summary>
        public string afsApplyTime { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 是不是有发票（0没有 1有）
        /// </summary>
        public int isHasInvoice { get; set; }

        /// <summary>
        /// 是不是有检测报告（0没有 1有）
        /// </summary>
        public int isNeedDetectionReport { get; set; }

        /// <summary>
        /// 是不是有包装（0没有 1有）
        /// </summary>
        public string isHasPackage { get; set; }

        /// <summary>
        /// 上传图片访问地址（不同图片逗号分割，可能为空）
        /// </summary>
        public string questionPic { get; set; }

        /// <summary>
        /// 服务单环节（申请阶段(10),审核不通过(20),客服审核(21),商家审核(22),京东收货(31),商家收货(32), 京东处理(33),商家处理(34), 用户确认(40),完成(50), 取消 (60)）
        /// </summary>
        public string afsServiceStep { get; set; }

        /// <summary>
        /// 服务单环节名称（申请阶段,客服审核,商家审核,京东收货,商家收货, 京东处理,商家处理, 用户确认,完成, 取消;）
        /// </summary>
        public string afsServiceStepName { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string approveNotes { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string questionDesc { get; set; }

        /// <summary>
        /// 审核结果（直赔积分 (11), 直赔余额 (12), 直赔优惠卷 (13), 直赔京豆 (14), 直赔商品 (21), 上门换新 (22), 自营取件 (31), 客户送货(32), 客户发货 (33),闪电退款 (34), 虚拟退款 (35), 大家电检测 (80), 大家电安装 (81), 大家电移机 (82), 大家电维修 (83), 大家电其它 (84)）
        /// </summary>
        public int? approvedResult { get; set; }

        /// <summary>
        /// 审核结果名称
        /// </summary>
        public string approvedResultName { get; set; }

        /// <summary>
        /// 处理结果(返修换新(23), 退货(40), 换良(50),原返60),病单 (71),出检 (72),维修(73),强制关单 (80),线下换新 (90))
        /// </summary>
        public int? processResult { get; set; }

        /// <summary>
        /// 处理结果名称
        /// </summary>
        public string processResultName { get; set; }

        /// <summary>
        /// 获取服务单允许的操作列表(列表为空代表不允许操作 列表包含1代表取消 列表包含2代表允许填写或者修改客户发货信息)
        /// </summary>
        public List<int> allowOperations { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        public ServiceCustomerInfoDTO serviceCustomerInfoDTO { get; set; }

        /// <summary>
        /// 售后地址信息
        /// </summary>
        public ServiceAftersalesAddressInfoDTO serviceAftersalesAddressInfoDTO { get; set; }

        /// <summary>
        /// 客户发货信息
        /// </summary>
        public ServiceExpressInfoDTO serviceExpressInfoDTO { get; set; }

        /// <summary>
        /// 退款明细
        /// </summary>
        public List<ServiceFinanceDetailInfoDTO> serviceFinanceDetailInfoDTOs { get; set; }

        /// <summary>
        /// 服务单追踪信息
        /// </summary>
        public List<ServiceTrackInfoDTO> serviceTrackInfoDTOs { get; set; }

        /// <summary>
        /// 服务单商品明细
        /// </summary>
        public List<ServiceDetailInfoDTO> serviceDetailInfoDTOs { get; set; }
    }

    /// <summary>
    /// 客户信息
    /// </summary>
    public class ServiceCustomerInfoDTO
    {
        /// <summary>
        /// 客户京东账号
        /// </summary>
        public string customerPin { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string customerName { get; set; }

        /// <summary>
        /// 服务单联系人
        /// </summary>
        public string customerContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string customerTel { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string customerMobilePhone { get; set; }

        /// <summary>
        /// 电子邮件地址
        /// </summary>
        public string customerEmail { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string customerPostcode { get; set; }
    }

    /// <summary>
    /// 售后地址信息
    /// </summary>
    public class ServiceAftersalesAddressInfoDTO
    {
        /// <summary>
        /// 售后地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 售后电话
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 售后联系人
        /// </summary>
        public string linkMan { get; set; }

        /// <summary>
        /// 售后邮编
        /// </summary>
        public string postCode { get; set; }
    }

    /// <summary>
    /// 客户发货信息
    /// </summary>
    public class ServiceExpressInfoDTO
    {
        /// <summary>
        /// 服务单号
        /// </summary>
        public string afsServiceId { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public string freightMoney { get; set; }

        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string expressCompany { get; set; }

        /// <summary>
        /// 客户发货日期
        /// </summary>
        public string deliverDate { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string expressCode { get; set; }
    }

    /// <summary>
    /// 退款明细
    /// </summary>
    public class ServiceFinanceDetailInfoDTO
    {
        /// <summary>
        /// 退款方式
        /// </summary>
        public string refundWay { get; set; }

        /// <summary>
        /// 退款方式名称
        /// </summary>
        public string refundWayName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string statusName { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal refundPrice { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string wareName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string wareId { get; set; }
    }

    /// <summary>
    /// 服务单追踪信息
    /// </summary>
    public class ServiceTrackInfoDTO
    {
        /// <summary>
        /// 服务单号
        /// </summary>
        public string afsServiceId { get; set; }

        /// <summary>
        /// 追踪标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 追踪内容
        /// </summary>
        public string context { get; set; }

        /// <summary>
        /// 提交时间(格式为yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string createDate { get; set; }

        /// <summary>
        /// 操作人昵称
        /// </summary>
        public string createName { get; set; }

        /// <summary>
        /// 操作人账号
        /// </summary>
        public string createPin { get; set; }
    }

    /// <summary>
    /// 服务单商品明细
    /// </summary>
    public class ServiceDetailInfoDTO
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string wareId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string wareName { get; set; }

        /// <summary>
        /// 明细类型(主商品(10), 赠品(20), 附件(30)，拍拍取主商品就可以)
        /// </summary>
        public int afsDetailType { get; set; }

        /// <summary>
        /// 附件描述
        /// </summary>
        public string wareDescribe { get; set; }
    }
}
