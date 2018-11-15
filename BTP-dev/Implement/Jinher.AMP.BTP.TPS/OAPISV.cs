using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */
    public class OAPISV : OutSideServiceBase<OAPISVFacade>
    {
        public static string GetCode(string apikey, string callerid, string time)
        {
            return Instance.GetCode(apikey, callerid, time);
        }

        public static ResultDTO CheckToken(string yjUserId, string token, ContextDTO context)
        {
            if (string.IsNullOrEmpty(yjUserId) || string.IsNullOrEmpty(token) || context == null) return null;
            return Instance.CheckToken(yjUserId, token, context);
        }
    }

    public class OAPISVFacade : OutSideFacadeBase
    {
        public string GetCode(string apikey, string callerid, string time)
        {
            var md5 = string.Format("apikey={0}&callerid={1}&time={2}", apikey, callerid, time);
            var code = MD5Helper.GetMD5(md5, Encoding.UTF8);
            return code;
        }

        [BTPAopLogMethod]
        public ResultDTO CheckToken(string yjUserId, string token, ContextDTO context)
        {
            if (string.IsNullOrEmpty(yjUserId) || string.IsNullOrEmpty(token) || context == null) return null;
            var time = TimeHelper.GetTimestampS().ToString();
            var code = GetCode(CustomConfig.OAPIApiKey, CustomConfig.OAPICallerId, time);
            return RequestHelper.CreateRequest<ResultDTO, string>(new RequestDTO<string>
            {
                ServiceUrl = string.Format("{0}?callerid={1}&time={2}&code={3}", CustomConfig.OAPICheckTokenUrl, CustomConfig.OAPICallerId, time, code),
                RequestData = SerializationHelper.JsonSerialize(new { userId = yjUserId, token = token }),
                Context = context
            });
        }
    }
}
