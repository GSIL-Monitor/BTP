using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.IUS.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    /// <summary>
    /// IUSSV接口
    /// </summary>
    public class IUSSV : OutSideServiceBase<IUSSVFacade>
    {

    }

    public class IUSSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 添加内容分享
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.IUS.Deploy.CustomDTO.ReturnInfoCDTO<ReturnIUSInfoCDTO> AddPicFromUrl(PicFromUrlCDTO dto)
        {
            Jinher.AMP.IUS.Deploy.CustomDTO.ReturnInfoCDTO<ReturnIUSInfoCDTO> returnInfoDTO = null;
            Jinher.AMP.IUS.ISV.Facade.IUSAddDataFacade iUSAddDataFacade = new IUS.ISV.Facade.IUSAddDataFacade();
            try
            {
                iUSAddDataFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfoDTO = iUSAddDataFacade.AddPicFromUrl(dto);

            }
            catch (Exception ex)
            {
                LogHelper.Warn(string.Format("IUSSV.AddPicFromUrl服务异常:发布广场异常。 dto：{0}, ContextDTO: {1}, ex: {1}", JsonHelper.JsonSerializer(dto), JsonHelper.JsonSerializer(iUSAddDataFacade.ContextDTO), ex));
            }
            return returnInfoDTO;
        }
    }
}
