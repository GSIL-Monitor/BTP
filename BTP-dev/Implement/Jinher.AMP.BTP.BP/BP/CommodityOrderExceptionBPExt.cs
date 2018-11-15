
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/7/27 14:41:14
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityOrderExceptionBP : BaseBP, ICommodityOrderException
    {

        /// <summary>
        ///  按条件获取订单异常日志
        ///  </summary>
        /// <param name="dto">参数实体</param>
        /// <returns></returns>
        public ResultDTO<List<CommodityOrderExceptionDTO>> GetOrderExceptionByAppIdExt(CommodityOrderExceptionParamDTO dto)
        {
            ResultDTO<List<CommodityOrderExceptionDTO>> result = new ResultDTO<List<CommodityOrderExceptionDTO>>();

            if (dto == null)
            {
                result.ResultCode = -1;
                result.Message = "参数不能为空!";
                return result;
            }

            try
            {
                var coeQuery = from coe in CommodityOrderException.ObjectSet()
                               select coe;
                if (dto.AppId != Guid.Empty)
                {
                    coeQuery = coeQuery.Where(coe => coe.AppId == dto.AppId);
                }
                if (dto.BeginTime.Date <= dto.EndTime.Date)
                {
                    //开始时间的当天的00:00:00;
                    DateTime dtBegin = dto.BeginTime.Date;
                    //结束时间的
                    DateTime dtEndOri = dto.EndTime.Date;
                    DateTime dtEnd = new DateTime(dtEndOri.Year, dtEndOri.Month, dtEndOri.Day, 23, 59, 59, 999);

                    coeQuery = coeQuery.Where(coe => coe.ExceptionTime >= dtBegin && coe.ExceptionTime <= dtEnd);
                }
                if (dto.State != -1)
                {
                    coeQuery = coeQuery.Where(coe => coe.State == dto.State);
                }

                //总行数。
                int totalCont = coeQuery.Count();
                result.Message = totalCont.ToString();

                int pageIndex = dto.PageNumber;
                int pageSize = dto.PageSize;
                var coeList = coeQuery.OrderByDescending(n => n.ExceptionTime).ThenByDescending(n => n.SubTime)
                     .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                if (coeList == null || coeList.Count() == 0)
                {
                    return result;
                }
                List<CommodityOrderExceptionDTO> listOrderExcDTO = coeList.ConvertAll<CommodityOrderExceptionDTO>(ConvertCommodityOrderException2DTO);
                result.Data = listOrderExcDTO;

            }
            catch (Exception ex)
            {
                string msg = "GetAllCommodityOrderExceptionByAppIdExt接口异常，异常信息：{0}";
                msg = string.Format(msg, ex);
            }

            return result;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderExceptionExt(Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO dto)
        {
            ResultDTO result = new ResultDTO();

            if (dto == null)
            {
                result.ResultCode = -1;
                result.Message = "参数不能为空!";
                return result;
            }

            try
            {
                var oeQuery = from coe in CommodityOrderException.ObjectSet()
                              where coe.Id == dto.Id
                              select coe;
                if (!oeQuery.Any())
                {
                    result.ResultCode = -2;
                    result.Message = "未更新任何订单异常日志!";
                    return result;
                }
                var oeFirst = oeQuery.First();
                if (dto.State != oeFirst.State && dto.State >= 0)
                {
                    oeFirst.State = dto.State;
                }
                oeFirst.Note = dto.Note;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveChanges();

                result.Message = "保存成功!";
                result.ResultCode = 0;
            }
            catch (Exception ex)
            {
                string msg = "UpdateOrderExceptionExt接口异常，异常信息：{0}";
                msg = string.Format(msg, ex);
            }

            return result;
        }

        private CommodityOrderExceptionDTO ConvertCommodityOrderException2DTO(CommodityOrderException orderExc)
        {
            CommodityOrderExceptionDTO orderExcDto = new CommodityOrderExceptionDTO();
            orderExcDto.FillWith(orderExc);
            orderExcDto.ClearingPrice = orderExc.ClearingPrice;
            orderExcDto.OrderRealPrice = orderExc.OrderRealPrice;
            //用SubId保存id,id在jquerygrid中是关键字，控件自身已使用。
            orderExcDto.SubId = orderExc.Id;
            return orderExcDto;
        }
    }
}