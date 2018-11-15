using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.IBP.Agent
{
    /// <summary>
    /// 苏宁售后
    /// </summary>
    public class SNAfterSaleAgent : BaseBpAgent<ISNAfterSale>, ISNAfterSale
    {
       // /// <summary>
       // /// 苏宁--单品申请售后
       // /// </summary>
       // /// <param name="input"></param>
       // /// <returns></returns>
       // public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnReturnPartOrder(SNReturnPartOrderDTO input, SNFactoryDeliveryEnum ty)
       // {
       //     //定义返回值
       //     Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

       //     try
       //     {
       //         //调用代理方法
       //         result = base.Channel.SnReturnPartOrder(input,ty);

       //     }
       //     catch
       //     {
       //         //抛异常
       //         throw;
       //     }
       //     finally
       //     {
       //         //关链接
       //         ChannelClose();
       //     }            //返回结果
       //     return result;
       // }
       // /// <summary>
       // /// 苏宁--整单申请售后
       // /// </summary>
       ///// <param name="input"></param>
       // /// <returns></returns>
       // public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnApplyRejected(SNApplyRejectedDTO input, SNFactoryDeliveryEnum ty)
       // {
       //     //定义返回值
       //     Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

       //     try
       //     {
       //         //调用代理方法
       //         result = base.Channel.SnApplyRejected(input,ty);

       //     }
       //     catch
       //     {
       //         //抛异常
       //         throw;
       //     }
       //     finally
       //     {
       //         //关链接
       //         ChannelClose();
       //     }            //返回结果
       //     return result;
       // }


       /// <summary>
       /// 获取苏宁订单详情
       /// </summary>
       /// <param name="reqDto"></param>
       /// <returns></returns>
       public List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> GetSNOrderItemList(Jinher.AMP.BTP.Deploy.SNOrderItemDTO input)
       {
           //定义返回值
           List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> result;

           try
           {
               //调用代理方法
               result = base.Channel.GetSNOrderItemList(input);

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
