using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{

    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class BTPFileSV : OutSideServiceBase<BTPFileSVFacade>
    {

    }

    public class BTPFileSVFacade : OutSideFacadeBase
    {
        [BTPAopLogMethod(IsLogParams = false)]
        public string UploadFile(FileDTO fileDTO)
        {
            string result = "";

            try
            {
                Jinher.JAP.BaseApp.FileServer.ISV.Facade.FileFacade filefacade = new JAP.BaseApp.FileServer.ISV.Facade.FileFacade();
                filefacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var returnInfo = filefacade.UploadFile(fileDTO);
                if (returnInfo != null)
                    result = returnInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BTPFileSV.UploadFile服务异常:获取应用信息异常。 fileDTO：{0}", fileDTO), ex);
            }
            return result;
        }

        [BTPAopLogMethod(IsLogParams = false)]
        public List<string> UploadFileList(List<FileDTO> fileDTO)
        {
            List<string> result = null;
            try
            {
                Jinher.JAP.BaseApp.FileServer.ISV.Facade.FileFacade filefacade = new JAP.BaseApp.FileServer.ISV.Facade.FileFacade();
                filefacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = filefacade.UploadFileList(fileDTO);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BTPFileSV.UploadFileList服务异常:获取应用信息异常。 fileDTO：{0}", fileDTO), ex);
            }
            return result;

        }
    }

}
