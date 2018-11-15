using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.LBP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class LBPSV : OutSideServiceBase<LBPSVFacade>
    {    
        /// <summary>
        /// 获取店铺轮播图片
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ReturnInfoDTO<Jinher.AMP.LBP.Deploy.CustomDTO.LBListReturnDTO> GetLBList(Jinher.AMP.LBP.Deploy.CustomDTO.LBListGetDTO arg)
        {
            return Instance.GetLBList(arg);
        }
    }

    public class LBPSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取店铺轮播图片
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<Jinher.AMP.LBP.Deploy.CustomDTO.LBListReturnDTO> GetLBList(Jinher.AMP.LBP.Deploy.CustomDTO.LBListGetDTO arg)
        {
            ReturnInfoDTO<Jinher.AMP.LBP.Deploy.CustomDTO.LBListReturnDTO> LBList = new ReturnInfoDTO<LBListReturnDTO>();
            try
            {
                AMP.LBP.ISV.Facade.QueryFacade facade = new LBP.ISV.Facade.QueryFacade();
                LBList= facade.GetLBList(arg);
            }
            catch (Exception ex)
            {
                LogHelper.Error("LBPSV.GetLBList获取轮播图片异常:", ex);
            }
            return LBList;
        }
    }

}
