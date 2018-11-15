using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 快递鸟推送整个内容
    /// </summary>
    [DataContract]
    public class KdNotifyDTO
    {
        /// <summary>
        /// 用户电商ID
        /// </summary>
        [DataMember]
        public string EBusinessID { get; set; }

        /// <summary>
        /// 推送物流单号轨迹个数
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// 推送时间
        /// </summary>
        [DataMember]
        public DateTime PushTime { get; set; }

        /// <summary>
        /// 推送物流单号轨迹集合
        /// </summary>
        [DataMember]
        public List<KDRouteData> Data { get; set; }
    }

    /// <summary>
    /// 物流单号轨迹
    /// </summary>
    [DataContract]
    public class KDRouteData
    {
        /// <summary>
        /// 电商用户ID
        /// </summary>
        [DataMember]
        public string EBusinessID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderCode { get; set; }

        /// <summary>
        /// 快递公司编码
        /// </summary>
        [DataMember]
        public string ShipperCode { get; set; }

        /// <summary>
        /// 物流运单号
        /// </summary>
        [DataMember]
        public string LogisticCode { get; set; }

        /// <summary>
        /// 成功与否
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// 物流状态1：已取件2：在途中3：签收
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 订阅接口的Bk值
        /// </summary>
        [DataMember]
        public string CallBack { get; set; }

        /// <summary>
        /// 物流轨迹详情
        /// </summary>
        [DataMember]
        public List<RouteDetail> Traces { get; set; }
    }

    /// <summary>
    /// 单个物流单号轨迹（路由）信息
    /// </summary>
    [DataContract]
    public class RouteDetail
    {


        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        public DateTime AcceptTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string AcceptStation { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
    }

    /// <summary>
    /// 物流接收到通知后给快递鸟的返回结果。
    /// </summary>
    [DataContract]
    public class KdNotifyResult
    {
        /// <summary>
        /// 用户电商ID
        /// </summary>
        [DataMember]
        public string EBusinessID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 成功与否
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

    }
}