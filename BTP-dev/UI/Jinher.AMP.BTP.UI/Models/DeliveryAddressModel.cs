using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Models
{
    public class DeliveryAddressModel
    {
        /// <summary>
        ///获取收货地址信息。
        /// </summary>
        /// <param name="addressId">地址id</param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public static AddressSDTO GetDeliveryAddressByAddressId(System.Guid addressId, System.Guid appId, string invoker)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
                var result = facade.GetDeliveryAddressByAddressId(addressId, appId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade.GetDeliveryAddressByAddressId接口异常。addressId：{1}，appId：{2}", invoker, addressId, appId), ex);
            }
            return null;
        }


        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="IsDefault">是否默认收货地址</param>
        /// <returns></returns>
        public static List<AddressSDTO> GetDeliveryAddressList(System.Guid userId, System.Guid appId, int IsDefault, string invoker)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
                var result = facade.GetDeliveryAddressList(userId, appId, IsDefault);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade.GetDeliveryAddressList接口异常userId：{1}，appId：{2}，IsDefault：{3}",invoker,userId,appId,IsDefault),ex);
            }
            return null;
        }

    }
}