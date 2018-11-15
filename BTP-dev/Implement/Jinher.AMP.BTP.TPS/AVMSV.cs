using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.AVM.Deploy.CustomDTO;
using Jinher.AMP.AVM.ISV.Facade;
using Jinher.AMP.App.ISV.Facade;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.App.Deploy.Enum;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Cache;

namespace Jinher.AMP.BTP.TPS
{

    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    /// <summary>
    /// 会员tps
    /// </summary>
    public class AVMSV : OutSideServiceBase<AVMSVFacade>
    {
        /// <summary>
        /// 获取会员折扣信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static VipPromotionDTO GetVipIntensity(Guid appId, Guid userId)
        {
            VipPromotionDTO result = new VipPromotionDTO(appId, userId);
            if (appId == Guid.Empty)
                return result;
            result.IsVipActive = Instance.CheckPrivilegeByCode(appId);
            if (userId == Guid.Empty)
                return result;

            if (result.IsVipActive)
            {
                var privilegeInfoDTO = Instance.GetPrivilegeInfoByUserId(appId, userId);
                if (privilegeInfoDTO != null && privilegeInfoDTO.RegistPrivileges != null && privilegeInfoDTO.RegistPrivileges.Any())
                {
                    decimal intensity;
                    var registPrivilege = privilegeInfoDTO.RegistPrivileges.FirstOrDefault(c => c.RegistPrivilegeCode == AVMSVFacade.VIPZheKou);
                    if (registPrivilege != null && decimal.TryParse(registPrivilege.PrivilegeValue, out intensity) && intensity > 0 && intensity < 10)
                    {
                        result.Intensity = intensity;
                        result.IsVip = true;
                        result.AppId = appId;
                        result.UserId = userId;
                        result.VipLevelDesc = privilegeInfoDTO.LevelName;
                        result.VipLevlelId = privilegeInfoDTO.LevelId;
                    }
                }
            }
            return result;
        }
        public static Dictionary<Guid, VipPromotionDTO> GetVipIntensities(List<Guid> appIds, Guid userId)
        {
            Dictionary<Guid, VipPromotionDTO> result = new Dictionary<Guid, VipPromotionDTO>();
            if (appIds == null || !appIds.Any() || userId == Guid.Empty)
                return result;
            foreach (var appId in appIds)
            {
                result.Add(appId, GetVipIntensity(appId, userId));
            }
            return result;
        }
    }

    public class AVMSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 会员打折体系特权编码
        /// </summary>
        internal const string VIPZheKou = "VIPZheKou";
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public GetPrivilegeInfoDTO GetPrivilegeInfoByUserId(Guid appId, Guid userId)
        {
            try
            {
                VIPQueryFacade facade = new VIPQueryFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var resultList = facade.GetPrivilegeInfoByUserId(appId, new List<Guid> { userId });
                if (resultList != null && resultList.Count == 1)
                    return resultList[0];
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AVMSV.GetPrivilegeInfoByUserId服务异常:获取会员信息异常。 appId：{0},userId:{1}", appId, userId), ex);
                return new GetPrivilegeInfoDTO();
            }
            return new GetPrivilegeInfoDTO();
        }
        /// <summary>
        /// 判断app是否启用会员折扣体系
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool CheckPrivilegeByCode(Guid appId)
        {
            bool result = false;
            try
            {
                //VIPQueryFacade facade = new VIPQueryFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                //result = facade.CheckPrivilegeByCode(appId, VIPZheKou);
                result = false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AVMSV.CheckPrivilegeByCode服务异常:获取会员信息异常。 appId：{0}", appId), ex);
            }
            return result;
        }
    }
}
