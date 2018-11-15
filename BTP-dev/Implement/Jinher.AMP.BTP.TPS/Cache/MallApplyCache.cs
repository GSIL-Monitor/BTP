using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS.Cache
{
    public class MallApplyCache
    {
        /// <summary>
        /// 获取App入驻类型
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public List<MallTypeDTO> GetMallTypeListByEsAppId(Guid esAppId)
        {
            var key = "MallType:EAI:" + esAppId.ToString();
            //var data = GlobalCacheWrapper.GetDataCache(key, Consts.CacheSetting) as List<MallTypeDTO>;
            var data = RedisHelper.Get<List<MallTypeDTO>>(key);
            if (data == null)
            {
                data = MallApply.GetTGQuery(esAppId).Select(_ => new MallTypeDTO { Id = _.AppId, Type = _.Type }).ToList();
                //GlobalCacheWrapper.AddCache(key, apps, TimeSpan.FromMinutes(30), CacheTypeEnum.redis, Consts.CacheSetting);
                RedisHelper.Set(key, data, TimeSpan.FromMinutes(30));
                LogHelper.Info("GetMallTypeListByEsAppId from DB......");
            }
            else
            {
                //LogHelper.Debug("GetMallTypeListByEsAppId from Cache......");
            }
            return data;
        }

        /// <summary>
        /// 获取入驻的App列表
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public List<Guid> GetAppIdList(Guid esAppId)
        {
            return GetMallTypeListByEsAppId(esAppId).Select(_ => _.Id).ToList();
        }

      

    }
}