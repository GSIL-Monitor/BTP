
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/1/8 9:01:53
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.App.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AppSetSV : BaseSv, IAppSet
    {

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="QryCommodityDTO"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.QryCommodityDTO qryCommodityDTO)
        {
            base.Do(false);
            return this.GetCommodityListExt(qryCommodityDTO);

        }

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(Guid appId)
        {
            base.Do(false);
            return this.GetCategoryExt(appId);
        }


        /// <summary>
        /// 按关键字获取商品列表
        /// </summary>
        /// <param name="want">关键字</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodity(string want, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetWantCommodityExt(want, pageIndex, pageSize);
        }
        /// <summary>
        /// 厂家直营app查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public AppSetAppGridDTO GetAppSet(AppSetSearchDTO search)
        {
            base.Do(false);
            return this.GetAppSetExt(search);
        }
        /// <summary>
        /// 根据分类Id获取该分类下的app列表
        /// </summary>
        /// <param name="search"></param>
        public List<AppSetAppDTO> GetCategoryAppList(AppSetSearchDTO search)
        {
            base.Do(false);
            return this.GetCategoryAppListExt(search);
        }
        /// <summary>
        /// 获取正品会“我的”，各栏目数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfoCountDTO GetUserInfoCount(Guid userId, Guid esAppId)
        {
            base.Do();
            return this.GetUserInfoCountExt(userId, esAppId);
        }

        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns>结果</returns>
        public ResultDTO RemoveAppInZPHCache()
        {
            base.Do(false);
            return this.RemoveAppInZPHCacheExt();
        }
        /// <summary>
        /// 获取商品列表（平台获取平台商品、店铺获取店铺商品）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityListV2Ext(search);
            timer.Stop();
            LogHelper.Debug(string.Format("AppSetSV.GetCommodityListV2：耗时：{0}。入参：search:{1} \r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 浏览过的店铺（20个）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> GetBrowseAppInfo(Guid userId, Guid appId)
        {
            base.Do(false);
            return this.GetBrowseAppInfoExt(userId, appId);
        }
        /// <summary>
        /// 分页获取浏览商品记录
        /// </summary>
        /// <param name="par"></param>
        public List<BTP.Deploy.CustomDTO.CommodityListCDTO> GetBrowseCommdity(BrowseParameter par)
        {
            base.Do(false);
            return this.GetBrowseCommdityExt(par);

        }
        /// <summary>
        /// 删除商品浏览记录
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <param name="CommdityId"></param>
        /// <returns></returns>

        public Deploy.CustomDTO.ResultDTO DeletebrowseCommdity(Guid AppId, Guid UserId, Guid CommdityId)
        {
            base.Do(false);
            return this.DeletebrowseCommdityExt(AppId,UserId,CommdityId);
        }

    }
}
