using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class AfsServicebyCustomerPinPage
    {
        /// <summary>
        /// 组件列表
        /// </summary>
        public List<AfsServicebyCustomerPin> serviceInfoList { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int totalNum { get; set; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageNum { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string pageIndex { get; set; }
    }

    public class AfsServicebyCustomerPin
    {
        /// <summary>
        /// 服务单号
        /// </summary>
        public string afsServiceId { get; set; }
        /// <summary>
        /// 服务类型码(退货(10)、换货(20)、维修(30))
        /// </summary>
        public int customerExpect { get; set; }
        /// <summary>
        /// 服务类型名称
        /// </summary>
        public string customerExpectName { get; set; }
        /// <summary>
        /// 服务单申请时间
        /// </summary>
        public string afsApplyTime { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string wareId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string wareName { get; set; }
        /// <summary>
        /// 服务单环节(申请阶段(10),审核不通过(20),客服审核(21),商家审核(22),京东收货(31),商家收货(32), 京东处理(33)
        /// 商家处理(34), 用户确认(40),完成(50), 取消(60))
        /// </summary>
        public int afsServiceStep { get; set; }
        /// <summary>
        /// 服务单环节名称
        /// </summary>
        public string afsServiceStepName { get; set; }
        /// <summary>
        /// 是否可取消(0代表否，1代表是)
        /// </summary>
        public int cancel { get; set; }
    }
}
