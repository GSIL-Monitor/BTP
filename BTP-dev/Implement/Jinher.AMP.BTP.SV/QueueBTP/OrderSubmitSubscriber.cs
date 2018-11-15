using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.EMB.MB;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV.QueueBTP
{
    /// <summary>
    /// 订阅者，接收并处理消息
    /// </summary>
    public class OrderSubmitSubscriber : IPushHandler
    {
        public void Process(ReceiveMessage receiveMsg)
        {
            JAP.Common.Loging.LogHelper.Info("OrderSubmitObserver Begin");

            OrderQueueDTO dto = receiveMsg.ReveiveData as OrderQueueDTO;
            if (dto != null)
            {
                CommodityOrderSV facade = new CommodityOrderSV();
                OrderResultDTO result = facade.SubmitOrder(dto);
                if (result.ResultCode != 0)
                {
                    LogHelper.Error(string.Format("下订单失败，订单Id：{0}，错误消息：{1}",dto.OrderId,result.Message));
                }
            }
            JAP.Common.Loging.LogHelper.Info("OrderSubmitObserver End");
        }
    }
}
