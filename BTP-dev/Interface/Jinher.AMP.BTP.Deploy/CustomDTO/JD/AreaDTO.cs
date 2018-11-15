using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
namespace Jinher.AMP.BTP.Deploy.CustomDTO.JD
{
    /// <summary>
    /// 获取京东地址信息接口
    /// </summary>
    [Serializable]
    [DataContract]
    public class AreaDTO
    {
        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 京东地址信息列表
        /// </summary>
        [DataMember]
        public List<AreaDto> Data { get; set; }
        
    }


    /// <summary>
    /// 获取京东地址信息接口
    /// </summary>
    [Serializable]
    [DataContract]
    public class AreaDto
    {
        /// <summary>
        /// 地址Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 地址名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }


    }



}
