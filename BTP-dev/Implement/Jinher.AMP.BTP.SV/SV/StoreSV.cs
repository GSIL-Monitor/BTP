
/***************
功能描述: BTPSV
作    者: 
创建时间: 2017/1/6 16:25:13
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE.MongoCollection;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.MongoDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class StoreSV : BaseSv, IStore
    {

        /// <summary>
        /// 获取门店
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStore
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="province">省份名称</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStore(System.Guid appId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetStoreExt(appId, pageIndex, pageSize);

        }
        /// <summary>
        /// 按地区查询门店
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByProvince
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="province">省</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetStoreByProvince(string province, System.Guid appId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetStoreByProvinceExt(province, appId, pageIndex, pageSize);

        }
        /// <summary>
        /// 查询有门店的省份列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetProvince
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<string> GetProvince(System.Guid appId)
        {
            base.Do();
            return this.GetProvinceExt(appId);

        }
        /// <summary>
        ///  获取门店列表（按用户当前位置到门店的距离排序）
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByLocation
        /// </para>
        /// </summary>
        /// <param name="slp">参数实体类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStoreByLocation(Jinher.AMP.BTP.Deploy.CustomDTO.StoreLocationParam slp)
        {
            base.Do(false);
            return this.GetStoreByLocationExt(slp);

        }
        /// <summary>
        /// 获取餐饮平台聚合门店
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetCateringPlatformStore(Jinher.AMP.BTP.Deploy.CustomDTO.StoreLocationParam param)
        {
            base.Do(false);
            return this.GetCateringPlatformStoreExt(param);

        }
        /// <summary>
        /// 初始化数据 http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/InitMongoFromSql
        /// </summary>
        public void InitMongoFromSql()
        {
            base.Do();
            this.InitMongoFromSqlExt();

        }
        /// <summary>
        /// 获取应用下是否只有一个门店（如果只有一个门店返回门店信息） http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetOnlyStoreInApp
        /// <param name="appId">应用id</param>
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetOnlyStoreInApp(System.Guid appId)
        {
            base.Do();
            return this.GetOnlyStoreInAppExt(appId);

        }
        /// <summary>
        /// 获取馆信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Jinher.AMP.ZPH.Deploy.CustomDTO.AppPavilionInfoIICDTO GetAppPavilionInfo(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam param)
        {
            base.Do(false);
            return this.GetAppPavilionInfoExt(param);

        }
        /// <summary>
        /// 获取门店 有效参数：AppId（必填），SubName（非必填）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<StoreSResultDTO> GetAppStores(StoreLocationParam search)
        {
            base.Do(false);
            return this.GetAppStoresExt(search);
        }
        /// <summary>
        /// 获取附近门店 有效参数：AppId（必填），Longitude（必填），Latitude（必填），MaxDistance（非必填）
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetAppStoresByLocation
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<StoreSDTO>> GetAppStoresByLocation(StoreLocationParam search)
        {
            base.Do(false);
            return this.GetAppStoresByLocationExt(search);
        }
    }
}
