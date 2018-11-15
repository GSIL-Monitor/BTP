
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/4 14:29:10
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class OrderExpressRouteSV : BaseSv, IOrderExpressRoute
    {

        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO express)
        {
            base.Do(false);
            return this.GetExpressRouteByExpNoExt(express);

        }
        /// <summary>
        ///  使用job重新订阅快递鸟物流信息（对订阅失败的）。
        /// </summary>
        public void SubscribeOrderExpressForJob()
        {
            base.Do(false);
            this.SubscribeOrderExpressForJobExt();

        }
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpress(System.Guid AppId, System.Guid Userid)
        {
            base.Do(false);
            return this.GetUserNewOrderExpressExt(AppId, Userid);

        }
        /// <summary>
        /// 获取用户最新的所有订单的物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew>> GetUserAllNewOrderExpress(System.Guid AppId, System.Guid UserId)
        {
            base.Do(false);
            return this.GetUserAllNewOrderExpressExt(AppId, UserId);

        }
    }
}