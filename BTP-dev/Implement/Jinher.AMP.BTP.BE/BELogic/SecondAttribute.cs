

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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
    public partial class SecondAttribute
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
        /// 查询卖家所有已存在尺寸/颜色
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeBySellerID(System.Guid sellerID, Guid attributeid)
        {
            var query = from n in Jinher.AMP.BTP.BE.Attribute.ObjectSet()
                        join m in SecondAttribute.ObjectSet().Where(n => n.IsDel == false).OrderBy(n => n.SubTime) on n.Id equals m.AttributeId
                        where m.AppId == sellerID && n.Id == attributeid
                        select new ColorAndSizeAttributeVM
                        {
                            AppId = n.AppId,
                            AttributeId = n.Id,
                            SecondAttributeId = m.Id,
                            AttributeName = n.Name,
                            SecondAttributeName = m.Name,
                        };
            return query;
        }

        /// <summary>
        /// 查询卖家所有已存在的次级属性
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.SecondAttributeDTO> GetSecondAttributeBySellerID(System.Guid sellerID)
        {
            var query = SecondAttribute.ObjectSet().Where(i => i.AppId == sellerID).OrderBy(n => n.SubTime).ToList();
            return new SecondAttribute().ToEntityDataList(query);
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        public void Add(SecondAttributeDTO secondAttributeDTO)
        {
            SecondAttribute secondAttribute = new SecondAttribute().FromEntityData(secondAttributeDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            secondAttribute.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(secondAttribute);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        public void Updates(SecondAttributeDTO secondAttributeDTO)
        {
            SecondAttribute secondAttribute = new SecondAttribute().FromEntityData(secondAttributeDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            secondAttribute.EntityState = System.Data.EntityState.Modified;
            contextSession.SaveObject(secondAttribute);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(SecondAttribute secondAttribute)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            secondAttribute.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(secondAttribute);
            contextSession.SaveChange();
        }

        private static List<SecondAttribute> _communicates;
        private static List<SecondAttribute> _billings;
        /// <summary>
        /// 获取服务类沟通方式
        /// </summary>
        /// <returns></returns>
        public static List<SecondAttribute> GetCommunicateAttributes()
        {
            if (_communicates == null)
                _communicates = ObjectSet().Where(c => c.AppId == Guid.Empty && c.AttributeId == new Guid("11111111-1111-1111-1111-111111111111")).ToList();
            return _communicates;
        }

        /// <summary>
        /// 获取服务类计费方式
        /// </summary>
        /// <returns></returns>
        public static List<SecondAttribute> GetBillingAttributes()
        {
            if (_billings == null)
                _billings = ObjectSet().Where(c => c.AppId == Guid.Empty && c.AttributeId == new Guid("22222222-2222-2222-2222-222222222222")).ToList();
            return _billings;
        }
    }
}



