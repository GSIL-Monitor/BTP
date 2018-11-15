
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 10:22:05
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DeliveryAddressBP : BaseBP, IDeliveryAddress
    {

        /// <summary>
        /// 添加收货地址
        /// </summary>
        /// <param name="deliveryAddressDTO">收货地址实体</param>
        public void AddDeliveryAddressExt(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO deliveryAddressDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (deliveryAddressDTO.EntityState == System.Data.EntityState.Added)
            {
                deliveryAddressDTO.Id = Guid.NewGuid();
            }
            DeliveryAddress deliveryAddress = new DeliveryAddress().FromEntityData(deliveryAddressDTO);
            contextSession.SaveObject(deliveryAddress);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        public void DeleteDeliveryAddressExt(System.Guid id)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DeliveryAddress deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.Id == id).FirstOrDefault();
            if (!string.IsNullOrEmpty(deliveryAddress.ToString()))
            {
                contextSession.Delete(deliveryAddress);

            }
            contextSession.SaveChange();
        }
        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="commodityDTO">地址实体</param>
        public void UpdateDeliveryAddressExt(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO commodityDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DeliveryAddress commodity = new DeliveryAddress().FromEntityData(commodityDTO);
            contextSession.Update(commodity);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 查询用户收货地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.DeliveryAddressDTO> GetAllDeliveryAddressExt(System.Guid userId)
        {
            DeliveryAddressDTO dto = new DeliveryAddressDTO();
            var deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.UserId.Equals(userId));
            var result = from d in deliveryAddress
                         select new DeliveryAddressDTO
                         {
                             AppId = d.AppId,
                             City = d.City,
                             Code = d.Code,
                             District = d.District,
                             Id = d.Id,
                             IsDefault = d.IsDefault,
                             Name = d.Name,
                             Province = d.Province,
                             RecipientsAddress = d.RecipientsAddress,
                             RecipientsPhone = d.RecipientsPhone,
                             RecipientsUserName = d.RecipientsUserName,
                             UserId = d.UserId
                         };
            return result.ToList();
        }
        /// <summary>
        /// 查询一条收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.DeliveryAddressDTO GetDeliveryAddressExt(System.Guid id)
        {
            DeliveryAddress deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.Id == id).FirstOrDefault();
            return deliveryAddress.ToEntityData();
        }
    }
}