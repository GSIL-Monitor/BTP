using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS.Cache
{
    public class CommoditySpecificationsCache
    {
        public List<CommoditySpecificationsDTO> GetCommoditySpecifications(Guid commodityId)
        {
            var key = "CommoditySpecifications:CI:" + commodityId.ToString();
            var data = RedisHelper.Get<List<CommoditySpecificationsDTO>>(key);
            if (data == null)
            {
                data = CommoditySpecifications.ObjectSet().Where(t => t.CommodityId == commodityId).ToList().Select(_ => _.ToEntityData()).ToList();
                if (data == null)
                {
                    data = new List<CommoditySpecificationsDTO>();
                }
                RedisHelper.Set(key, data, TimeSpan.FromMinutes(30));
                LogHelper.Info("GetCommoditySpecifications from DB......");
            }
            return data;
        }
    }
}
