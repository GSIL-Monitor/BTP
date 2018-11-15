
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/4 14:29:09
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class OrderExpressRouteFacade : BaseFacade<IOrderExpressRoute>
    {

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
        ///  使用job重新订阅快递鸟物流信息（对订阅失败的）。
        /// </summary>
        public void SubscribeOrderExpressForJob()
        {
            base.Do();
            this.Command.SubscribeOrderExpressForJob();
        }
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpress(System.Guid AppId, System.Guid Userid)
        {
            base.Do();
            return this.Command.GetUserNewOrderExpress(AppId, Userid);
        }
        /// <summary>
        /// 获取用户最新的所有订单的物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew>> GetUserAllNewOrderExpress(System.Guid AppId, System.Guid UserId)
        {
            base.Do();
            return this.Command.GetUserAllNewOrderExpress(AppId, UserId);
        }
    }
}