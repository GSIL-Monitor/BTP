using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CreateOrderSearchDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [DataMember]
        public Guid userId { get; set; }

        /// <summary>
        /// 下订单方式： 购物车结算（gouwuche） ;直接购买。
        /// </summary>
        [DataMember]
        public string coType { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        [DataMember]
        public string areaCode { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid esAppId { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid appSelfTakeStationId { get; set; }

        /// <summary>
        /// 收货地址Id
        /// </summary>
        [DataMember]
        public Guid addressId { get; set; }

        /// <summary>
        /// 下单商品列表
        /// </summary>
        [DataMember]
        public List<CreateOrderCom> coms { get; set; }
    }
    /// <summary>
    /// 下订单商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CreateOrderCom
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 属性1
        /// </summary>
        [DataMember]
        public string size { get; set; }
        /// <summary>
        /// 属性2
        /// </summary>
        [DataMember]
        public string color { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        [DataMember]
        public decimal realPrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int number { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid commodityId { get; set; }
        /// <summary>
        /// 商品库存Id（如果没有传'00000000-0000-0000-0000-000000000000'）
        /// </summary>
        [DataMember]
        public Guid commodityStockId { get; set; }
        /// <summary>
        /// 活动Id（如果没有传'00000000-0000-0000-0000-000000000000'）
        /// </summary>
        [DataMember]
        public Guid promotionId { get; set; }
        /// <summary>
        /// 商品校验结果
        /// </summary>
        [DataMember]
        public CreateOrderComCheckResult checkResult { get; set; }

    }
    /// <summary>
    /// 下订单商品校验结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class CreateOrderComCheckResult
    {

        private bool _hasError;
        /// <summary>
        /// 是否校验失败
        /// </summary>
        [DataMember]
        public bool HasError
        {
            get { return _hasError; }
            set { _hasError = value; }
        }

        private bool _canUserBuy;
        /// <summary>
        /// 是否可以购买
        /// </summary>
        [DataMember]
        public bool CanUserBuy
        {
            get { return _canUserBuy; }
            set { _canUserBuy = value; }
        }

        private int _canBuyNum;
        /// <summary>
        /// 可购买数量
        /// </summary>
        [DataMember]
        public int CanBuyNum
        {
            get { return _canBuyNum; }
            set { _canBuyNum = value; }
        }

        private ComCantBuyReasonEnum _errorType;
        /// <summary>
        /// 错误类型
        /// </summary>
        [DataMember]
        public ComCantBuyReasonEnum ErrorType
        {
            get { return _errorType; }
            set { _errorType = value; }
        }

        private string _message;
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="hasError"></param>
        /// <param name="canUserBuy"></param>
        /// <param name="canBuyNum"></param>
        /// <param name="errorType"></param>
        /// <param name="message"></param>
        public void FillData(bool hasError, bool canUserBuy, int canBuyNum, ComCantBuyReasonEnum errorType, string message)
        {
            _hasError = hasError;
            _canUserBuy = canUserBuy;
            _canBuyNum = canBuyNum;
            _errorType = errorType;
            _message = message;
        }

    }

}
