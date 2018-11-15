

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
    public partial class Journal
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
        /// 生成Jounal实体
        /// </summary>
        /// <param name="ucopDto"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="oldState"></param>
        /// <param name="journalName"></param>
        /// <returns></returns>
        public static Journal CreateJournal(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, int oldState, string journalName)
        {
            Journal journal = CreateJournal();
            journal.Code = commodityOrder.Code;
            journal.SubId = ucopDto.userId == Guid.Empty ? commodityOrder.SubId : ucopDto.userId;
            journal.SubTime = DateTime.Now;
            journal.CommodityOrderId = commodityOrder.Id;
            journal.OrderType = commodityOrder.OrderType;
            journal.Name = journalName;

            if (ucopDto.orderItemId == Guid.Empty)
            {
                journal.Details = "订单状态由" + oldState + "变为" + ucopDto.targetState;
                journal.StateFrom = oldState;
                journal.StateTo = ucopDto.targetState;
            }
            else
            {
                journal.Details = "订单单商品退款，订单id：" + commodityOrder.Id + "订单商品项id：" + ucopDto.orderItemId;
                journal.StateFrom = oldState;
                journal.StateTo = ucopDto.targetState;
            }
            return journal;
        }
    }
}



