using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using System.Text.RegularExpressions;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 收货地址接口类
    /// </summary>
    public partial class DeliveryAddressSV : BaseSv, IDeliveryAddress
    {

        /// <summary>
        /// 添加收货地址
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">APPId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddressExt(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            try
            {
                ResultDTO result = ValidAddress(addressDTO);
                if (result.ResultCode != 0)
                {
                    return result;
                }

                DeliveryAddressDTO deliveryAddressDTO = new DeliveryAddressDTO();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                deliveryAddressDTO.EntityState = System.Data.EntityState.Added;
                deliveryAddressDTO.Id = Guid.NewGuid();
                deliveryAddressDTO.AppId = addressDTO.AppId;
                deliveryAddressDTO.Name = "收货地址";
                deliveryAddressDTO.RecipientsUserName = addressDTO.ReceiptUserName;
                deliveryAddressDTO.UserId = addressDTO.UserId;
                deliveryAddressDTO.RecipientsPhone = addressDTO.ReceiptPhone;
                deliveryAddressDTO.RecipientsAddress = addressDTO.ReceiptAddress;
                deliveryAddressDTO.Province = addressDTO.Province;
                deliveryAddressDTO.City = addressDTO.City;
                deliveryAddressDTO.District = addressDTO.District;
                deliveryAddressDTO.Street = addressDTO.Street;
                deliveryAddressDTO.ProvinceCode = addressDTO.ProvinceCode;
                deliveryAddressDTO.CityCode = addressDTO.CityCode;
                deliveryAddressDTO.DistrictCode =addressDTO.DistrictCode;
                deliveryAddressDTO.StreetCode = addressDTO.StreetCode;
                deliveryAddressDTO.RecipientsZipCode = addressDTO.RecipientsZipCode;
                deliveryAddressDTO.IsDefault = addressDTO.IsDefault == 1 ? true : false;
                DeliveryAddress deliveryAddress = new DeliveryAddress().FromEntityData(deliveryAddressDTO);
                if (addressDTO.IsDefault == 1)
                {
                    DeliveryAddress.ObjectSet().Context.ExecuteStoreCommand("update DeliveryAddress  set IsDefault=0 where UserId='" + deliveryAddress.UserId + "'");
                }
                contextSession.SaveObject(deliveryAddress);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加收货地址服务异常。addressDTO：{0}", JsonHelper.JsonSerializer(addressDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "添加收货地址异常!" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="IsDefault">是否默认收货地址</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddressListExt
            (System.Guid userId, System.Guid appId, int IsDefault = 0)
        {
            // bool isde = IsDefault == 0 ? false : true;
            var aa = DeliveryAddress.ObjectSet().Where(n => n.UserId == userId).OrderByDescending(n=>n.IsDefault).ThenByDescending(n => n.SubTime).Select(
                                        n => new AddressSDTO
                                        {
                                            UserId = n.UserId,
                                            ReceiptUserName = n.RecipientsUserName,
                                            ReceiptPhone = n.RecipientsPhone,
                                            ReceiptAddress = n.RecipientsAddress,
                                            Province = n.Province,
                                            City = n.City,
                                            District = n.District,
                                            Street=n.Street,
                                            AppId = n.AppId,
                                            AddressId = n.Id,
                                            RecipientsZipCode = n.RecipientsZipCode,
                                            IsDefault = (n.IsDefault == true ? 1 : 0),
                                            ProvinceCode = n.ProvinceCode,
                                            CityCode = n.CityCode,
                                            DistrictCode = n.DistrictCode,
                                            StreetCode=n.StreetCode
                                        });
            List<AddressSDTO> deliveryAddress = new List<AddressSDTO>();
            foreach (var addressSdto in aa)
            {
                deliveryAddress.Add(addressSdto);
            }

            //如果没有默认收货地址 获取最新添加的收货地址 
            if (IsDefault == 1 && deliveryAddress != null && deliveryAddress.Count() > 0)
            {
                var deliveryAddressnew = deliveryAddress.Where(n => n.IsDefault == IsDefault).ToList();

                if (deliveryAddressnew == null || deliveryAddressnew.Count() == 0)
                {
                    System.Collections.Generic.List<AddressSDTO> list = new System.Collections.Generic.List<AddressSDTO>();
                    list.Add(deliveryAddress.FirstOrDefault());
                    deliveryAddress = list;

                }
                else
                {
                    deliveryAddress = deliveryAddressnew;
                }
            }
            return deliveryAddress;
        }

        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddressExt
            (System.Guid userId, System.Guid appId)
        {
            var deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.UserId == userId).Select(
                                        n => new AddressSDTO
                                        {
                                            UserId = n.UserId,
                                            ReceiptUserName = n.RecipientsUserName,
                                            ReceiptPhone = n.RecipientsPhone,
                                            ReceiptAddress = n.RecipientsAddress,
                                            Province = n.Province,
                                            City = n.City,
                                            District = n.District,
                                            Street=n.Street,
                                            AppId = n.AppId,
                                            AddressId = n.Id,
                                            RecipientsZipCode = n.RecipientsZipCode,
                                            ProvinceCode = n.ProvinceCode,
                                            CityCode = n.CityCode,
                                            DistrictCode = n.DistrictCode,
                                            StreetCode=n.StreetCode
                                        }).ToList();

           
            return deliveryAddress;
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteDeliveryAddressExt(System.Guid addressId, System.Guid appId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var address = DeliveryAddress.ObjectSet().Where(n => n.Id == addressId).FirstOrDefault();

                if (address != null)
                {
                    address.EntityState = System.Data.EntityState.Deleted;
                    contextSession.SaveObject(address);
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除收货地址服务异常。addressId：{0}。appId：{1}", addressId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 编辑收货地址 设置默认地址
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddressIsDefaultExt(System.Guid addressId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var address = DeliveryAddress.ObjectSet().FirstOrDefault(n => n.Id == addressId);

                if (address != null)
                {
                    if (!address.IsDefault)
                    {
                        var alist = DeliveryAddress.ObjectSet().Where(t => t.UserId == address.UserId && t.Id != addressId);
                        foreach (var a in alist)
                        {
                            a.IsDefault = false;
                            a.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(a);
                        }
                        address.IsDefault = true;
                    }
                    else
                    {
                        address.IsDefault = false;                      
                    }

                    address.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(address);
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑收货地址服务异常。addressId：{0}。", addressId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 编辑收货地址
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddressExt
            (Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO, System.Guid appId)
        {
            try
            {
                ResultDTO result = ValidAddress(addressDTO);
                if (result.ResultCode != 0)
                {
                    return result;
                }

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                DeliveryAddress deliveryAddress = null;
                if (addressDTO.AddressId != Guid.Empty)
                {
                    deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.Id == addressDTO.AddressId).FirstOrDefault();
                }

                if (deliveryAddress == null)
                {
                    deliveryAddress = DeliveryAddress.CreateDeliveryAddress();
                }
                else
                {
                    deliveryAddress.EntityState = System.Data.EntityState.Modified;
                }
                deliveryAddress.Name = "";
                if (addressDTO.UserId != Guid.Empty)
                {
                    deliveryAddress.UserId = addressDTO.UserId;
                }

                deliveryAddress.AppId = addressDTO.AppId;
                deliveryAddress.RecipientsUserName = addressDTO.ReceiptUserName;
                deliveryAddress.RecipientsPhone = addressDTO.ReceiptPhone;
                deliveryAddress.RecipientsAddress = addressDTO.ReceiptAddress;
                deliveryAddress.Province = addressDTO.Province;
                deliveryAddress.City = addressDTO.City;
                deliveryAddress.District = addressDTO.District;
                deliveryAddress.Street = addressDTO.Street;
                deliveryAddress.ProvinceCode = addressDTO.ProvinceCode;
                deliveryAddress.CityCode = addressDTO.CityCode;
                deliveryAddress.DistrictCode = addressDTO.DistrictCode;
                deliveryAddress.StreetCode = addressDTO.StreetCode;
                deliveryAddress.RecipientsZipCode = addressDTO.RecipientsZipCode;
                deliveryAddress.IsDefault = addressDTO.IsDefault == 1 ? true : false;
                if (addressDTO.IsDefault == 1)
                {
                    DeliveryAddress.ObjectSet().Context.ExecuteStoreCommand("update DeliveryAddress  set IsDefault=0 where UserId='" + deliveryAddress.UserId + "'");
                }
                contextSession.SaveObject(deliveryAddress);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑收货地址服务异常。addressDTO：{0}。appId：{1}", JsonHelper.JsonSerializer(addressDTO), appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 收货地址详情
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO GetDeliveryAddressByAddressIdExt(System.Guid addressId, System.Guid appId)
        {
            var deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.Id == addressId).FirstOrDefault();
            AddressSDTO address = new AddressSDTO();
            if (deliveryAddress != null)
            {
                address.AddressId = deliveryAddress.Id;
                address.UserId = deliveryAddress.UserId;
                address.ReceiptUserName = deliveryAddress.RecipientsUserName;
                address.ReceiptPhone = deliveryAddress.RecipientsPhone;
                address.ReceiptAddress = deliveryAddress.RecipientsAddress;
                address.Province = deliveryAddress.Province;
                address.City = deliveryAddress.City;
                address.District = deliveryAddress.District;
                address.Street = deliveryAddress.Street;
                address.AppId = deliveryAddress.AppId;
                address.RecipientsZipCode = deliveryAddress.RecipientsZipCode;
                address.IsDefault = deliveryAddress.IsDefault == true ? 1 : 0;
                address.ProvinceCode = deliveryAddress.ProvinceCode;
                address.CityCode = deliveryAddress.CityCode;
                address.DistrictCode = deliveryAddress.DistrictCode;
                address.StreetCode = deliveryAddress.StreetCode;
                
            }



            return address;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ValidAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            if (addressDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，参数不能为空！" };
            }
            if (string.IsNullOrWhiteSpace(addressDTO.ReceiptUserName))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，收货人不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.ReceiptPhone))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，手机号不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.ReceiptAddress))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，收货地址不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.Province))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，省不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.City))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，市不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.District))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，区不能为空！" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ValidAddressNew(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            ResultDTO result = ValidAddress(addressDTO);
            if (result.ResultCode != 0)
            {
                return result;
            }
            if (string.IsNullOrWhiteSpace(addressDTO.ProvinceCode))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，省编码不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.CityCode))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，市编码不能为空！" };
            }
            else if (string.IsNullOrWhiteSpace(addressDTO.DistrictCode))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误，地区编码不能为空！" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }


        /// <summary>
        /// 保存（添加或修改）收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/SaveDeliveryAddressNew
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddressNewExt(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            try
            {
                ResultDTO result = ValidAddressNew(addressDTO);
                if (result.ResultCode != 0)
                {
                    return result;
                }

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                DeliveryAddress deliveryAddress = null;
                if (addressDTO.AddressId != Guid.Empty)
                {
                    deliveryAddress = DeliveryAddress.ObjectSet().Where(n => n.Id == addressDTO.AddressId).FirstOrDefault();
                }

                if (deliveryAddress == null)
                {
                    deliveryAddress = DeliveryAddress.CreateDeliveryAddress();
                    deliveryAddress.Id = Guid.NewGuid();
                }
                else
                {
                    deliveryAddress.EntityState = System.Data.EntityState.Modified;
                }
                deliveryAddress.Name = "";
                if (addressDTO.UserId != Guid.Empty)
                {
                    deliveryAddress.UserId = addressDTO.UserId;
                }

                deliveryAddress.AppId = addressDTO.AppId;
                deliveryAddress.RecipientsUserName = addressDTO.ReceiptUserName;
                deliveryAddress.RecipientsPhone = addressDTO.ReceiptPhone;
                deliveryAddress.RecipientsAddress = addressDTO.ReceiptAddress;
                deliveryAddress.Province = addressDTO.Province;
                deliveryAddress.City = addressDTO.City;
                deliveryAddress.District = addressDTO.District;
                deliveryAddress.Street = addressDTO.Street;
                deliveryAddress.RecipientsZipCode = addressDTO.RecipientsZipCode;
                deliveryAddress.ProvinceCode = addressDTO.ProvinceCode;
                deliveryAddress.CityCode = addressDTO.CityCode;
                deliveryAddress.DistrictCode = addressDTO.DistrictCode;
                deliveryAddress.StreetCode = addressDTO.StreetCode;
                deliveryAddress.IsDefault = addressDTO.IsDefault == 1 ? true : false;
                if (addressDTO.IsDefault == 1)
                {
                    DeliveryAddress.ObjectSet().Context.ExecuteStoreCommand("update DeliveryAddress  set IsDefault=0 where UserId='" + deliveryAddress.UserId + "'");
                }
                contextSession.SaveObject(deliveryAddress);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SaveDeliveryAddressNewExt异常。addressDTO：{0}，异常信息：{1}", addressDTO, ex));
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
    }
}
