using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.ADM.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */
    public class ADMSV : OutSideServiceBase<ADMFacade>
    {
    }

    /// <summary>
    /// 外部接口
    /// </summary>
    public class ADMFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取竞价排名的广告
        /// </summary>
        /// <param name="dto">组件要提供的数据</param>
        /// <returns>要显示的广告信息</returns>
        [BTPAopLogMethod]
        public List<ADInfo4ComponentCDTO> GetBiddingAD(BiddingAdCDTO dto)
        {
            List<ADInfo4ComponentCDTO> aDInfo = new List<ADInfo4ComponentCDTO>();
            try
            {
                Jinher.AMP.ADM.ISV.Facade.AdComponentFacade adComponentFacade = new ADM.ISV.Facade.AdComponentFacade();
                adComponentFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                aDInfo = adComponentFacade.GetBiddingAD(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ADMSV.GetBiddingAD服务异常。dto：{0}", dto), ex);
            }
            return aDInfo;
        }
        /// <summary>
        ///  产生消费后
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public CpsCostReturnCDTO EffectivePromote(CpsCostParamCDTO dto)
        {
            CpsCostReturnCDTO cpsDTO = new CpsCostReturnCDTO();
            try
            {
                Jinher.AMP.ADM.ISV.Facade.ProductWallFacade productWallFacade = new ADM.ISV.Facade.ProductWallFacade();
                productWallFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                cpsDTO = productWallFacade.EffectivePromote(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ADMSV.EffectivePromote服务异常。dto：{0}", dto), ex);
            }
            return cpsDTO;
        }
    }
}
