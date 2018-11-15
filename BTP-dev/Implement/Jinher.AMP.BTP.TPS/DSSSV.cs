using System;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.DSS.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class DSSSV : OutSideServiceBase<DSSSVFacade>
    {

    }

    public class DSSSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取行为记录js的url
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GetBehaviorRecordUrl()
        {
            string reT = string.Empty;
            try
            {
                Jinher.AMP.DSS.ISV.Facade.BehaviorRecordFacade facade = new DSS.ISV.Facade.BehaviorRecordFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                reT = facade.GetBehaviorRecordUrl();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("DSSSV.GetBehaviorRecordUrl服务异常:获取应用信息异常。 Exception：{0}", ex));
            }
            return reT;
        }

        /// <summary>
        /// 获取应用统计信息
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppUserPVInfo GetAppUserPV(string date, Guid appId)
        {
            try
            {
                Jinher.AMP.DSS.ISV.Facade.DssFacade facade = new DSS.ISV.Facade.DssFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var result = facade.GetAppUserPV(date, appId.ToString());
                if (result != null) return result.Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DSSSV.GetAppUserPV服务异常", ex);
            }
            return null;
        }
    }
}
