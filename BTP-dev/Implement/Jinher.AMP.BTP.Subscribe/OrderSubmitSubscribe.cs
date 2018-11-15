using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.EMB.MB;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.Facade;

namespace Jinher.AMP.BTP.Subscribe
{
    public class OrderSubmitSubscribe : IPushHandler
    {
        public void Process(ReceiveMessage receiveMsg)
        {
            LogHelper.Debug("OrderSubmitObserver Begin");
            
            OrderQueueDTO dto = receiveMsg.ReveiveData as OrderQueueDTO;
            if (dto != null)
            {
                
                CommodityOrderFacade facade = new CommodityOrderFacade();
                OrderResultDTO result = facade.SubmitOrder(dto);
                if (result.ResultCode != 0)
                {
                    LogHelper.Error("下订单失败，订单Id：" + dto.OrderId);
                }
            }
            LogHelper.Debug("OrderSubmitObserver End");
        }
    }
}
