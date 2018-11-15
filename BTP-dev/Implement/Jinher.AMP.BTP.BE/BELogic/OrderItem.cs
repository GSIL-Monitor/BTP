

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE.Custom;
namespace Jinher.AMP.BTP.BE
{
    public partial class OrderItem
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion


        /// <summary>
        /// 添加操作
        /// </summary>
        public void Add(OrderItemDTO orderItemsDTO)
        {
            OrderItem orderItems = new OrderItem().FromEntityData(orderItemsDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            orderItems.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(orderItems);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        public void Updates(OrderItemDTO orderItemsDTO)
        {
            OrderItem orderItems = new OrderItem().FromEntityData(orderItemsDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            orderItems.EntityState = System.Data.EntityState.Modified;
            contextSession.SaveObject(orderItems);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(OrderItem orderItems)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            orderItems.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(orderItems);
            contextSession.SaveChange();
        }

        /// <summary>
        /// 查询订单下商品
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <returns></returns>
        public List<OrderItemsVM> SelectOrderItemsByOrderId(Guid commodityOrderId, Guid appId)
        {
            CommodityCategory cc = new CommodityCategory();
            IEnumerable<OrderItemsVM> query = from data in OrderItem.ObjectSet()
                        join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                        where (data.CommodityOrderId == commodityOrderId && data1.CommodityType == 0)
                        select new OrderItemsVM
                        {
                            CommodityOrderId = data.CommodityOrderId,
                            CommodityId = data.CommodityId,
                            CommodityIdName = data1.Name,
                            PicturesPath = data1.PicturesPath,
                            Price = data.CurrentPrice,//取订单商品列表中的价格
                            Number = data.Number,
                            SizeAndColorId = data.ComAttributeIds,
                            //CommodityCategorys = cc.GetCommodityCategory(data1.Id).Select(n => n.Name).ToList(),
                        };

            query = query.ToList();
            //遍历查询商品的类别信息  ps:真的要这么做么？
            foreach (var item in query)
            {
                var category = (from data in CommodityCategory.ObjectSet()
                               join data1 in Category.ObjectSet() on data.CategoryId equals data1.Id
                                where data.CommodityId == item.CommodityId && data.AppId == appId
                               select data1.Name).ToList();
                item.CommodityCategorys = category;
            }
            //获取订单商品的一些信息
            List<OrderItemsVM> orderItemsVMList = query.ToList<OrderItemsVM>();

            SecondAttribute secondAttribute = new SecondAttribute();
            Jinher.AMP.BTP.BE.Attribute attribute = new Jinher.AMP.BTP.BE.Attribute();

            //获取app的所有次级属性
            List<SecondAttributeDTO> secondAttributeDTOList = secondAttribute.GetSecondAttributeBySellerID(appId);

            //获取app的所有属性(目前只有颜色和尺寸两种)
            List<Attribute> attributeDTOList = Attribute.ObjectSet().ToList();

            List<OrderItemsVM> orderItemslist = new List<OrderItemsVM>();
            Collection collect = new Collection();

            //遍历商品信息，获取每个商品对应的颜色、尺寸属性
            foreach (OrderItemsVM model in orderItemsVMList)
            {
                
                List<ComAttibuteDTO> comAlist = new List<ComAttibuteDTO>();
                string attributeString = model.SizeAndColorId;
                if (!string.IsNullOrWhiteSpace(attributeString))
                {
                    string[] attributeStringArray = attributeString.Split(',');
                    for (int i = 0; i < attributeStringArray.Length; i++)
                    {
                        SecondAttributeDTO tempSecondDTO = secondAttributeDTOList.Where(p => p.Id == Guid.Parse(attributeStringArray[i])).FirstOrDefault();
                        if (tempSecondDTO != null)
                        {
                            Attribute tempDTO = attributeDTOList.Where(p => p.Id == tempSecondDTO.AttributeId).FirstOrDefault();
                            ComAttibuteDTO comA = new ComAttibuteDTO();
                            comA.AttributeId = tempDTO.Id;
                            comA.Code = "1";
                            comA.SubTime = DateTime.Now;
                            comA.AttributeName = tempDTO.Name;
                            comA.CommodityId = model.CommodityId;
                            comA.SecondAttributeName = tempSecondDTO.Name;
                            comA.SecondAttributeId = tempSecondDTO.Id;
                            comAlist.Add(comA);
                        }
                    }
                }
                model.SelectedComAttibutes = comAlist;
                orderItemslist.Add(model);
            }
            return orderItemslist;
        }
        #region 公共方法

        /// <summary>
        /// 订单商品LIST方法
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityForOrderDTO> GetOrderItemsExt(System.Guid userId, System.Guid appId, Guid commodityOrderId, int state)
        {

            var commodityOrderList = (from o in OrderItem.ObjectSet()
                                     where o.CommodityOrderId == commodityOrderId
                                     select new CommodityOrderItemDTO
                                     {
                                         Id = o.Id,
                                         PicturesPath = o.PicturesPath,
                                         Name = o.Name,
                                         Price = o.CurrentPrice, // o.CommodityOrder.Price, 商品单价
                                         Intensity = o.Intensity, //折扣
                                         Number = o.Number,       //数量
                                         CommodityOrderId = o.CommodityOrderId,
                                         ComAttributeIds = o.ComAttributeIds,
                                         CommodityAttributes = o.CommodityAttributes,
                                         CommodityId = o.CommodityId,    
                                         HasReview = o.AlreadyReview,
                                         RealPrice = o.CurrentPrice,  // 原价
                                         DiscountPrice = o.DiscountPrice
                                     }).ToList();

            Dictionary<Guid, decimal> promotionDic = new Dictionary<Guid, decimal>();
            if (state == 0)
            {
                List<Guid> commodityIds = commodityOrderList.Select(o => o.CommodityId).ToList();                
                if (commodityIds.Count > 0)
                {
                    PromotionItems promotionItems = new PromotionItems();
                    promotionDic = promotionItems.GetIntensity(commodityIds);
                }
            }

            List<CommodityForOrderDTO> commoditySDTOList = new List<CommodityForOrderDTO>();
            foreach (CommodityOrderItemDTO commodityOrderItemDTO in commodityOrderList)
            {
                decimal? realIntensity = null;
                CommodityForOrderDTO commoditySDTO = new CommodityForOrderDTO();

                commoditySDTO.Id = commodityOrderItemDTO.Id;
                commoditySDTO.Pic = commodityOrderItemDTO.PicturesPath;
                commoditySDTO.Name = commodityOrderItemDTO.Name;
               
                //待付款
                if (state == 0)
                {
                    Guid commodityId = commodityOrderItemDTO.CommodityId;
                    if (promotionDic != null && promotionDic.ContainsKey(commodityId))
                    {
                        realIntensity = promotionDic[commodityId];
                    }
                    else
                    {
                        realIntensity = commodityOrderItemDTO.Intensity;
                    }
                    commoditySDTO.Price = commodityOrderItemDTO.Price;
                    commoditySDTO.Intensity = realIntensity;
                }
                else {
                    commoditySDTO.Price = commodityOrderItemDTO.RealPrice;
                    commoditySDTO.Intensity = commodityOrderItemDTO.Intensity;//折扣
                }
                
                commoditySDTO.CommodityNumber = commodityOrderItemDTO.Number;
                commoditySDTO.OrderId = commodityOrderItemDTO.CommodityOrderId;
                commoditySDTO.AttributesString = commodityOrderItemDTO.ComAttributeIds;
                commoditySDTO.HasReview = commodityOrderItemDTO.HasReview;
                commoditySDTO.SelectedAttributes = Util.ConvertToAttributeList(commodityOrderItemDTO.CommodityAttributes);                

                commoditySDTOList.Add(commoditySDTO);
            }
            return commoditySDTOList;
        }
        #endregion

        /// <summary>
        /// 订单商品LIST方法
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Dictionary<Guid, List<CommodityForOrderDTO>> GetAllOrderItemsExt(System.Guid userId, System.Guid appId, List<Guid> commodityOrderIds)
        {

            //获取商品信息
            var query = (from o in OrderItem.ObjectSet()
                         where commodityOrderIds.Contains(o.CommodityOrderId)
                         select new CommodityOrderItemDTO
                         {
                             Id = o.Id,
                             PicturesPath = o.PicturesPath,
                             Name = o.Name,
                             Price = o.CurrentPrice, //o.CommodityOrder.Price,单价
                             Intensity = o.Intensity,
                             Number = o.Number, //数量
                             CommodityOrderId = o.CommodityOrderId,
                             ComAttributeIds = o.ComAttributeIds,
                             CommodityAttributes = o.CommodityAttributes,
                             CommodityId = o.CommodityId
                             //OrderState = o.CommodityOrder.State
                         }).ToList();

            //查询待付款的商品ID
            List<Guid> commodityIds = query.Where(o => o.OrderState == 0).Select(o => o.CommodityId).ToList();

            Dictionary<Guid, decimal> promotionDic = new Dictionary<Guid, decimal>();
            //判断促销信息
            if (commodityIds.Count > 0)
            {
                PromotionItems promotionItems = new PromotionItems();
                //获取折扣信息
                promotionDic = promotionItems.GetIntensity(commodityIds);
            }
            Jinher.JAP.Common.Loging.LogHelper.Error("GetAllOrderItemsExt方法：");
            List<CommodityForOrderDTO> commoditySDTOList = new List<CommodityForOrderDTO>();

            //遍历订单待付款的
            foreach (CommodityOrderItemDTO commodityOrderItemDTO in query)
            {


                decimal? realIntensity = null;
                //待付款
                if (commodityOrderItemDTO.OrderState == 0)
                {
                    Guid commodityId = commodityOrderItemDTO.CommodityId;
                    if (promotionDic != null && promotionDic.ContainsKey(commodityId))
                    {
                        realIntensity = promotionDic[commodityId];
                    }
                    else
                    {
                        realIntensity = 10;
                    }
                }
                else
                {
                    realIntensity = commodityOrderItemDTO.Intensity;
                }
                CommodityForOrderDTO commoditySDTO = new CommodityForOrderDTO();
                commoditySDTO.Id = commodityOrderItemDTO.Id;
                commoditySDTO.Pic = commodityOrderItemDTO.PicturesPath;
                commoditySDTO.Name = commodityOrderItemDTO.Name;
                commoditySDTO.Price = commodityOrderItemDTO.Price;

                Jinher.JAP.Common.Loging.LogHelper.Error("商品价格为：" + commodityOrderItemDTO.Price);
                Jinher.JAP.Common.Loging.LogHelper.Error("realIntensity商品折扣为：" + realIntensity);
                Jinher.JAP.Common.Loging.LogHelper.Error("commodityOrderItemDTO.Intensity商品折扣为：" + commodityOrderItemDTO.Intensity);

                commoditySDTO.Intensity = commodityOrderItemDTO.Intensity;// realIntensity;
                commoditySDTO.CommodityNumber = commodityOrderItemDTO.Number;
                commoditySDTO.OrderId = commodityOrderItemDTO.CommodityOrderId;
                commoditySDTO.AttributesString = commodityOrderItemDTO.ComAttributeIds;
                commoditySDTO.SelectedAttributes = Util.ConvertToAttributeList(commodityOrderItemDTO.CommodityAttributes);

                commoditySDTOList.Add(commoditySDTO);
            }
            return commoditySDTOList.GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group }).ToDictionary(c => c.OrderId, c => c.CommodityList.ToList());

        }
    }
}
