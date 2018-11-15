using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS.Cache
{
    public class CommodityCategoryCache
    {
        public CommodityCategoryDTO GetCommodityCategory(Guid appId, Guid commodityId)
        {
            var key = "CommodityCategory:AI_CI:" + appId.ToString() + commodityId.ToString();
            //var data = GlobalCacheWrapper.GetDataCache(key, Consts.CacheSetting) as CommodityCategoryDTO;
            var data = RedisHelper.Get<CommodityCategoryDTO>(key);
            if (data == null)
            {
                var sdata = CommodityCategory.ObjectSet().FirstOrDefault(t => t.CommodityId == commodityId && t.AppId == appId);
                if (sdata == null)
                {
                    data = new CommodityCategoryDTO { Id = Guid.Empty };
                }
                else
                {
                    data = sdata.ToEntityData(); 
                }
                //GlobalCacheWrapper.AddCache(key, data, TimeSpan.FromMinutes(30), CacheTypeEnum.redis, Consts.CacheSetting);
                RedisHelper.Set(key, data, TimeSpan.FromMinutes(30));
                LogHelper.Info("GetCommodityCategory from DB......");
            }
            if (data.Id == Guid.Empty) return null;
            return data;
        }
    }
}
