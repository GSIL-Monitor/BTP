/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 10:22:04
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
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DeliveryAddressBP : BaseBP, IDeliveryAddress
    {

        /// <summary>
        /// 添加收货地址
        /// </summary>
        /// <param name="deliveryAddressDTO">收货地址实体</param>
        public void AddDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO deliveryAddressDTO)
        {
            base.Do();
            this.AddDeliveryAddressExt(deliveryAddressDTO);
        }
        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        public void DeleteDeliveryAddress(System.Guid id)
        {
            base.Do();
            this.DeleteDeliveryAddressExt(id);
        }
        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="commodityDTO">地址实体</param>
        public void UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO commodityDTO)
        {
            base.Do();
            this.UpdateDeliveryAddressExt(commodityDTO);
        }
        /// <summary>
        /// 查询用户收货地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.DeliveryAddressDTO> GetAllDeliveryAddress(System.Guid userId)
        {
            base.Do();
            return this.GetAllDeliveryAddressExt(userId);
        }
        /// <summary>
        /// 查询一条收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.DeliveryAddressDTO GetDeliveryAddress(System.Guid id)
        {
            base.Do();
            return this.GetDeliveryAddressExt(id);
        }
    }
}