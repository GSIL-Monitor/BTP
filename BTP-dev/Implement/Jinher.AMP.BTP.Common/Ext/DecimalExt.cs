namespace System
{
    /// <summary>
    /// decimal类型扩展类
    /// </summary>
    public static class DecimalExt
    {
        /// <summary>
        /// 转换成人民币（将decimal类型数据向下取整，并保留两位小数）
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static decimal ToMoney(this decimal dec)
        {
            return ToMoney(dec, 2);
        }

        /// <summary>
        /// 转换成人民币 将decimal类型数据向下取整
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="roundNum">保留位数</param>
        /// <returns></returns>
        public static decimal ToMoney(this decimal dec, int roundNum)
        {
            var cardinalNumber = (decimal)Math.Pow(10, roundNum);
            return ((long)(cardinalNumber * dec)) / cardinalNumber;
        }
        /// <summary>
        /// 转换成支持金币的金额（将decimal类型数据向下取整，并保留三位小数）
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static decimal ToGoldMoney(this decimal dec)
        {
            return ((long)(1000 * dec)) / 1000.0m;
        }

        /// <summary>
        /// 转换成支持金币（将decimal类型数据向下取整*1000）
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static long ToGold(this decimal dec)
        {
            return (long)(1000 * dec);
        }
    }
}
