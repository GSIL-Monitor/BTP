

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
using Jinher.JAP.Common.Loging;
namespace Jinher.AMP.BTP.BE
{
    public partial class Attribute
    {
        /// <summary>
        /// 根据名称集合获取属性
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Attribute> GetAttributeByName(List<string> attr, Guid appId)
        {
            List<Attribute> attrList = Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(n => n.AppId == appId && attr.Contains(n.Name.ToLower())).ToList();
            if (attrList.Any())
            {
                attrList.AddRange(Jinher.AMP.BTP.BE.Custom.Util.GetColorSizes(appId));
            }
            else
            {
                attrList = new List<Attribute>();
                attrList.AddRange(Jinher.AMP.BTP.BE.Custom.Util.GetColorSizes(appId));
            }
            return attrList.Where(n => attr.Any(c => c.ToLower() == n.Name.ToLower())).ToList();
        }

        public List<SecondAttribute> GetAttributeValueById(List<Guid> attrid, Guid appId)
        {
            return SecondAttribute.ObjectSet().Where(n => attrid.Contains(n.AttributeId) && n.AppId == appId).ToList();
        }


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

    }
}



