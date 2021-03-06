﻿
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/12 20:29:49
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CategorySV : BaseSv, ICategory
    {

        /// <summary>
        /// 获取商品分类
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CategorySV.svc/GetCategory
        /// </para>
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(System.Guid appId)
        {
            base.Do(false);
            return this.GetCategoryExt(appId);

        }
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategory2(System.Guid appId)
        {
            base.Do(false);
            return this.GetCategory2Ext(appId);

        }
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="levelCount">分类级别</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategoryByDrawer(System.Guid appId, out int levelCount)
        {
            base.Do(false);
            return this.GetCategoryByDrawerExt(appId, out levelCount);

        }
        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            base.Do(false);
            return this.GetCommodityListExt(commodityListInfer);

        }
        /// <summary>
        /// 校验app是否显示search菜单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> CheckIsShowSearchMenu(Jinher.AMP.BTP.Deploy.CustomDTO.CategorySearchDTO search)
        {
            base.Do(false);
            return this.CheckIsShowSearchMenuExt(search);

        }
        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCache2DTO> GetCacheCateGories(System.Guid appid)
        {
            base.Do(false);
            return this.GetCacheCateGoriesExt(appid);

        }
        /// <summary>
        /// 删除指定“电商馆”下applist下的商品分类关系
        /// </summary>
        /// <param name="belongTo">电商馆APPId</param>
        /// <param name="appList">applist</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommodityCategory(System.Guid belongTo, System.Collections.Generic.List<System.Guid> appList)
        {
            base.Do(false);
            return this.DeleteCommodityCategoryExt(belongTo, appList);

        }
        /// <summary>
        /// 分页获取电商馆下商品
        /// </summary>
        /// <param name="comdtySearch4SelCdto"></param>
        /// <param name="comdtyCount"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityLisByBeLongTo(Jinher.AMP.ZPH.Deploy.CustomDTO.ComdtySearch4SelCDTO comdtySearch4SelCdto, out int comdtyCount)
        {
            base.Do(false);
            return this.GetCommodityLisByBeLongToExt(comdtySearch4SelCdto, out comdtyCount);

        }
        /// <summary>
        /// 获取应用的一级商品分类
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.CategorySV.svc/GetCategoryL1
        /// </para>
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1(System.Guid appId)
        {
            base.Do(false);
            return this.GetCategoryL1Ext(appId);

        }
        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            base.Do(false);
            return this.GetCommodityListV2Ext(commodityListInfer);

        }
        /// <summary>
        /// 分页获取分类下筛选商品
        /// </summary>
        /// <param name="commodityListInfer"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            base.Do(false);
            return this.GetCommodityFilterListExt(commodityListInfer);

        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterListSecond(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            base.Do(false);
            return this.GetCommodityFilterListSecondExt(commodityListInfer);
        }
        /// <summary>
        /// 获取一级分类下的品牌及广告信息
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetBrandAndAdvertise(System.Guid CategoryID)
        {
            base.Do(false);
            return this.GetBrandAndAdvertiseExt(CategoryID);

        }

        /// <summary>
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> GetZshCategories()
        {
            base.Do(false);
            return this.GetZshCategoriesExt();
        }
    }
}
