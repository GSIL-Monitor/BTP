using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class ShortUrlSV : OutSideServiceBase<ShortUrlSVFacade>
    {

    }

    public class ShortUrlSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 生成短地址
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GenShortUrl(string longUrl)
        {
            try
            {
                Jinher.JAP.ShortUrl.ISV.Facade.ShortUrlManageFacade shortSV = new JAP.ShortUrl.ISV.Facade.ShortUrlManageFacade();
                shortSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var shortUrl = shortSV.GenShortUrl(longUrl);
                return shortUrl;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("ShortUrlSV.GenShortUrl异常，异常信息：{0}", ex);
                LogHelper.Error(errorMsg);
            }
            return "";
        }
    }
}
