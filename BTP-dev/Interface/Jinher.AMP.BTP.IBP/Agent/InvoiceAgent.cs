
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/5/29 11:37:11
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class InvoiceAgent : BaseBpAgent<IInvoice>, IInvoice
    {
        /// <summary>
        /// 查询发票信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceResultDTO> GetInvoiceInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceInfoList(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 保存增值税发票资质信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveVatInvoiceProof(Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO VatInvoiceP)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveVatInvoiceProof(VatInvoiceP);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 设置发票类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInvoiceCategory(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetInvoiceCategory(model);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 修改发票
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateInvoice(List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceUpdateDTO> list)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateInvoice(list);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 获取全局设置的发票类型
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO> GetInvoiceCategory(Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceCategory(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
       
        /// 显示增值税发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProof(Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ShowVatInvoiceProof(userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// 显示增值税发票信息 金采支付专用
        /// </summary>
        /// <param name="jcActivityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProofII(Guid jcActivityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ShowVatInvoiceProofII(jcActivityId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// 导出发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<InvoiceExportDTO> GetInvoiceExport(InvoiceExportDTO search)
        {
            //定义返回值
            List<InvoiceExportDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceExport(search);
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        /// <summary>
        /// 获取导出的电子发票的详细数据
        /// </summary>
        public List<ElectronicInvoiceDTO> GetInvoiceExportDetail(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            //定义返回值
            List<ElectronicInvoiceDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceExportDetail(search);
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


    }
}
