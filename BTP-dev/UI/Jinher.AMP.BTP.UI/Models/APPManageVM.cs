using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Models
{
    public class APPManageVM
    {
        public static List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> GetAppSet(string invoker)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.AppSetFacade appSetFa = new Jinher.AMP.BTP.ISV.Facade.AppSetFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO appSearch = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO()
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue
                };
                var applist = appSetFa.GetAppSet(appSearch);
                return applist.AppList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用接口Jinher.AMP.BTP.ISV.Facade.AppSetFacade.GetAppSet异常。invoker：{0}", invoker), ex);
            }
            return null;
        }

        /// <summary>
        /// 是否显示商品分类树
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public static bool GetIsShowCategoryTree(Guid appId)
        {
            //定制应用
            bool isFit = Jinher.AMP.BTP.TPS.APPBP.IsFittedApp(appId);
            if (isFit)
            {
                bool b = Jinher.AMP.BTP.TPS.BACBP.CheckCommodityCategory(appId);
                if (b)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
