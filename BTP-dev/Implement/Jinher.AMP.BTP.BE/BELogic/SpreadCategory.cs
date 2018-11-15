

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
namespace Jinher.AMP.BTP.BE
{
    public partial class SpreadCategory
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

        private volatile static List<SpreadCategory> spreadCategory = null;

        private static readonly object lockHelper = new object();
        /// <summary>
        /// 获取推广主分成比例相关所有信息
        /// </summary>
        /// <returns>推广主分成比例信息</returns>
        public static List<SpreadCategory> GetSpreadCategory()
        {
            return ObjectSet().ToList();
        }
        public static List<SpreadCategoryDTO> GetSpreadCategoryDTO()
        {
            List<SpreadCategoryDTO> result = new List<SpreadCategoryDTO>();
            var entitys = GetSpreadCategory();
            if (entitys != null && entitys.Any())
            {
                result.AddRange(entitys.Select(category => category.ToEntityData()));
            }
            return result;
        }
    }
}



