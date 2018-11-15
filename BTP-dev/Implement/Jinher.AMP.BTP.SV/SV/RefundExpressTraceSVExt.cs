
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/5/10 14:17:43
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class RefundExpressTraceSV : BaseSv, IRefundExpressTrace
    {


        /// <summary>
        /// 新增退货物流跟踪中的物流信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpressExt(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            try
            {
                var rets = RefundExpressTrace.ObjectSet().Where(t => t.OrderId == retd.OrderId);
                if (rets == null)
                {

                    LogHelper.Error(string.Format("更新退货物流跟踪数据不存在。UpdateRefundExpressExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "更新数据不存在" };
                }
                RefundExpressTrace ret = null;
                //整单退货
                if (retd.OrderItemId == null || retd.OrderItemId == Guid.Empty)
                {
                    ret = rets.FirstOrDefault();
                }
                //单商品退货
                else
                {
                    ret = rets.FirstOrDefault(r => r.OrderItemId == retd.OrderItemId);
                }
                if (ret != null)
                {
                    ret.RefundExpCo = retd.RefundExpCo;
                    ret.RefundExpOrderNo = retd.RefundExpOrderNo;
                    ret.UploadExpOrderTime = retd.UploadExpOrderTime;
                    ret.EntityState = System.Data.EntityState.Modified;
                    ContextFactory.CurrentThreadContext.SaveObject(ret);
                    var result = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (result > 0)
                    {
                        return new ResultDTO { ResultCode = 0, isSuccess = true, Message = "退货物流跟踪数据更新成功" };
                    }
                    LogHelper.Error(string.Format("更新退货物流跟踪数据失败。UpdateRefundExpressExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "退货物流跟踪数据更新失败" };
                }
                else
                {
                    LogHelper.Error(string.Format("更新退货物流跟踪数据不存在。UpdateRefundExpressExt：{0}", JsonHelper.JsonSerializer(retd)));
                    return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "更新数据不存在" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("更新退货物流跟踪数据(商家确认退款时间)异常。UpdateRefundExpressExt：{0}", JsonHelper.JsonSerializer(retd)), ex);
                return new ResultDTO { ResultCode = 1, isSuccess = false, Message = "退货物流跟踪数据更新失败" };
            }

        }

        /// <summary>
        /// 获取退货物流跟踪列表数据
        /// </summary>
        /// <returns></returns>
        public ResultDTO<List<RefundExpressTraceDTO>> GetRefundExpressTraceExt()
        {
            var result = RefundExpressTrace.ObjectSet().ToList().Select(t => t.ToEntityData()).ToList();
            return new ResultDTO<List<RefundExpressTraceDTO>> { isSuccess = true, Message = "Success", ResultCode = 0, Data = result };
        }
    }
}