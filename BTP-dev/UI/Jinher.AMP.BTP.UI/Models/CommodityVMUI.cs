using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.ISV.Facade;

namespace Jinher.AMP.BTP.UI.Models
{
    public class CommodityVMUI
    {

        /// <summary>
        /// 获取商品是否支持自提。
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <returns></returns>
        public static ReturnInfo<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO>> GetCommodityIsEnableSelfTakeList(string commodityIds)
        {
            ReturnInfo<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO>> ri = new ReturnInfo<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO>>();
            try
            {

                List<Guid> listGuid = new List<Guid>();
                if (string.IsNullOrWhiteSpace(commodityIds))
                {
                    ri.Code = "-1";
                    ri.IsSuccess = false;
                    return ri;
                }
                string[] cidArr = commodityIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in cidArr)
                {
                    Guid cid = Guid.Empty;
                    Guid.TryParse(s, out cid);
                    if (cid == Guid.Empty)
                    {
                        continue;
                    }
                    listGuid.Add(cid);
                }
                Jinher.AMP.BTP.ISV.Facade.CommodityFacade cf = new Jinher.AMP.BTP.ISV.Facade.CommodityFacade();
                var result = cf.GetCommodityIsEnableSelfTakeList(listGuid);
                ri.Code = "0";
                ri.IsSuccess = true;
                ri.Data = result;

                return ri;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("MobileController.GetCommodityIsEnableSelfTakeList中调用Jinher.AMP.BTP.ISV.Facade.CommodityFacade.GetCommodityIsEnableSelfTakeList。commodityIds：{0}", commodityIds), ex);

                ri.Code = "-2";
                ri.IsSuccess = false;
                return ri;
            }
        } 
    }
}
