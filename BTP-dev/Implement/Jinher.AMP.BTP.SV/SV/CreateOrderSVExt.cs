
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/10/28 18:04:24
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.SV.Order;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CreateOrderSV : BaseSv, ICreateOrder
    {


        #region GetCreateOrderInfoExt
        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderNeedDTO> GetCreateOrderInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderSearchDTO search)
        {
            //TODO 
            CreateOrderRequire requires = new CreateOrderRequire(search);
 
            return new ResultDTO<CreateOrderNeedDTO>();
        }


        #endregion
    }
}