using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.ISV.Facade;

namespace Jinher.AMP.BTP.UI.Models
{
    public class SelfTakeStationVM
    {
        /// <summary>
        /// 获取商品是否支持自提。
        /// </summary>
        /// <param name="search"></param>
        /// <param name="rowCount"></param>
        /// <param name="invoker"></param>
        /// <returns></returns>
        public static List<SelfTakeStationSearchResultDTO> GetSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchDTO search, out int rowCount, string invoker)
        {
            try
            {
                rowCount = 0;
                SelfTakeStationFacade stsFacade = new SelfTakeStationFacade();
                var result = stsFacade.GetSelfTakeStation(search);
                if (result != null && result.SelfTakeStationList != null && result.SelfTakeStationList.Any())
                {
                    rowCount = result.Count;
                    return result.SelfTakeStationList;
                }
                return new List<SelfTakeStationSearchResultDTO>();
            }
            catch (Exception ex)
            {
                rowCount = 0;
                return null;
            }
        }

    }
}