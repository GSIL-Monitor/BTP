
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/9/9 15:11:27
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
    /// <summary>
    /// 缓存服务
    /// </summary>
    public class CacheFacade : BaseFacade<ICache>
    {

        /// <summary>
        /// 清空App的缓存
        /// </summary>
        public void RemoveAppCache()
        {
            base.Do();
            this.Command.RemoveAppCache();
        }
    }
}