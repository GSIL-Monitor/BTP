using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.LVP.Deploy.CustomDTO.Equipment;
using Jinher.AMP.LVP.ISV.Facade;
using Jinher.AMP.LVP.Deploy.CustomDTO.EnterpriseOperation;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class LVPSV : OutSideServiceBase<LVPFacade>
    {
        /// <summary>
        /// 获取爱尔目直播地址
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetEquipmentUrl(Guid appId)
        {
            var list = Instance.GetAppEquipmentList2(appId);
            if (list != null && list.Any())
            {
                return string.Format(CustomConfig.EquipmentUrl, appId);
            }
            return null;
        }

        /// <summary>
        /// 获取爱尔目直播地址(新)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetEquipmentUrlNew(Guid appId, Guid userId)
        {
            return string.Empty;
            var result = Instance.GetFirstPageSourcesNew(appId, userId);
            if (result != null && result.liveSources != null && result.liveSources.liveSources != null && result.liveSources.liveSources.Any())
            {
                return string.Format(CustomConfig.EquipmentUrl, appId);
            }
            return null;
        }
    }

    public class LVPFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 根据appid获取爱尔目直播列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public LiveSourcesInfoNewDTO GetFirstPageSourcesNew(Guid appId, Guid userId)
        {
            LiveSourcesInfoNewDTO result = null;
            try
            {
                LiveManageFacade facade = new LiveManageFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var requetDto = new GetLiveSourcesNewDTO()
                {
                    appId = appId,
                    categoryId = Guid.Empty,
                    pageIndex = 1,
                    pageSize = 20,
                    userId = userId
                };
                result = facade.getFirstPageSourcesNew(requetDto);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("LVPSV.GetFirstPageSourcesNew异常，AppId：" + appId + " UserId：" + userId + "，异常信息：", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据appid获取爱尔目直播列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<EquipmentDetailDTO2> GetAppEquipmentList2(Guid appId)
        {
            List<EquipmentDetailDTO2> result = null;

            try
            {
                EquipmentFacade facade = new EquipmentFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var lvpResult = facade.GetAppEquipmentList2(new GetAppEquipmentListDTO() { AppId = appId });
                if (lvpResult != null && lvpResult.IsSuccess && lvpResult.Data != null)
                {
                    result = lvpResult.Data.List;
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("LVPSV.GetAppEquipmentList2异常，异常信息：", ex);
            }
            return result;
        }
    }
}
