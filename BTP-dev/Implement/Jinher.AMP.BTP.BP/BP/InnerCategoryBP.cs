﻿
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/9/16 13:47:11
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class InnerCategoryBP : BaseBP, IInnerCategory
    {

        /// <summary>
        /// 添加同级类别
        /// </summary>
        /// <param name="categoryName">分类名称</param>
        /// <param name="appId">卖家ID</param>
        /// <param name="targetId">目标类别ID</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCategory(string categoryName, System.Guid appId, System.Guid targetId)
        {
            base.Do();
            return this.AddCategoryExt(categoryName, appId, targetId);
        }
        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategories(System.Guid appId)
        {
            base.Do();
            return this.GetCategoriesExt(appId);
        }
        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategories2(System.Guid appId)
        {
            base.Do();
            return this.GetCategories2Ext(appId);
        }
        /// <summary>
        /// 删除卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategory(System.Guid appId, System.Guid myId)
        {
            base.Do();
            return this.DeleteCategoryExt(appId, myId);
        }
        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name">类别名称</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory(System.Guid appId, string name, System.Guid myId)
        {
            base.Do();
            return this.UpdateCategoryExt(appId, name, myId);
        }
        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name">类别名称</param>
        /// <param name="myId">被操作类别ID</param>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory2(System.Guid appId, string name, System.Guid myId, string icon)
        {
            base.Do();
            return this.UpdateCategory2Ext(appId, name, myId, icon);
        }
        /// <summary>
        /// 升级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO LevelUpCategory(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            base.Do();
            return this.LevelUpCategoryExt(appId, targetId, myId);
        }
        /// <summary>
        /// 降级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO LevelDownCategory(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            base.Do();
            return this.LevelDownCategoryExt(appId, targetId, myId);
        }
        /// <summary>
        /// 升序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpCategory(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            base.Do();
            return this.UpCategoryExt(appId, targetId, myId);
        }
        /// <summary>
        /// 降序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DownCategory(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            base.Do();
            return this.DownCategoryExt(appId, targetId, myId);
        }
        /// <summary>
        /// 拖动类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DragCategory(System.Guid appId, System.Guid targetId, System.Guid myId, string moveType)
        {
            base.Do();
            return this.DragCategoryExt(appId, targetId, myId, moveType);
        }
        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddChildCategory(string name, System.Guid targetId, System.Guid appId)
        {
            base.Do();
            return this.AddChildCategoryExt(name, targetId, appId);
        }
        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddChildCategory2(string name, System.Guid targetId, System.Guid appId, string icon)
        {
            base.Do();
            return this.AddChildCategory2Ext(name, targetId, appId, icon);
        }
        /// <summary>
        /// 创建初始类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatCategory(System.Guid appId)
        {
            base.Do();
            return this.CreatCategoryExt(appId);
        }
        /// <summary>
        /// 创建初始类别（三级分类）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatCategory2(System.Guid appId)
        {
            base.Do();
            return this.CreatCategory2Ext(appId);
        }
        /// <summary>
        /// 校验app是否显示search菜单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> CheckIsShowSearchMenu(Jinher.AMP.BTP.Deploy.CustomDTO.CategorySearchDTO search)
        {
            base.Do();
            return this.CheckIsShowSearchMenuExt(search);
        }
        /// <summary>
        /// 保存是否显示菜单标志
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateIsShowSearchMenu(Jinher.AMP.BTP.Deploy.CustomDTO.CategorySearchDTO search)
        {
            base.Do();
            return this.UpdateIsShowSearchMenuExt(search);
        }

        /// <summary>
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetZshCategories()
        {
            base.Do(false);
            return this.GetZshCategoriesExt();
        }
    }
}
