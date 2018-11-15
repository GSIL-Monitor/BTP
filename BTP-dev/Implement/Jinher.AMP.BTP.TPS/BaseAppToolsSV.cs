using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Jinher.AMP.BTP.Common;
using System.Globalization;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO;


namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class BaseAppToolsSV : OutSideServiceBase<BaseAppToolsSVFacade>
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="url">要构建二维码的推广码的短地址</param>
        /// <param name="fileImgUrl">临时图片名称，可以空</param>
        /// <returns></returns>
        public static string GenerateQrCode(string url, string fileImgUrl = null)
        {
            string result = "";
            WebClient mywebclient = null;
            try
            {
                //用户自已上传图片生成链接图片
                if (!string.IsNullOrEmpty(fileImgUrl))
                {
                    //网络图片读取
                    mywebclient = new WebClient();
                    var imgfile = mywebclient.DownloadData(fileImgUrl);

                    var qRCodeWithIconDto = new Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO
                        {
                            IconDate = imgfile,
                            Source = url
                        };

                    //生成带图片的二维码
                    string codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCodeWithIcon(qRCodeWithIconDto);
                    result = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;
                }
                else
                {
                    //系统默认生成的不带图片的二维码                    
                    string codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCode(url);
                    result = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(string.Format("生成二维码异常。url：{0}，fileImgUrl：{1}", url, fileImgUrl), ex);
            }
            finally
            {
                if (mywebclient != null)
                {
                    mywebclient.Dispose();
                }
            }
            return result;
        }
    }

    public class BaseAppToolsSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 生成二维码 serverpath = "http://tools.iuoooo.com/Jinher.JAP.BaseApp.Tools.SV.QRCodeManageSV.svc/GenQRCodeWithIcon";
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GenQRCode(string source)
        {
            string result = "";
            try
            {
                string codepath = string.Empty;
                Jinher.JAP.BaseApp.Tools.ISV.Facade.QRCodeManageFacade toolFacade = new JAP.BaseApp.Tools.ISV.Facade.QRCodeManageFacade();
                toolFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                codepath = toolFacade.GenQRCode(source);
                result = codepath;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BaseAppToolsSV.GenQRCode生成二维码服务异常。source：{0}", JsonHelper.JsonSerializer(source)), ex);
            }
            return result;
        }
        [BTPAopLogMethod]
        public string GenQRCodeWithIcon(QRCodeWithIconDTO qRCodeWithIconDTO)
        {
            string result = "";
            try
            {
                string codepath = string.Empty;
                Jinher.JAP.BaseApp.Tools.ISV.Facade.QRCodeManageFacade toolFacade = new JAP.BaseApp.Tools.ISV.Facade.QRCodeManageFacade();
                toolFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                codepath = toolFacade.GenQRCodeWithIcon(qRCodeWithIconDTO);
                result = codepath;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BaseAppToolsSV.GenQRCodeWithIcon生成二维码服务异常。qRCodeWithIconDTO：{0}", JsonHelper.JsonSerializer(qRCodeWithIconDTO)), ex);
            }
            return result;
        }
    }
}
