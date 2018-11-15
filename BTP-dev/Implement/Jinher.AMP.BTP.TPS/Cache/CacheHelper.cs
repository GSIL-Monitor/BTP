using Jinher.AMP.BTP.TPS.Cache;

namespace Jinher.AMP.BTP.TPS
{
    public static class CacheHelper
    {
        public static MallApplyCache MallApply = new MallApplyCache();
        public static CommodityCategoryCache CommodityCategory = new CommodityCategoryCache();
        public static CommoditySpecificationsCache CommoditySpecifications = new CommoditySpecificationsCache();
        public static ZPHCache ZPH = new ZPHCache();
        public static YJBCache YJB = new YJBCache();
        public static AppCache App = new AppCache();
        public static LVPCache LVP = new LVPCache();
    }
}
