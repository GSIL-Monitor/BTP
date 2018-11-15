
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/3/19 10:22:02
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class DeliveryAddressFacade : BaseFacade<IDeliveryAddress>
    {

        /// <summary>
        /// 添加收货地址
        /// </summary>
        /// <param name="deliveryAddressDTO">收货地址实体</param>
        public void AddDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO deliveryAddressDTO)
        {
            base.Do();
            this.Command.AddDeliveryAddress(deliveryAddressDTO);
        }
        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        public void DeleteDeliveryAddress(System.Guid id)
        {
            base.Do();
            this.Command.DeleteDeliveryAddress(id);
        }
        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="commodityDTO">地址实体</param>
        public void UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO commodityDTO)
        {
            base.Do();
            this.Command.UpdateDeliveryAddress(commodityDTO);
        }
        /// <summary>
        /// 查询用户收货地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.DeliveryAddressDTO> GetAllDeliveryAddress(System.Guid userId)
        {
            base.Do();
            return this.Command.GetAllDeliveryAddress(userId);
        }
        /// <summary>
        /// 查询一条收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.DeliveryAddressDTO GetDeliveryAddress(System.Guid id)
        {
            base.Do();
            return this.Command.GetDeliveryAddress(id);
        }
    }
}