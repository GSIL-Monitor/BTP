
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/5/10 17:19:30
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class OrderExpressRouteFacade : BaseFacade<IOrderExpressRoute>
    {

        /// <summary>
        /// 接收快递鸟推送的物流路由信息。
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReceiveKdniaoExpressRoute(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> oerList)
        {
            base.Do();
            return this.Command.ReceiveKdniaoExpressRoute(oerList);
        }
        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO express)
        {
            base.Do();
            return this.Command.GetExpressRouteByExpNo(express);
        }
        /// <summary>
        /// 根据快递单号获取快递信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO GetExpressRouteByExpOrderNo(string expOrderNo)
        {
            base.Do();
            return this.Command.GetExpressRouteByExpOrderNo(expOrderNo);
        }
        /// <summary>
        /// 修改快递信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateExpressRoute(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO model)
        {
            base.Do();
            return this.Command.UpdateExpressRoute(model);
        }

        /// <summary>
        ///  获取物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJdJob()
        {
            base.Do();
            this.Command.GetOrderExpressForJdJob();
        }

        /// <summary>
        ///  获取物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJsJob()
        {
            base.Do();
            this.Command.GetOrderExpressForJsJob();
        }

        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpress(System.Guid AppId, System.Guid Userid)
        {
            base.Do();
            return this.Command.GetUserNewOrderExpress(AppId, Userid);
        }
    }
}
