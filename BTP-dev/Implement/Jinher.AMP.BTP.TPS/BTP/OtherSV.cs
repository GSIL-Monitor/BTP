using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 处理其它不需要添
    /// </summary>
    public class OtherSV
    {
        public static void UpdataOldDataInCommodityCategory()
        {
            int pageIndex = 1;
            int pageSize = 1000;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var list = CommodityCategory.ObjectSet().Select(t => t).OrderBy(t => t.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            while (list.Count == pageSize)
            {
                foreach (var item in list)
                {
                    item.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(item.AppId);
                    item.ModifiedOn = DateTime.Now;
                    item.EntityState = System.Data.EntityState.Modified;
                }
                contextSession.SaveChanges();
                pageIndex++;
                list = CommodityCategory.ObjectSet().Select(t => t).OrderBy(t => t.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(item.AppId);
                    item.ModifiedOn = DateTime.Now;
                    item.EntityState = System.Data.EntityState.Modified;
                }
                contextSession.SaveChanges();
            }
        }

        public static void InsertDataInShoppingCartItems()
        {
            int pageIndex = 1;
            int pageSize = 1000;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
           
            var listShop = ShoppingCartItems.ObjectSet().Select(t => t).OrderBy(t => t.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            while (listShop.Count == pageSize)
            {
                foreach (var itemShop in listShop)
                {
                    var attrsShop = itemShop.ComAttributeIds.Replace("null", "")
                                            .Replace("nil", "")
                                            .Replace("undefined", "");
                    var arr = attrsShop.Split(',');
                    if (arr[0] != "" && arr[1] != "")
                    {
                        var list = CommodityStock.ObjectSet().Where(t => t.CommodityId == itemShop.CommodityId).
                        Select(t => t).ToList();
                        foreach (var item in list)
                        {
                            var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                            if ((attrs[0].SecondAttribute == arr[0] && attrs[1].SecondAttribute == arr[1]) || (attrs[0].SecondAttribute == arr[1] && attrs[1].SecondAttribute == arr[0]))
                            {
                                itemShop.CommodityStockId = item.Id;
                                itemShop.ModifiedOn = DateTime.Now;
                                itemShop.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                        contextSession.SaveChanges();
                        pageIndex++;
                        listShop = ShoppingCartItems.ObjectSet().Select(t => t).OrderBy(t => t.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }

                }
            }
            if (listShop.Count > 0)
            {
                foreach (var itemShop in listShop)
                {
                    var attrsShop = itemShop.ComAttributeIds.Replace("null", "")
                                            .Replace("nil", "")
                                            .Replace("undefined", "");
                    var arr = attrsShop.Split(',');
                    if (arr[0] != "" && arr[1] != "")
                    {
                        var list = CommodityStock.ObjectSet().Where(t => t.CommodityId== itemShop.CommodityId).
                        Select(t => t).ToList();
                        foreach (var item in list)
                        {
                            var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                            if ((attrs[0].SecondAttribute == arr[0] && attrs[1].SecondAttribute == arr[1])||(attrs[0].SecondAttribute == arr[1] && attrs[1].SecondAttribute == arr[0]))
                            {
                                itemShop.CommodityStockId = item.Id;
                                itemShop.ModifiedOn = DateTime.Now;
                                itemShop.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                        contextSession.SaveChanges();
                    }
                        
                }
            }
        }
    }
}
