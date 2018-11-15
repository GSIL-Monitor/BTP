

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
    public partial class CommodityInnerCategory
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
        /// 创建商品默认分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static CommodityInnerCategory CreateDefaultInnerCategory(Guid commodityId, Guid appId, Guid subId)
        {
            CommodityInnerCategory result = CommodityInnerCategory.CreateCommodityInnerCategory();
            result.CategoryId = Guid.Empty;
            result.CommodityId = commodityId;
            result.SubId = subId;
            result.SubTime = DateTime.Now;
            result.Name = "商品分类";
            result.AppId = appId;
            result.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(appId);
            return result;
        }
    }
}



