namespace System
{
    /// <summary>
    /// decimal类型扩展类
    /// </summary>
    public static class LongExt
    {
        /// <summary>
        /// 将金币转换成人民币（并保留两位小数）
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static decimal ToMoney(this long dec)
        {
            return dec / 10 / 100.0m;
        }
        /// <summary>
        /// 转换成支持金币的金额（并保留三位小数）
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static decimal ToGoldMoney(this long dec)
        {
            return dec / 1000.0m;
        }
    }
}
