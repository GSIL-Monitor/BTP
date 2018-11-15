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
    public class CommodityCache
    {
        public Deploy.CustomDTO.CommoditySDTO GetCommodityDetails(Guid appId, Guid commodityId)
        {
            var key = "CommodityDetails:" + commodityId.ToString();
            var data = RedisHelper.Get<Deploy.CustomDTO.CommoditySDTO>(key);
            if (data == null)
            {
                RedisHelper.Set(key, data, TimeSpan.FromMinutes(10));
                LogHelper.Info("GetCommodityDetails from DB......");
            }
            return data;
        }
    }
}
