
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/9 15:11:30
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 缓存服务
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CacheSV : BaseSv, ICache
    {

        /// <summary>
        /// 清空App的缓存
        /// </summary>
        public void RemoveAppCache()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.RemoveAppCacheExt();
            timer.Stop();
            LogHelper.Info(string.Format("CacheSV.RemoveAppCache：耗时：{0}。", timer.ElapsedMilliseconds));
        }
    }
}
