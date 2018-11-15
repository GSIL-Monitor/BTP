

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
    public partial class CommodityJournal
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
        /// 根据商品实体生成商品日志
        /// </summary>
        /// <param name="commodity"></param>
        /// <returns></returns>
        public static CommodityJournal CreateCommodityJournal(Commodity commodity)
        {
            if (commodity == null || commodity.Id == Guid.Empty)
            {
                return null;
            }
            var journal = CreateCommodityJournal();
            journal.No_Number = commodity.No_Number;
            journal.Price = commodity.Price;
            journal.Stock = commodity.Stock;
            journal.PicturesPath = commodity.PicturesPath;
            journal.Description = commodity.Description;
            journal.State = commodity.State;
            journal.IsDel = commodity.IsDel;
            journal.AppId = commodity.AppId;
            journal.No_Code = commodity.No_Code;
            journal.TotalCollection = commodity.TotalCollection;
            journal.TotalReview = commodity.TotalReview;
            journal.Salesvolume = commodity.Salesvolume;
            journal.GroundTime = commodity.GroundTime;
            journal.ComAttribute = commodity.ComAttribute;
            journal.CategoryName = commodity.CategoryName;
            journal.SortValue = commodity.SortValue;
            journal.FreightTemplateId = commodity.FreightTemplateId;
            journal.MarketPrice = commodity.MarketPrice;
            journal.IsEnableSelfTake = commodity.IsEnableSelfTake;
            journal.Weight = commodity.Weight;
            journal.PricingMethod = commodity.PricingMethod;
            journal.SaleAreas = commodity.SaleAreas;
            journal.CommodityId = commodity.Id;
            journal.Name = commodity.Name;
            journal.SharePercent = commodity.SharePercent;
            journal.HtmlVideoPath = commodity.HtmlVideoPath;
            journal.MobileVideoPath = commodity.MobileVideoPath;
            journal.VideoPic = commodity.VideoPic;
            journal.VideoName = commodity.VideoName;
            journal.ScorePercent = commodity.ScorePercent;

            return journal;
        }
    }
}



