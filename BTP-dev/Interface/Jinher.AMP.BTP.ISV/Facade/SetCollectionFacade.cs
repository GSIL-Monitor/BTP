
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/5/30 16:02:34
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
    public class SetCollectionFacade : BaseFacade<ISetCollection>
    {

        /// <summary>
        /// 添加商品收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/SaveCommodityCollection
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId">渠道Id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityCollection(System.Guid commodityId, System.Guid userId, System.Guid channelId)
        {
            base.Do();
            return this.Command.SaveCommodityCollection(commodityId, userId, channelId);
        }
        /// <summary>
        /// 店铺收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/SaveAppCollection
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppCollection(System.Guid appId, System.Guid userId, System.Guid channelId)
        {
            base.Do();
            return this.Command.SaveAppCollection(appId, userId, channelId);
        }
        /// <summary>
        /// 根据用户ID查询收藏商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionComs
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCollectionComs(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            base.Do();
            return this.Command.GetCollectionComs(search);
        }
        /// <summary>
        /// 根据用户ID查询收藏商品数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionComsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int GetCollectionComsCount(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            base.Do();
            return this.Command.GetCollectionComsCount(search);
        }
        /// <summary>
        /// 根据用户ID查询收藏商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionApps
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> GetCollectionApps(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            base.Do();
            return this.Command.GetCollectionApps(search);
        }
        /// <summary>
        /// 根据用户ID查询收藏店铺数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionAppsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int GetCollectionAppsCount(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            base.Do();
            return this.Command.GetCollectionAppsCount(search);
        }
        /// <summary>
        /// 删除正品会收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/DeleteCollections
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollections(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            base.Do();
            return this.Command.DeleteCollections(search);
        }
        /// <summary>
        /// 校验是否收藏店铺
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckAppCollected(System.Guid channelId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.CheckAppCollected(channelId, userId, appId);
        }
    }
}