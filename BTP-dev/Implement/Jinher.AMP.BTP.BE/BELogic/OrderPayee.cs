

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
namespace Jinher.AMP.BTP.BE
{
    public partial class OrderPayee
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        /// <summary>
        /// 生成钱款去向实体
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="payeeId">收款人</param>
        /// <param name="isJHSelfUseAccount">是否金和自用账户</param>
        /// <param name="isVoucherBuyGold">是否需要代金券换购金币</param>
        /// <param name="payMoney">支付金额(单位:元)</param>
        /// <param name="payeeType">收款人角色,1:商家，2：金和众销（给金和分的钱），3：商贸众销（给分享者分的钱），4：商贸众筹，5：推广主分成，6：应用主分成，7金和分润，8买家, 30：商城分润</param>
        /// <param name="description">收款人角色描述,1:商家，2：金和众销（给金和分的钱），3：商贸众销（给分享者分的钱），4：商贸众筹，5：推广主分成，6：应用主分成，7金和分润，8买家，30：商城分润</param>
        /// <returns></returns>
        public static OrderPayee CreateOrderPayee(Guid orderId, Guid payeeId, bool isJHSelfUseAccount, bool isVoucherBuyGold, decimal payMoney, int payeeType, string description)
        {
            var result = CreateOrderPayee();
            result.OrderId = orderId;
            result.PayeeId = payeeId;
            result.IsJHSelfUseAccount = isJHSelfUseAccount;
            result.IsVoucherBuyGold = isVoucherBuyGold;
            result.PayMoney = payMoney;
            result.PayeeType = payeeType;
            result.Description = description;
            return result;
        }
    }
}



