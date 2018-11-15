
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/9 15:11:33
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 缓存服务
    /// </summary>
    public partial class CacheSV : BaseSv, ICache
    {
        /// <summary>
        /// 清空App的缓存
        /// </summary>
        public void RemoveAppCacheExt()
        {
            try
            {
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.AppInZPH, "BTPCache", CacheTypeEnum.redisSS);
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.AppInfo, "BTPCache", CacheTypeEnum.redisSS);
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.AppNameIcon, "BTPCache", CacheTypeEnum.redisSS);
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.AppOwnerType, "BTPCache", CacheTypeEnum.redisSS);
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.OrderBatch + ":" + DateTime.Today.AddDays(-1).ToString("yyyyMMdd"), "BTPCache", CacheTypeEnum.redisSS);
            }
            catch (Exception ex)
            {
                LogHelper.Error("CacheSV.RemoveAppCacheExt。清理商品缓存异常。", ex);
            }
        }
    }
}
