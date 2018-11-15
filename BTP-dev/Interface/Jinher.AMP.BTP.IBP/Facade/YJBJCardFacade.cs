
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/11/23 10:08:04
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
    public class YJBJCardFacade : BaseFacade<IYJBJCard>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Create(System.Guid orderId)
        {
            base.Do();
            return this.Command.Create(orderId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJBJCardDTO> Get(System.Guid orderId)
        {
            base.Do();
            return this.Command.Get(orderId);
        }
    }
}