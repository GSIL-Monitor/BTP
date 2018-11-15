using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.UI.Models
{
    public class OrderExceptionVM
    {
        public static Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> GetOrderExceptionByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderExceptionParamDTO dto)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> result = null;
            try
            {
                CommodityOrderExceptionFacade orderExcFacade = new CommodityOrderExceptionFacade();
                result = orderExcFacade.GetOrderExceptionByAppId(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用接口Jinher.AMP.BTP.IBP.Facade.CommodityOrderExceptionFacade.GetAllCommodityOrderExceptionByAppId异常。dto：{1}",dto.Invoker, JsonHelper.JsonSerializer(dto)), ex);

                result = new Deploy.CustomDTO.ResultDTO<List<Deploy.CommodityOrderExceptionDTO>>();
                result.ResultCode = 1;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }


        public static Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderException(CommodityOrderExceptionDTO dto,string invoker)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = null;
            try
            {
                CommodityOrderExceptionFacade orderExcFacade = new CommodityOrderExceptionFacade();
                result = orderExcFacade.UpdateOrderException(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用接口Jinher.AMP.BTP.IBP.Facade.CommodityOrderExceptionFacade.UpdateOrderException异常。dto：{1}",invoker, JsonHelper.JsonSerializer(dto)), ex);

                result = new Deploy.CustomDTO.ResultDTO();
                result.ResultCode = -5;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }
    }
}