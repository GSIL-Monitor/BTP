
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商收发货地址信息
    /// </summary>
    public class ThirdAddress
    {
        /// <summary>
        /// 收件人姓名	
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 收件人电话	
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 收件人一级地址编号
        /// </summary>
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 收件人一级地址名称	
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 收件人二级地址编号
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 收件人二级地址名称	
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 收件人三级地址编号
        /// </summary>
        public string CountyCode { get; set; }

        /// <summary>
        /// 收件人三级地址名称	
        /// </summary>
        public string CountyName { get; set; }

        /// <summary>
        /// 收件人四级地址编号
        /// </summary>
        public string TownCode { get; set; }

        /// <summary>
        /// 收件人四级地址名称	
        /// </summary>
        public string TownName { get; set; }

        /// <summary>
        /// 收件人详细地址	
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// 收件人完整地址
        /// </summary>
        public string FullAddress { get; set; }

        /// <summary>
        /// 收件人邮政编码	
        /// </summary>
        public string ZipCode { get; set; }
    }
}