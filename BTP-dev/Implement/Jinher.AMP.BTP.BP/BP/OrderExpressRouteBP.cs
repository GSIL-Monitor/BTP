
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/5/10 17:19:33
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class OrderExpressRouteBP : BaseBP, IOrderExpressRoute
    {

        /// <summary>
        /// 接收快递鸟推送的物流路由信息。
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReceiveKdniaoExpressRoute(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> oerList)
        {
            base.Do();
            return this.ReceiveKdniaoExpressRouteExt(oerList);
        }
        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO express)
        {
            base.Do();
            return this.GetExpressRouteByExpNoExt(express);
        }
        /// <summary>
        /// 根据快递单号获取快递信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO GetExpressRouteByExpOrderNo(string expOrderNo)
        {
            base.Do();
            return this.GetExpressRouteByExpOrderNoExt(expOrderNo);
        }
        /// <summary>
        /// 修改快递信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateExpressRoute(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO model)
        {
            base.Do();
            return this.UpdateExpressRouteExt(model);
        }
        /// <summary>
        ///  获取京东物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJdJob()
        {
            base.Do(false);
            this.GetOrderExpressForJdJobExt();
        }
        /// <summary>
        ///  获取急速数据物流跟踪信息
        /// </summary>
        public void GetOrderExpressForJsJob()
        {
            base.Do(false);
            this.GetOrderExpressForJsJobExt();
        }
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpress(System.Guid AppId, System.Guid Userid)
        {
            base.Do();
            return this.GetUserNewOrderExpressExt(AppId, Userid);
        }
    }
}