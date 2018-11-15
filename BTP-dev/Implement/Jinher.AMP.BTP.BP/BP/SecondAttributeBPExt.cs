
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/25 15:13:08
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SecondAttributeBP : BaseBP, ISecondAttribute
    {

        #region 数据方法区
        /// <summary>
        /// 添加尺寸/颜色
        /// </summary>
        /// <param name="commodityDTO">商品实体</param>
        public void SaveSecondAttribute(SecondAttributeDTO secondAttributeDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            secondAttributeDTO.EntityState = System.Data.EntityState.Added;
            SecondAttribute secondAttribute = new SecondAttribute().FromEntityData(secondAttributeDTO);
            contextSession.SaveObject(secondAttribute);
            contextSession.SaveChanges();
        }
        #endregion


        /// <summary>
        /// 添加尺寸/颜色
        /// </summary>
        /// <param name="attributeId">属性ID</param>
        /// <param name="name">二级属性名</param>
        public ResultDTO AddSecondAttributeExt(System.Guid attributeId, string name, Guid appid)
        {
            //缓存列表
            List<SecondAttributeDTO> attrList = new List<SecondAttributeDTO>();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Replace(';', ',');
                    string[] names = name.Split(',').Distinct().ToArray();

                    for (int i = 0; i < names.Length; i++)
                    {
                        SecondAttributeDTO secondAttributeDTO = new SecondAttributeDTO();
                        secondAttributeDTO.Id = Guid.NewGuid();
                        secondAttributeDTO.AttributeId = attributeId;
                        secondAttributeDTO.Name = names[i];
                        secondAttributeDTO.AppId = appid;

                        //this.SaveSecondAttribute(secondAttributeDTO);

                        secondAttributeDTO.EntityState = System.Data.EntityState.Added;
                        SecondAttribute secondAttribute = new SecondAttribute().FromEntityData(secondAttributeDTO);

                        DateTime date = DateTime.Now.AddSeconds(i);
                        secondAttribute.SubTime = date;

                        contextSession.SaveObject(secondAttribute);

                        attrList.Add(secondAttributeDTO);
                        contextSession.SaveChanges();
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加颜色尺寸服务异常。attributeId：{0}，name：{1}，appid：{2}", attributeId, name, appid), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }


            //后台线程更新属性缓存
            System.Threading.ThreadPool.QueueUserWorkItem(a =>
            {
                List<ComAttributeCacheDTO> cache = new List<ComAttributeCacheDTO>();

                foreach (SecondAttributeDTO comAttr in attrList)
                {
                    ComAttributeCacheDTO dto = GetComAttributeCacheDTO(comAttr);
                    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_AttributeInfo", dto.Id.ToString(), cache, "BTPCache");

                    Jinher.JAP.Common.Loging.LogHelper.Info("添加颜色/尺寸时更新了缓存");
                }

            });

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        private ComAttributeCacheDTO GetComAttributeCacheDTO(SecondAttributeDTO comAttr)
        {
            var dto = new ComAttributeCacheDTO
            {
                Id = comAttr.Id,
                AttributeId = comAttr.AttributeId,
                Name = comAttr.Name,
                SubTime = comAttr.SubTime
            };

            return dto;
        }

        /// <summary>
        /// 查询卖家所有已存在属性值
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeBySellerIDExt(System.Guid sellerID, Guid attributeid)
        {
            var query = from n in Jinher.AMP.BTP.BE.Attribute.ObjectSet()
                        join m in SecondAttribute.ObjectSet().Where(n => n.IsDel == false).OrderBy(n => n.SubTime) on n.Id equals m.AttributeId
                        where m.AppId == sellerID && n.Id == attributeid
                        orderby m.SubTime
                        select new ColorAndSizeAttributeVM
                        {
                            AppId = n.AppId,
                            AttributeId = n.Id,
                            SecondAttributeId = m.Id,
                            AttributeName = n.Name,
                            SecondAttributeName = m.Name,
                        };
            return query.ToList();
        }

        /// <summary>
        /// 查询卖家所有已存属性
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeByAppIDExt(System.Guid appID)
        {
            List<ColorAndSizeAttributeVM> query = (from n in Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(n => n.IsDel == false && n.AppId == appID).OrderBy(n => n.SubTime)
                                                   select new ColorAndSizeAttributeVM
                                                   {
                                                       AppId = n.AppId,
                                                       AttributeId = n.Id,
                                                       AttributeName = n.Name,
                                                   }).ToList();
            if (query == null)
            {
                query = new List<ColorAndSizeAttributeVM>();
            }

            Jinher.AMP.BTP.BE.Custom.Util.GetColorSizes(appID).ForEach(sz => query.Insert(0,new ColorAndSizeAttributeVM
                                  {
                                      AppId = sz.AppId,
                                      AttributeId = sz.Id,
                                      AttributeName = sz.Name,
                                  }));

            return query;
        }

        /// <summary>
        /// 商品属性添加
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ResultDTO AddAttributeExt(System.Guid attributeId, string name, Guid appid)
        {
            try
            {
                if (name == "颜色" || name == "尺寸")
                {
                    return new ResultDTO { ResultCode = 1, Message = "属性名称已存在！" };
                }

                var haveAttr = Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(r => r.Name == name && r.AppId == appid && r.IsDel == false).FirstOrDefault();
                if (haveAttr == null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    AttributeDTO attrDTO = new AttributeDTO();
                    attrDTO.Id = attributeId;
                    attrDTO.Name = name;
                    attrDTO.AppId = appid;
                    attrDTO.EntityState = System.Data.EntityState.Added;
                    Jinher.AMP.BTP.BE.Attribute attribute = new Jinher.AMP.BTP.BE.Attribute().FromEntityData(attrDTO);
                    attribute.SubTime = DateTime.Now;
                    contextSession.SaveObject(attribute);
                    contextSession.SaveChanges();
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "属性名称已存在！" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加属性服务异常。attributeId：{0}，name：{1}，appid：{2}", attributeId, name, appid), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 商品属性编辑
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ResultDTO UpdateAttributeExt(System.Guid attributeId, string name, Guid appid)
        {
            try
            {
                if (Jinher.AMP.BTP.BE.Custom.Util.GetColorSizesId().Contains(attributeId))
                {
                    return new ResultDTO { ResultCode = 0, Message = "颜色和尺寸名称不能修改" };
                }

                var attrModel = Jinher.AMP.BTP.BE.Attribute.FindByID(attributeId);

                if (attrModel != null && attrModel.Name.ToLower() == name.ToLower())
                {
                    return new ResultDTO { ResultCode = 0, Message = "名称未修改" };
                }
                else
                {
                    var haveAttr = Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(r => r.Name == name && r.AppId == appid && r.IsDel == false && r.Id != attributeId).FirstOrDefault();

                    // 判断此属性是否已经被商品引用过

                    if (haveAttr == null)
                    {
                        ContextSession contextSession = ContextFactory.CurrentThreadContext;

                        attrModel.Name = name;
                        attrModel.EntityState = System.Data.EntityState.Modified;

                        contextSession.SaveObject(attrModel);

                        contextSession.SaveChanges();
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 1, Message = "属性名称已存在！" };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加属性服务异常。attributeId：{0}，name：{1}，appid：{2}", attributeId, name, appid), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 属性删除
        /// </summary>
        /// <param name="secondAttributeId">次级属性ID</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelSecondAttributeExt(System.Guid secondAttributeId, System.Guid appid)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                SecondAttribute secondAttribute = SecondAttribute.ObjectSet().Where(n => n.Id == secondAttributeId).FirstOrDefault();
                if (secondAttribute != null)
                {
                    secondAttribute.EntityState = System.Data.EntityState.Modified;
                    secondAttribute.IsDel = true;
                    contextSession.SaveObject(secondAttribute);
                    contextSession.SaveChanges();

                    //后台线程更新属性缓存
                    System.Threading.ThreadPool.QueueUserWorkItem(a =>
                    {
                        ComAttributeCacheDTO dto = new ComAttributeCacheDTO
                        {
                            Id = secondAttribute.Id,
                            Name = secondAttribute.Name,
                            AttributeId = secondAttribute.AttributeId,
                            SubTime = secondAttribute.SubTime
                        };
                        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_AttributeInfo", dto.Id.ToString(), "BTPCache");
                        Jinher.JAP.Common.Loging.LogHelper.Info("删除颜色/尺寸时更新了缓存");
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除属性服务异常。secondAttributeId：{0}，appid：{1}", secondAttributeId, appid), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 是否已有属性  true/不存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExistsExt(string name, Guid appid, Guid attId)
        {
            bool bReturn = false;
            var commodity = SecondAttribute.ObjectSet().Where(n => n.Name == name && n.AppId == appid && n.AttributeId == attId && n.IsDel != true).FirstOrDefault();
            bReturn = (null == commodity);
            return bReturn;
        }
    }
}