using System;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
 * 请注意：！！！！！！！！！！！！！！！
 * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
 */
    /// <summary>
    /// 电子发票访问易捷专线站点相关接口
    /// </summary>
    public class YJBJMQSV : OutSideServiceBase<YJBJMQFacade>
    {
        /// <summary>
        /// 调用开票接口保存相关的返回信息
        /// </summary>
        public static Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO CreateInvoic(string strXml)
        {
            if (string.IsNullOrEmpty(strXml))
            {
                return new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            }
            var result = Instance.CreateInvoic(strXml);
            if (result == null)
            {
                return new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            }
            return result;
        }

        /// <summary>
        /// 调用开票接口保存相关的返回信息
        /// </summary>
        public static Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO DownloadInvoic(string strXml)
        {
            if (string.IsNullOrEmpty(strXml))
            {
                return new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            }
            var result = Instance.DownloadInvoic(strXml);
            if (result == null)
            {
                return new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            }
            return result;
        }

        /// <summary>
        /// 数据发送到MQ
        /// </summary>
        /// <param name="queueName">mq队列名</param>
        /// <param name="json">json格式数据</param>
        /// <returns></returns>
        public static bool SendToMq(string queueName, string json)
        {
            return Instance.SendToMq(queueName, json);
        }
    }

    public class YJBJMQFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 调用开票接口保存相关的返回信息
        /// </summary>
        [BTPAopLogMethod]
        public Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO CreateInvoic(string strXml)
        {
            Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO result = new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO();
            try
            {
                Jinher.AMP.YJBJMQ.ISV.Facade.InvoicManageFacade facade = new YJBJMQ.ISV.Facade.InvoicManageFacade();
                result = facade.CreateInvoic(strXml);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBJMQ.CreateInvoic服务异常，调用开票接口保存相关的返回信息。 输入：" + strXml, ex);
            }
            if (result != null && result.isSuccess)
            {
                return result;
            }
            LogHelper.Error("YJBJMQ.CreateInvoic服务失败，调用开票接口保存相关的返回信息。 输入：" + strXml + "，返回：" + JsonHelper.JsonSerializer(result));
            return null;
        }

        /// <summary>
        /// 下载电子发票
        /// </summary>
        [BTPAopLogMethod]
        public Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO DownloadInvoic(string strXml)
        {
            Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO result = new Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO();
            try
            {
                Jinher.AMP.YJBJMQ.ISV.Facade.InvoicManageFacade facade = new YJBJMQ.ISV.Facade.InvoicManageFacade();
                result = facade.DownloadInvoic(strXml);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBJMQ.DownloadInvoic服务异常，调用开票接口保存相关的返回信息。 输入：" + strXml, ex);
            }
            if (result != null && result.isSuccess)
            {
                return result;
            }
            LogHelper.Error("YJBJMQ.DownloadInvoic服务失败，调用开票接口保存相关的返回信息。 输入：" + strXml + "，返回：" + JsonHelper.JsonSerializer(result));
            return null;
        }

        /// <summary>
        /// 数据发送到MQ
        /// </summary>
        /// <param name="queueName">mq队列名</param>
        /// <param name="json">json格式数据</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool SendToMq(string queueName, string json)
        {
            try
            {
                return new YJBJMQ.ISV.Facade.QueueFacade().SendToMq(queueName, json);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YJBJMQ.SendToMq异常，输入:queueName={0},json={1}", queueName, json), ex);
            }
            return false;
        }
    }
}