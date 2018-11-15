using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.EBC.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.EBC.ISV.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    /// <summary>
    /// 查询组织名称和图标
    /// </summary>
    public class EBCSV : OutSideServiceBase<EBCSVFacade>
    {
        /// <summary>
        /// 获取组织创建者id
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static Guid GetOrgCreateUser(Guid orgId)
        {
            OrganizationForeignDTO forInfo = Instance.GetOrgForeignInfoById(orgId);
            if (forInfo != null && forInfo.SubId != Guid.Empty)
                return forInfo.SubId;
            return orgId;
        }

        /// <summary>
        /// 获得所有我创建的账户（创建的组织和当前个人账户）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Guid> GetMyCreateAccountList(Guid userId)
        {
            List<Guid> result = new List<Guid>();
            if (userId == Guid.Empty)
                return result;
            result.Add(userId);
            var orgList = Instance.GetMyCreateOrgList(userId);
            if (orgList != null && orgList.Any())
                orgList.ForEach(c => result.Add(c.Id));
            return result;
        }


        /// <summary>
        /// 获取应用下有“订单管理”菜单权限的用户列表
        /// </summary>
        /// <param name="applicationDTO"></param>
        /// <returns></returns>
        public static List<Guid> GetOrderMenuUsers(Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO)
        {
            List<Guid> result = new List<Guid>();
            if (applicationDTO != null && applicationDTO.OwnerId != null && applicationDTO.OwnerId.Value != Guid.Empty)
            {
                //定制应用和免费应用的“订单管理”菜单id不一样
                string orderMenuId = applicationDTO.TemplateId == 8 ? "40038046" : "40030004";

                if (applicationDTO.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Personal)
                {
                    result.Add(applicationDTO.OwnerId.Value);
                }
                else if (applicationDTO.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Org)
                {
                    result = Instance.GetRoleUserInfo(applicationDTO.OwnerId.Value, orderMenuId, applicationDTO.Id);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据组织帐号查找组织Id
        /// </summary>
        /// <param name="iwCode">组织帐号</param>
        /// <returns>组织Id</returns>
        public static Guid GetOrgIdByIwCode(string iwCode)
        {
            return Instance.GetOrgIdByIwCode(iwCode);
        }

        /// <summary>
        /// 根据组织ID查找组织信息
        /// </summary>

        public static OrgInfoDTO GetOrgInfoById(Guid iwId)
        {
            return Instance.GetOrgInfoById(iwId);
        }
    }

    public class EBCSVFacade : OutSideFacadeBase
    {
        [BTPAopLogMethod]
        public List<UserOrganizationDTO> GetOrgNameIconByIdList(List<Guid> orgIds)
        {
            List<Jinher.AMP.EBC.Deploy.CustomDTO.UserOrganizationDTO> userOrganizationDTO = null;
            try
            {
                OrganizationQueryFacade organizationQueryFacade = new OrganizationQueryFacade();
                organizationQueryFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                userOrganizationDTO = organizationQueryFacade.GetOrgNameIconByIdList(orgIds);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("EBCSV.GetOrgNameIconByIdList服务异常:获取应用信息异常。 orgIds：{0}", orgIds), ex);
            }
            return userOrganizationDTO;
        }
        /// <summary>
        /// 通过组织Id和权限Code获取有此权限的UserId集合
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Guid> GetUserIdsByOrgIdAndCode(Guid OrgId, string Code)
        {
            List<Guid> userOrganizationDTO = null;
            try
            {
                OrganizationQueryFacade organizationQueryFacade = new OrganizationQueryFacade();
                organizationQueryFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                userOrganizationDTO = organizationQueryFacade.GetUserIdsByOrgIdAndCode(OrgId, Code);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("EBCSV.GetUserIdsByOrgIdAndCode服务异常:获取应用信息异常。 OrgId：{0},Code{1}", OrgId, Code), ex);
            }
            return userOrganizationDTO;
        }

        /// <summary>
        /// 获取组织基本信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public OrganizationForeignDTO GetOrgForeignInfoById(Guid orgId)
        {
            if (orgId == Guid.Empty)
                return null;
            OrganizationQueryFacade organizationQueryFacade = new OrganizationQueryFacade();
            organizationQueryFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            return organizationQueryFacade.GetOrgForeignInfoById(orgId);
        }
        /// <summary>
        /// 获取我创建的组织列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<UserOrganizationDTO> GetMyCreateOrgList(Guid userId)
        {
            if (userId == Guid.Empty)
                return null;
            OrganizationQueryFacade organizationQueryFacade = new OrganizationQueryFacade();
            organizationQueryFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            return organizationQueryFacade.GetMyCreateOrgList(userId);
        }
        /// <summary>
        /// 通过职员Id、组织Id、应用Id、权限编码判断是否有权限
        /// </summary>
        /// <param name="userId">职员Id</param>
        /// <param name="orgId">组织Id</param>
        /// <param name="appId">应用Id</param>
        /// <param name="featureCode">权限编码</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool GetHasFeatureByCode(Guid userId, Guid orgId, Guid appId, string featureCode)
        {
            bool result = false;
            try
            {
                Jinher.AMP.EBC.IBP.Facade.RoleFeatureFacade roleBP = new EBC.IBP.Facade.RoleFeatureFacade();
                roleBP.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = roleBP.HasFeature(userId, orgId, appId.ToString(), featureCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("通过职员Id、组织Id、应用Id、权限编码判断是否有权限HasFeature异常。userId：{0}，orgId：{1}，appId：{2}，featureCode：{3}", userId, orgId, appId, featureCode), ex);
            }
            return result;
        }
        /// <summary>
        /// 根据组织Id、菜单Id、AppId查询有权限的用户Id
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="menuId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Guid> GetRoleUserInfo(Guid ownerId, string menuId, Guid appId)
        {

            Jinher.AMP.EBC.IBP.Facade.RoleMenuFacade roleMenuBP = new EBC.IBP.Facade.RoleMenuFacade();
            roleMenuBP.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            var result = roleMenuBP.GetRoleUserInfo(ownerId, menuId, appId) ?? new List<Guid>();
            result = result.Distinct().ToList();
            return result;
        }
        /// <summary>
        ///  通过用户Id和权限Code获取此用户在哪些组织内有此权限
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Guid> GetOrgIdsByUserIdAndCode(Guid UserId, string Code)
        {
            List<Guid> getOrgIdsByUserIdAndCode = new List<Guid>();
            try
            {
                Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade orgDetail = new Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade();
                orgDetail.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                List<Guid> orgIdList = orgDetail.GetOrgIdsByUserIdAndCode(UserId, Code);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("EBCSV.GetOrgIdsByUserIdAndCode服务异常:获取应用信息异常。 UserId：{0},Code{1}", UserId, Code), ex);
            }
            return getOrgIdsByUserIdAndCode;
        }

        [BTPAopLogMethod]
        public Guid GetOrgIdByIwCode(string iwCode)
        {
            try
            {
                Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade orgDetail = new Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade();
                orgDetail.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                return orgDetail.GetOrgIdByIWCode(iwCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("EBCSV.GetOrgIdByIWCode服务异常:获取应用信息异常。 IWCode：{0}", iwCode), ex);
            }
            return Guid.Empty;
        }

        [BTPAopLogMethod]
        public OrgInfoDTO GetOrgInfoById(Guid iwId)
        {
            try
            {
                Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade orgDetail = new Jinher.AMP.EBC.ISV.Facade.OrganizationQueryFacade();
                orgDetail.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                return orgDetail.GetOrgInfoById(iwId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("EBCSV.GetOrgIdByIWCode服务异常:获取应用信息异常。 IWId：{0}", iwId), ex);
            }
            return null;
        }
    }


}
