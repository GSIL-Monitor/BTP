
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/1/8 9:01:50
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jinher.AMP.Apm.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class AppSetFacade : BaseFacade<IAppSet>
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.QryCommodityDTO qryCommodityDTO)
        {
            base.Do();
            return this.Command.GetCommodityList(qryCommodityDTO);
        }

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(Guid appId)
        {
            base.Do();
            return this.Command.GetCategory(appId);
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
            base.Do();
            return this.Command.GetWantCommodity(want, pageIndex, pageSize);
        }

        /// <summary>
        /// 厂家直营app查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO GetAppSet(Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO search)
        {
            base.Do();
            return this.Command.GetAppSet(search);
        }
        /// <summary>
        /// 根据分类Id获取该分类下的app列表
        /// </summary>
        /// <param name="search"></param>
        public List<AppSetAppDTO> GetCategoryAppList(AppSetSearchDTO search)
        {
            base.Do();
            return this.Command.GetCategoryAppList(search);
        }

        /// <summary>
        /// 获取正品会“我的”，各栏目数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfoCountDTO GetUserInfoCount(Guid userId, Guid esAppId)
        {
            base.Do();
            return this.Command.GetUserInfoCount(userId, esAppId);
        }

        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns>结果</returns>        
        public ResultDTO RemoveAppInZPHCache()
        {
            base.Do();
            return this.Command.RemoveAppInZPHCache();
        }
        /// <summary>
        /// 根据独立电商是否属于平台获取商品列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityListV2(search);
        }

        /// <summary>
        /// 浏览过的店铺（20个）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> GetBrowseAppInfo(Guid userId, Guid appId)
        {
             
            base.Do();
            return this.Command.GetBrowseAppInfo(userId, appId);
        
        }

        /// <summary>
        /// 分页获取浏览商品记录
        /// </summary>
        /// <param name="par"></param>
        public List<BTP.Deploy.CustomDTO.CommodityListCDTO> GetBrowseCommdity(BrowseParameter par)
        {
            base.Do();
            return this.Command.GetBrowseCommdity(par);
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
            base.Do();
            return this.Command.DeletebrowseCommdity(AppId, UserId, CommdityId);
        }
    }
}
