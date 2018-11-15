
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/6/6 11:44:09
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class InvoiceAgent : BaseBpAgent<IInvoice>, IInvoice
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSettingDTO> GetInvoiceSetting(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSettingDTO> result;
            int xx = 0;
            xx++;
            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceSetting(search);

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

        public Deploy.CustomDTO.ResultDTO<List<InvoiceInfoDTO>> GetInvoiceInfoList(Guid appId, Guid userId, int category)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<InvoiceInfoDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetInvoiceInfoList(appId, userId, category);

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
